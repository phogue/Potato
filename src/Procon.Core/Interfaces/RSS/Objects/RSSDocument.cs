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
    public class RSSDocument : RSS<RSSDocument> {

        /// <summary>
        /// List of news articles
        /// </summary>
        public List<RSSNews> News { get; private set; }

        public RSSProcon2 Procon2 { get; private set; }

        /// <summary>
        /// List of donations made to phogue.net
        /// </summary>
        public List<RSSDonation> Donations { get; private set; }

        /// <summary>
        /// List of promotional banners
        /// </summary>
        public List<RSSPromotion> Promotions { get; private set; }

        public RSSDocument() {

            this.News = new List<Objects.RSSNews>();
            this.Donations = new List<RSSDonation>();
            this.Promotions = new List<RSSPromotion>();
        }

        public RSSDocument Parse(XElement element) {

            foreach (XElement itemElement in element.Descendants("channel").Descendants("item")) {
                this.News.Add(new RSSNews().Parse(itemElement));
            }

            foreach (XElement donationElement in element.Descendants("donations").Descendants("donation")) {
                this.Donations.Add(new RSSDonation().Parse(donationElement));
            }

            foreach (XElement promotionElement in element.Descendants("promotions").Descendants("promotion")) {
                this.Promotions.Add(new RSSPromotion().Parse(promotionElement));
            }

            this.Procon2 = new RSSProcon2().Parse(element);

            return this;
        }
    }
}
