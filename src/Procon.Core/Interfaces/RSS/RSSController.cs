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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Procon.Core.Interfaces.RSS {
    using Procon.Core.Utils;
    using Procon.Net.Utils.HTTP;
    using Procon.Core.Interfaces.RSS.Objects;

    [Obsolete]
    public class RSSController : Executable<RSSController> {

        public RSSDocument Document { get; private set; }

        public string Uri { get; set; }

        public delegate void FetchCompleteHandler(RSSController sender, RSSDocument document);
        public event FetchCompleteHandler FetchComplete;

        public RSSController() {
            this.Document = new Objects.RSSDocument();
            this.Uri = Defines.PHOGUE_NET_PHOGUE_RSS_PHP;
        }



        #region Executable

        /// <summary>
        /// Begins the execution of this rss controller.
        /// Sends out a request for information across the interwebs!
        /// </summary>
        public override RSSController Execute()
        {
            Request request = new Request(this.Uri);
            request.RequestComplete += new Request.RequestEventDelegate(request_RequestComplete);
            request.BeginRequest();

            return base.Execute();
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        protected override void WriteConfig(XElement xNamespace, ref FileInfo xFile) { }

        #endregion



        private void request_RequestComplete(Request sender) {

            string data = Encoding.UTF8.GetString(sender.CompleteFileData);

            try {
                XElement element = XElement.Parse(data);

                this.Document = new RSSDocument().Parse(element);

                if (this.FetchComplete != null) {
                    this.FetchComplete(this, this.Document);
                }
            }
            catch (Exception) {

            }
        }
    }
}
