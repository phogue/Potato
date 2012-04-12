// Copyright 2011 Geoffrey 'Phogue' Green
// 
// Altered by Cameron 'Imisnew2' Gunnin
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
using System.Xml.Linq;
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils;

namespace Procon.Net.Protocols.Frostbite.Objects
{
    [Serializable]
    public class FrostbiteGameMode : GameMode
    {
        /// <summary>The default squad to move the player to on a move command.</summary>
        public virtual Squad DefaultSquad { get; set; }



        /// <summary>Initializes the frostbite game mode with some default values.</summary>
        public FrostbiteGameMode() { }

        /// <summary>Deserializes frostbite game mode information received via a network.</summary>
        public override GameMode Deserialize(XElement element) {
            if (Enum.IsDefined(typeof(Squad), element.ElementValue("defaultsquad")) == true)
                DefaultSquad = (Squad)Enum.Parse(typeof(Squad), element.ElementValue("defaultsquad"));
            return base.Deserialize(element);
        }
    }
}
