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
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Procon.Core.Interfaces.RSS.Objects {
    using Procon.Core.Utils;

    public class RSSNews : RSS<RSSNews> {

        /// <summary>
        /// The title of the news article
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Link back to the article on the phogue.net blog
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// The date the article was published
        /// </summary>
        public DateTime PublishDate { get; private set; }

        /// <summary>
        /// The brief description of the article
        /// </summary>
        public string Content { get; private set; }

        public RSSNews Parse(XElement element) {

            DateTime pubDate = DateTime.Now.AddMonths(-1);
            DateTime.TryParse(element.ElementValue("pubDate"), out pubDate);

            this.Title = element.ElementValue("title");
            this.Link = element.ElementValue("link");
            this.Content = element.ElementValue("description");
            this.PublishDate = pubDate;

            return this;
        }
    }
}
