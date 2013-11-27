using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Procon.Net.Utils.HTTP {
    [Serializable]
    [Obsolete]
    public class Request {

        public HttpWebResponse WebResponse;

        private HttpWebRequest _mWebRequest;
        private Stream _mResponseStream;

        public string DownloadSource { get; private set; }

        protected byte[] BufferStream;

        public bool FileDownloading { get; private set; }

        /// <summary>
        /// Total numbers of bytes downloaded so far.
        /// </summary>
        protected int BytesDownloaded { get; private set; }

        /// <summary>
        /// The completed file size
        /// </summary>
        protected int FileSize { get; private set; }

        /// <summary>
        /// All of the data downloaded.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The stringified error
        /// </summary>
        public string RequestError { get; private set; }

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
        private int _mTimeout;

        public string FileName {
            get {
                string fileName = String.Empty;

                if (this.DownloadSource.Length > 0) {
                    fileName = this.DownloadSource.Substring(this.DownloadSource.LastIndexOf("/", System.StringComparison.Ordinal) + 1, (this.DownloadSource.Length - this.DownloadSource.LastIndexOf("/", System.StringComparison.Ordinal) - 1));
                }

                return fileName;
            }
        }

        /// <summary>
        /// Fired when the request has completted successfully.
        /// </summary>
        public event RequestEventDelegate Complete;

        protected void OnComplete() {
            var handler = this.Complete;
            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Fired when an error occures during the request/response.
        /// </summary>
        public event RequestEventDelegate Error;

        protected void OnError() {
            var handler = this.Error;
            if (handler != null) {
                handler(this);
            }
        }

        public delegate void RequestEventDelegate(Request sender);

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

        private static void RequestTimeoutCallback(object state, bool timedOut) {
            if (timedOut == true) {
                Request parent = (Request)state;
                if (parent != null) {
                    parent._mWebRequest.Abort();
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

            this.BytesDownloaded = 0;
            this.FileSize = 1;

            this.FileDownloading = true;

            this.BufferStream = new byte[UInt16.MaxValue];

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
                    ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(RequestTimeoutCallback), this, this._mTimeout, true);
                }
            }
            catch (Exception e) {
                this.FileDownloading = false;
                this.RequestError = e.Message;
                this.OnError();
            }
        }

        private void ResponseCallback(IAsyncResult ar) {
            try {
                this.WebResponse = (HttpWebResponse)this._mWebRequest.EndGetResponse(ar);

                string contentLength = null;
                if ((contentLength = this.WebResponse.Headers["Content-Length"]) != null) {
                    this.FileSize = Convert.ToInt32(contentLength);
                }

                this.Data = new byte[0];

                this._mResponseStream = this.WebResponse.GetResponseStream();

                if (this._mResponseStream != null) {
                    IAsyncResult arResult = this._mResponseStream.BeginRead(this.BufferStream, 0, UInt16.MaxValue, new AsyncCallback(this.ReadCallBack), this);

                    ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(ReadTimeoutCallback), this, this._mTimeout, true);
                }
            }
            catch (WebException e) {
                this.FileDownloading = false;
                this.WebResponse = (HttpWebResponse)e.Response;
                this.RequestError = e.Message;
                this.OnError();
            }
            catch (Exception e) {
                this.FileDownloading = false;
                this.RequestError = e.Message;

                this.OnError();
            }
        }

        private static void ReadTimeoutCallback(object state, bool timedOut) {
            if (timedOut == true) {
                Request parent = (Request)state;
                if (parent != null && parent._mResponseStream != null) {
                    parent._mResponseStream.Close();
                    parent.RequestError = "Read Timeout";
                    parent.OnError();
                }
            }
        }

        private void ReadCallBack(IAsyncResult ar) {
            if (this.FileDownloading == true) {
                try {

                    int bytesRead = -1;
                    if ((bytesRead = this._mResponseStream.EndRead(ar)) > 0) {

                        if (this.Data.Length < this.BytesDownloaded + bytesRead) {
                            byte[] resizedFileData = new byte[this.Data.Length + bytesRead];

                            this.Data.CopyTo(resizedFileData, 0);

                            this.Data = resizedFileData;
                        }

                        Array.Copy(this.BufferStream, 0, this.Data, this.BytesDownloaded, bytesRead);
                        this.BytesDownloaded += bytesRead;

                        IAsyncResult result = this._mResponseStream.BeginRead(this.BufferStream, 0, UInt16.MaxValue, new AsyncCallback(this.ReadCallBack), this);

                        ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(ReadTimeoutCallback), this, this._mTimeout, true);
                    }
                    else {

                        this.FileDownloading = false;
                        this.OnComplete();

                        this._mResponseStream.Close();
                        this._mResponseStream.Dispose();
                        this._mResponseStream = null;
                    }
                }
                catch (Exception e) {
                    this.FileDownloading = false;
                    this.RequestError = e.Message;

                    this.OnError();
                }
            }
        }

        public String GetResponseContent() {
            return this.Data != null ? Encoding.UTF8.GetString(this.Data) : String.Empty;
        }
    }
}
