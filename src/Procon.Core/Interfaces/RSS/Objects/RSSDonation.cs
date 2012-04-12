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
using System.Globalization;

namespace Procon.Core.Interfaces.RSS.Objects {
    using Procon.Core.Utils;
    public class RSSDonation : RSS<RSSDonation> {

        /// <summary>
        /// The currency used, generally USD
        /// </summary>
        public string Currency { get; private set; }
        
        /// <summary>
        /// The amount donated
        /// </summary>
        public float Amount { get; private set; }

        /// <summary>
        /// The date of the donation
        /// </summary>
        public DateTime PublishDate { get; private set; }

        /// <summary>
        /// The name supplied by the donator 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Link supplied by the donator back to their website
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// Small comment supplied by the donator - "Thanks" or comment back to their website
        /// </summary>
        public string Comment { get; private set; }

        public RSSDonation Parse(XElement element) {

            DateTime date = DateTime.Now.AddMonths(-1);
            DateTime.TryParse(element.ElementValue("date"), out date);

            float amount = 0.0F;
            float.TryParse(element.ElementValue("amount"), NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out amount);

            this.Currency = element.ElementValue("currency");
            this.Amount = amount;
            this.PublishDate = date;
            this.Name = element.ElementValue("name");
            this.Link = element.ElementValue("link");
            this.Comment = element.ElementValue("comment");

            return this;
        }

    }
}
