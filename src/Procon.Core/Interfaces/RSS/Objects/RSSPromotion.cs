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

    public class RSSPromotion : RSS<RSSPromotion> {

        /// <summary>
        /// The friendly name of the promotion
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Link to the image ("phogue.net/images/image.png")
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        /// The link to follow when the user goes to the promotion
        /// ("phogue.net")
        /// </summary>
        public string Link { get; private set; }

        public RSSPromotion Parse(XElement element) {
            this.Name = element.ElementValue("name");
            this.Image = element.ElementValue("image");
            this.Link = element.ElementValue("link");

            return this;
        }
    }
}
