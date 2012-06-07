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
    /// <summary>Wraps a Ban of Procon so that it can be used in the UI.</summary>
    public class BanViewModel : ViewModelBase<Ban>
    {
        // Standard Ban Properties
        public Player Target
        {
            get { return Model.Target; }
        }
        public BanActionType BanActionType
        {
            get { return Model.BanActionType; }
        }
        public TimeSubsetContext Context
        {
            get { return Model.Time.Context; }
        }
        public TimeSpan? Length
        {
            get { return Model.Time.Length; }
            set { Model.Time.Length = value; OnPropertyChanged(this, "Length"); }
        }
        public String Reason
        {
            get { return Model.Reason; }
        }

        // Custom Properties
        public String TargetId
        {
            get { return (!String.IsNullOrEmpty(Target.UID)) ? Target.UID : (!String.IsNullOrEmpty(Target.GUID)) ? Target.GUID : (!String.IsNullOrEmpty(Target.IP)) ? Target.IP : Target.Name; }
        }

        /// <summary>Creates an instance of BanViewModel and initalizes its properties.</summary>
        /// <param name="model">A reference to an instance of a ban in procon.</param>
        public BanViewModel(Ban model) : base(model) { }
    }
}
