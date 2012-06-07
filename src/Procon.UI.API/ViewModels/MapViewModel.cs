// Copyright 2011 Cameron 'Imisnew2' Gunnin
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

using Procon.Net.Protocols.Objects;
using Procon.UI.API.Utils;

namespace Procon.UI.API.ViewModels
{
    /// <summary>Wraps a Map of Procon so that it can be used in the UI.</summary>
    public class MapViewModel : ViewModelBase<Map>
    {
        // Standard Map Properties
        public Int32 Index
        {
            get { return Model.Index; }
        }
        public GameMode GameMode
        {
            get { return Model.GameMode; }
        }
        public String Name
        {
            get { return Model.Name; }
        }
        public String FriendlyName
        {
            get { return Model.FriendlyName; }
        }
        public Int32 Rounds
        {
            get { return Model.Rounds; }
        }

        // Custom Properties
        public String GameModeName
        {
            get { return GameMode.FriendlyName; } //Localizer.Loc("Procon.UI.API.GameModes." + GameMode.FriendlyName); }
        }

        /// <summary>Creates an instance of MapViewModel and initalizes its properties.</summary>
        /// <param name="model">A reference to an instance of a map in procon.</param>
        public MapViewModel(Map model) : base(model) { }
    }
}
