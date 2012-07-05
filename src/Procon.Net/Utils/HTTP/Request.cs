// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Timers;
using System.IO.Compression;

namespace Procon.Net.Utils.HTTP {
    [Serializable]
    public class Request {

        public delegate void RequestEventDelegate(Request sender);
        public event RequestEventDelegate RequestComplete;
        public event RequestEventDelegate RequestError;
        public event RequestEventDelegate RequestDiscoveredFileSize;
        public event RequestEventDelegate RequestProgressUpdate;

        private HttpWebRequest m_webRequest;
        private WebResponse m_webResponse;
        private Stream m_responseStream;

        public string DownloadSource { get; private set; }

        private const int INT_BUFFER_SIZE = UInt16.MaxValue;
        private byte[] ma_bBufferStream;

        //private int m_iReadBytes;
        //private int m_iCompleteFileSize;
        //private byte[] ma_bCompleteFile;

        private System.Timers.Timer m_progressTimer;
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
        /// The WebRequestMethods.Http string representing the type of
        /// method to use in the request.  Default is Get.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The contents of a POST request
        /// </summary>
        public string RequestContent { get; set; }

        private int m_timeout;
        /// <summary>
        /// ReadTimeout of the stream in milliseconds.  Default is 10 seconds.
        /// </summary>
        public int Timeout {
            get {
                return this.m_timeout;
            }
            set {
                this.m_timeout = value;

                if (this.m_responseStream != null) {
                    this.m_responseStream.ReadTimeout = value;
                }
            }
        }

        public string FileName {
            get {
                string strReturnFileName = String.Empty;

                if (this.DownloadSource.Length > 0) {
                    strReturnFileName = this.DownloadSource.Substring(this.DownloadSource.LastIndexOf("/") + 1, (this.DownloadSource.Length - this.DownloadSource.LastIndexOf("/") - 1));
                }

                return strReturnFileName;
            }
        }

        public Request(string downloadSource) {
            this.DownloadSource = downloadSource;

            this.m_timeout = 10000;
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
                    cdfParent.m_webRequest.Abort();
                }
            }
        }

        public void BeginRequest() {
            new Thread(new ThreadStart(this.BeginRequestCallback)).Start();
        }

        private void BeginRequestCallback() {

            this.UnknownSize = true;

            this.BytesDownloaded = 0;
            this.FileSize = 1;

            this.FileDownloading = true;

            this.ma_bBufferStream = new byte[Request.INT_BUFFER_SIZE];

            try {
                this.m_webRequest = (HttpWebRequest)HttpWebRequest.Create(this.DownloadSource);
                // this.m_wrRequest.Referer = "http://www.phogue.net/procon/";
                this.m_webRequest.Method = this.Method;

                //this.m_wrRequest.Headers.Add("Range", "bytes=-10000");
                if (this.Range != null) {
                    this.m_webRequest.AddRange((int)this.Range);
                }
                //this.m_webRequest.AddRange(-888192);
                if (this.Referrer != null) {
                    this.m_webRequest.UserAgent = this.Referrer;
                }
                // Range: bytes=-10000
                //this.m_webRequest.KeepAlive = false;
                //this.m_webRequest.ProtocolVersion = HttpVersion.Version10;
                //this.m_wrRequest.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "Procon 2.0");
                //this.m_wrRequest.Headers.Add(System.Net.HttpRequestHeader.Range, "bytes=-10000");
                this.m_webRequest.Headers.Add(System.Net.HttpRequestHeader.AcceptEncoding, "gzip");

                this.m_webRequest.Proxy = null;

                if (this.RequestContent != null && this.RequestContent.Length > 0) {
                    this.m_webRequest.ContentType = "application/x-www-form-urlencoded";
                    this.m_webRequest.ContentLength = this.RequestContent.Length;

                    Stream newStream = this.m_webRequest.GetRequestStream();
                    // Send the data.
                    newStream.Write(Encoding.UTF8.GetBytes(this.RequestContent), 0, this.RequestContent.Length);
                    newStream.Close();
                }

                if (this.m_webRequest != null) {
                    IAsyncResult arResult = this.m_webRequest.BeginGetResponse(new AsyncCallback(this.ResponseCallback), this);
                    ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(this.RequestTimeoutCallback), this, this.m_timeout, true);

                    if (this.DownloadRate == true) {
                        this.m_progressTimer = new System.Timers.Timer(100);
                        this.m_progressTimer.Elapsed += new ElapsedEventHandler(m_progressTimer_Elapsed);
                        this.m_progressTimer.Start();
                    }
                }
            }
            catch (Exception) { }
        }

        private void ResponseCallback(IAsyncResult ar) {
            //Request cdfParent = (Request)ar.AsyncState;
            
            try {
                this.m_webResponse = this.m_webRequest.EndGetResponse(ar);

                string strContentLength = null;
                if ((strContentLength = this.m_webResponse.Headers["Content-Length"]) != null) {
                    this.FileSize = Convert.ToInt32(strContentLength);
                    this.CompleteFileData = new byte[this.FileSize];

                    this.UnknownSize = false;

                    if (this.RequestDiscoveredFileSize != null) {
                        this.RequestDiscoveredFileSize(this);
                    }
                }
                else {
                    this.CompleteFileData = new byte[0];
                }

                this.m_responseStream = this.m_webResponse.GetResponseStream();

                if (this.m_webResponse.Headers.Get("Content-Encoding") != null && this.m_webResponse.Headers.Get("Content-Encoding").ToLower() == "gzip") {
                    this.m_responseStream = new GZipStream(this.m_responseStream, CompressionMode.Decompress);
                }

                IAsyncResult arResult = this.m_responseStream.BeginRead(this.ma_bBufferStream, 0, Request.INT_BUFFER_SIZE, new AsyncCallback(this.ReadCallBack), this);

                ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(this.ReadTimeoutCallback), this, this.m_timeout, true);
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
                if (cdfParent != null && cdfParent.m_responseStream != null) {
                    cdfParent.m_responseStream.Close();

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
                    if ((iBytesRead = this.m_responseStream.EndRead(ar)) > 0) {

                        if (this.UnknownSize == true) {
                            byte[] resizedFileData = new byte[this.CompleteFileData.Length + iBytesRead];

                            this.CompleteFileData.CopyTo(resizedFileData, 0);

                            this.CompleteFileData = resizedFileData;

                            // Array.Resize<byte>(ref cdfParent.CompleteFileData, cdfParent.CompleteFileData.Length + iBytesRead);
                        }

                        Array.Copy(this.ma_bBufferStream, 0, this.CompleteFileData, this.BytesDownloaded, iBytesRead);
                        this.BytesDownloaded += iBytesRead;

                        IAsyncResult arResult = this.m_responseStream.BeginRead(this.ma_bBufferStream, 0, Request.INT_BUFFER_SIZE, new AsyncCallback(this.ReadCallBack), this);

                        ThreadPool.RegisterWaitForSingleObject(arResult.AsyncWaitHandle, new WaitOrTimerCallback(this.ReadTimeoutCallback), this, this.m_timeout, true);
                    }
                    else {

                        this.FileDownloading = false;
                        if (this.RequestComplete != null) {
                            //FrostbiteConnection.RaiseEvent(cdfParent.DownloadComplete.GetInvocationList(), cdfParent);
                            this.RequestComplete(this);
                        }

                        this.m_responseStream.Close();
                        this.m_responseStream.Dispose();
                        this.m_responseStream = null;
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

            //Request cdfParent = ((Request)obj);

            int iTickCount = 0;
            int[] a_iKiBytesPerTick = new int[50];
            int iPreviousTickReadBytes = 0;

            while (this.FileDownloading == true) {

                a_iKiBytesPerTick[iTickCount] = this.BytesDownloaded - iPreviousTickReadBytes;
                iTickCount = (++iTickCount % 50);

                this.KibPerSecond = 0.0;
                foreach (int iKiBytesTick in a_iKiBytesPerTick) {
                    this.KibPerSecond += iKiBytesTick;
                }

                this.KibPerSecond = this.KibPerSecond / 5120; // / 1024 / 5;

                iPreviousTickReadBytes = this.BytesDownloaded;

                if (this.RequestProgressUpdate != null && iPreviousTickReadBytes > 0) {
                    this.RequestProgressUpdate(this);

                    //FrostbiteConnection.RaiseEvent(cdfParent.DownloadProgressUpdate.GetInvocationList(), cdfParent);
                }

                //Thread.Sleep(100);
            }
        }

        public String GetResponseContent() {
            return Encoding.UTF8.GetString(this.CompleteFileData);
        }

    }
}
