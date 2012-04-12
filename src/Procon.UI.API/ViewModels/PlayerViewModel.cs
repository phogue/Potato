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
using System.ComponentModel;

using Procon.Net.Protocols.Objects;
using Procon.UI.API.Utils;

namespace Procon.UI.API.ViewModels
{
    /// <summary>Wraps a Player of Procon so that it can be used in the UI.</summary>
    public class PlayerViewModel : ViewModel<Player>
    {
        // Standard Player Properties
        public String Uid
        {
            get { return Model.UID; }
        }
        public String GUID
        {
            get { return Model.GUID; }
        }
        public UInt32 SlotID
        {
            get { return Model.SlotID; }
        }
        public String ClanTag
        {
            get { return Model.ClanTag; }
        }
        public String Name
        {
            get { return Model.Name; }
        }
        public String NameStripped
        {
            get { return Model.NameStripped; }
        }
        public Team Team
        {
            get { return Model.Team; }
        }
        public Squad Squad
        {
            get { return Model.Squad; }
        }
        public Role Role
        {
            get { return Model.Role; }
        }
        public Inventory Inventory
        {
            get { return Model.Inventory; }
        }
        public Int32 Score
        {
            get { return Model.Score; }
        }
        public Int32 Kills
        {
            get { return Model.Kills; }
        }
        public Int32 Deaths
        {
            get { return Model.Deaths; }
        }
        public Double Kdr
        {
            get { return Model.Kdr; }
        }
        public String Country
        {
            get { return Model.CountryName; }
        }
        public String CountryCode
        {
            get { return Model.CountryCode; }
        }
        public String IP
        {
            get { return Model.IP; }
        }
        public String Port
        {
            get { return Model.Port; }
        } 
        public UInt32 Ping
        {
            get { return Model.Ping; }
        }

        // Custom Properties
        public String TeamName
        {
            get { return Localizer.Loc("Procon.UI.API.Teams." + Team); }
        }
        public String SquadName
        {
            get { return Localizer.Loc("Procon.UI.API.Squads." + Squad); }
        }
        public String RoleName
        {
            get { return Role != null ? Localizer.Loc("Procon.UI.API.Roles." + Role.Name) : null; }
        }

        /// <summary>Creates an instance of PlayerViewModel and initalizes its properties.</summary>
        /// <param name="model">A reference to an instance of a player in procon.</param>
        public PlayerViewModel(Player model) : base(model)
        {
            // Listen for changes within the model:
            Model.PropertyChanged += PropertyHasChanged;
        }

        /// <summary>
        /// Is notified when a change is made to one of the model's properties.  We pass on the
        /// notification so the UI knows the property has been changed.
        /// </summary>
        private void PropertyHasChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(this, e.PropertyName);
            switch (e.PropertyName)
            {
                case "Team":
                    OnPropertyChanged(this, "TeamName");
                    break;
                case "Squad":
                    OnPropertyChanged(this, "SquadName");
                    break;
                case "Role":
                    OnPropertyChanged(this, "RoleName");
                    break;
            }
        }
    }
}
