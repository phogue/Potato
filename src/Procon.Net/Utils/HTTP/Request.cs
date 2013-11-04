using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Timers;

namespace Procon.Net.Utils.HTTP {
    [Serializable]
    public class Request {

        public delegate void RequestEventDelegate(Request sender);
        public event RequestEventDelegate RequestComplete;
        public event RequestEventDelegate RequestError;
        public event RequestEventDelegate RequestDiscoveredFileSize;
        public event RequestEventDelegate RequestProgressUpdate;

        public HttpWebResponse WebResponse;

        private HttpWebRequest _mWebRequest;
        private Stream _mResponseStream;

        public string DownloadSource { get; private set; }

        private const int IntBufferSize = UInt16.MaxValue;
        private byte[] _mBufferStream;

        //private int m_iReadBytes;
        //private int m_iCompleteFileSize;
        //private byte[] ma_bCompleteFile;

        private System.Timers.Timer _mProgressTimer;
        //private Thread m_thProgressTick;

        public bool FileDownloading { get; private set; }

        public bool DownloadRate { get; set; }

        public int BytesDownloaded { get; private set; }

        public int FileSize { get; private set; }

        //private double m_dblKibPerSecond = 0.0;
        public double KibPerSecond { get; private set; }

        public byte[] CompleteFileData { get; private set; }

        // private bool m_blUnknownSize;
        public bool UnknownSize { get; private set; }

        //private object m_objData;
        public object AdditionalData { get; set; }

        //private string m_strLastError;
        public string Error { get; private set; }

        /// <summary>
        /// Optional range to include in the request header
        /// </summary>
        public int? Range { get; set; }

        /// <summary>
        /// Optional referrer to include in the request header
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        /// What content type should be used. If this is left null, then no content type is specified.
        /// </summary>
        public String RequestContentType { get; set; }

        /// <summary>
        /// The WebRequestMethods.Http string representing the type of
        /// method to use in the request.  Default is Get.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The contents of a POST request
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// The contents to build up a multipart request.
        /// 
        /// I'd like this to replace RequestContent at some point
        /// </summary>
        public PostContent PostContent { get; protected set; }

        public CredentialCache CredentialCache { get; protected set; }

        private int _mTimeout;
        /// <summary>
        /// ReadTimeout of the stream in milliseconds.  Default is 10 seconds.
        /// </summary>
        public int Timeout {
            get {
                return this._mTimeout;
            }
            set {
                this._mTimeout = value;

                if (this._mResponseStream != null) {
                    this._mResponseStream.ReadTimeout = value;
                }
            }
        }

        public string FileName {
            get {
                string strReturnFileName = String.Empty;

                if (this.DownloadSource.Length > 0) {
                    strReturnFileName = this.DownloadSource.Substring(this.DownloadSource.LastIndexOf("/", System.StringComparison.Ordinal) + 1, (this.DownloadSource.Length - this.DownloadSource.LastIndexOf("/", System.StringComparison.Ordinal) - 1));
                }

                return strReturnFileName;
            }
        }

        public Request(string downloadSource) {
            this.DownloadSource = downloadSource;

            this.PostContent = new PostContent();
            this.CredentialCache = new CredentialCache();

            this._mTimeout = 10000;
            this.Method = WebRequestMethods.Http.Get;
        }

        public void EndRequest() {
            this.FileDownloading = false;
        }

        public string GetLabelProgress() {
            return String.Format("{0:0.0} KiB of {1:0.0} KiB @ {2:0.0} KiB/s", this.BytesDownloaded / 1024, this.FileSize / 1024, this.KibPerSecond);
        }

        private void RequestTimeoutCallback(object state, bool timedOut) {
            if (timedOut == true) {
                Request cdfParent = (Request)state;
                if (cdfParent != null) {
                    cdfParent._mWebRequest.Abort();
                }
            }
        }

        public void BeginRequest() {
            new Thread(new ThreadStart(this.BeginRequestCallback)).Start();
        }

        private void ProcessPostRequest() {
            if (string.IsNullOrEmpty(this.RequestContent) == false) {
                this._mWebRequest.ContentType = this.RequestContentType ?? "application/x-www-form-urlencoded";

                this._mWebRequest.ContentLength = this.RequestContent.Length;

                using (Stream postStream = this._mWebRequest.GetRequestStream()) {
                    postStream.Write(Encoding.UTF8.GetBytes(this.RequestContent), 0, this.RequestContent.Length);
                }
                //Stream newStream = this.m_webRequest.GetRequestStream();
                // Send the data.
                //newStream.Write(Encoding.UTF8.GetBytes(this.RequestContent), 0, this.RequestContent.Length);
                //newStream.Close();
            }
        }

        private void ProcessMultipartPostRequest() {
            if (this.PostContent.Params.Count > 0) {

                byte[] payload = this.PostContent.BuildPostData();

                // Ignores the specified method
                this._mWebRequest.Method = "POST";
                this._mWebRequest.ContentLength = payload.Length;
                this._mWebRequest.ContentType = "multipart/form-data; boundary=" + this.PostContent.Boundry;

                using (Stream postStream = this._mWebRequest.GetRequestStream()) {
                    postStream.Write(payload);
                }
            }
        }

        private void BeginRequestCallback() {

            this.UnknownSize = true;

            this.BytesDownloaded = 0;
            this.FileSize = 1;

            this.FileDownloading = true;

            this._mBufferStream = new byte[Request.IntBufferSize];

            try {
                this._mWebRequest = WebRequest.Create(this.DownloadSource) as HttpWebRequest;

                if (this._mWebRequest != null) {
                    // this.m_wrRequest.Referer = "http://www.phogue.net/procon/";
                    this._mWebRequest.Method = this.Method;
                    this._mWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    //this.m_wrRequest.Headers.Add("Range", "bytes=-10000");
                    if (this.Range != null) {
                        this._mWebRequest.AddRange((int)this.Range);
                    }
                    //this.m_webRequest.AddRange(-888192);
                    if (this.Referrer != null) {
                        this._mWebRequest.UserAgent = this.Referrer;
                    }
                    // Range: bytes=-10000
                    //this.m_webRequest.KeepAlive = false;
                    //this.m_webRequest.ProtocolVersion = HttpVersion.Version10;
                    //this.m_wrRequest.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "Procon 2.0");
                    //this.m_wrRequest.Headers.Add(System.Net.HttpRequestHeader.Range, "bytes=-10000");
                    this._mWebRequest.Headers.Add(System.Net.HttpRequestHeader.AcceptEncoding, "gzip");

                    this._mWebRequest.Credentials = this.CredentialCache;

                    this._mWebRequest.Proxy = null;

                    this.ProcessPostRequest();

                    this.ProcessMultipartPostRequest();

                    IAsyncResult arResult = this._mWebRequest.BeginGetResponse(new AsyncCallback(this.ResponseCallback), this);
                    ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(this.RequestTimeoutCallback), this, this._mTimeout, true);

                    if (this.DownloadRate == true) {
                        this._mProgressTimer = new System.Timers.Timer(100);
                        this._mProgressTimer.Elapsed += new ElapsedEventHandler(m_progressTimer_Elapsed);
                        this._mProgressTimer.Start();
                    }
                }
            }
            catch (Exception e) {
                this.FileDownloading = false;
                if (this.RequestError != null) {
                    this.Error = e.Message;

                    this.RequestError(this);
                }
            }
        }

        private void ResponseCallback(IAsyncResult ar) {
            //Request cdfParent = (Request)ar.AsyncState;

            try {
                this.WebResponse = (HttpWebResponse)this._mWebRequest.EndGetResponse(ar);

                string strContentLength = null;
                if ((strContentLength = this.WebResponse.Headers["Content-Length"]) != null) {
                    this.FileSize = Convert.ToInt32(strContentLength);

                    this.UnknownSize = false;

                    if (this.RequestDiscoveredFileSize != null) {
                        this.RequestDiscoveredFileSize(this);
                    }
                }

                this.CompleteFileData = new byte[0];

                this._mResponseStream = this.WebResponse.GetResponseStream();

                if (this._mResponseStream != null) {
                    IAsyncResult arResult = this._mResponseStream.BeginRead(this._mBufferStream, 0, Request.IntBufferSize, new AsyncCallback(this.ReadCallBack), this);

                    ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(this.ReadTimeoutCallback), this, this._mTimeout, true);
                }
            }
            catch (WebException e) {
                this.FileDownloading = false;
                if (this.RequestError != null) {
                    this.WebResponse = (HttpWebResponse)e.Response;
                    this.Error = e.Message;

                    //FrostbiteConnection.RaiseEvent(cdfParent.DownloadError.GetInvocationList(), cdfParent);
                    this.RequestError(this);
                }
            }
            catch (Exception e) {
                this.FileDownloading = false;
                if (this.RequestError != null) {
                    this.Error = e.Message;

                    //FrostbiteConnection.RaiseEvent(cdfParent.DownloadError.GetInvocationList(), cdfParent);
                    this.RequestError(this);
                }
            }
        }

        private void ReadTimeoutCallback(object state, bool timedOut) {
            if (timedOut == true) {
                Request cdfParent = (Request)state;
                if (cdfParent != null && cdfParent._mResponseStream != null) {
                    cdfParent._mResponseStream.Close();

                    if (cdfParent.RequestError != null) {
                        cdfParent.Error = "Read Timeout";

                        //FrostbiteConnection.RaiseEvent(cdfParent.DownloadError.GetInvocationList(), cdfParent);
                        cdfParent.RequestError(cdfParent);
                    }
                }
            }
        }

        private void ReadCallBack(IAsyncResult ar) {
            //Request cdfParent = (Request)ar.AsyncState;

            if (this.FileDownloading == true) {
                try {

                    int iBytesRead = -1;
                    if ((iBytesRead = this._mResponseStream.EndRead(ar)) > 0) {

                        if (this.CompleteFileData.Length < this.BytesDownloaded + iBytesRead) {
                            byte[] resizedFileData = new byte[this.CompleteFileData.Length + iBytesRead];

                            this.CompleteFileData.CopyTo(resizedFileData, 0);

                            this.CompleteFileData = resizedFileData;
                        }

                        Array.Copy(this._mBufferStream, 0, this.CompleteFileData, this.BytesDownloaded, iBytesRead);
                        this.BytesDownloaded += iBytesRead;

                        IAsyncResult arResult = this._mResponseStream.BeginRead(this._mBufferStream, 0, Request.IntBufferSize, new AsyncCallback(this.ReadCallBack), this);

                        ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(this.ReadTimeoutCallback), this, this._mTimeout, true);
                    }
                    else {

                        this.FileDownloading = false;
                        if (this.RequestComplete != null) {
                            //FrostbiteConnection.RaiseEvent(cdfParent.DownloadComplete.GetInvocationList(), cdfParent);
                            this.RequestComplete(this);
                        }

                        this._mResponseStream.Close();
                        this._mResponseStream.Dispose();
                        this._mResponseStream = null;
                    }
                }
                catch (Exception e) {
                    this.FileDownloading = false;
                    if (this.RequestError != null) {
                        this.Error = e.Message;

                        //FrostbiteConnection.RaiseEvent(cdfParent.DownloadError.GetInvocationList(), cdfParent);
                        this.RequestError(this);
                    }
                }
            }
        }

        private void m_progressTimer_Elapsed(object sender, ElapsedEventArgs e) {

            int tickCount = 0;
            int[] bytesPerTick = new int[50];
            int previousTickReadBytes = 0;

            while (this.FileDownloading == true) {

                bytesPerTick[tickCount] = this.BytesDownloaded - previousTickReadBytes;
                tickCount = (++tickCount % 50);

                this.KibPerSecond = 0.0;
                foreach (int iKiBytesTick in bytesPerTick) {
                    this.KibPerSecond += iKiBytesTick;
                }

                this.KibPerSecond = this.KibPerSecond / 5120; // / 1024 / 5;

                previousTickReadBytes = this.BytesDownloaded;

                if (this.RequestProgressUpdate != null && previousTickReadBytes > 0) {
                    this.RequestProgressUpdate(this);

                    //FrostbiteConnection.RaiseEvent(cdfParent.DownloadProgressUpdate.GetInvocationList(), cdfParent);
                }

                //Thread.Sleep(100);
            }
        }

        public String GetResponseContent() {
            return this.CompleteFileData != null ? Encoding.UTF8.GetString(this.CompleteFileData) : String.Empty;
        }

    }
}
