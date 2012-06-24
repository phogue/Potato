using System;
using System.ComponentModel;

using Procon.Net.Protocols.Objects;
using Procon.UI.API.Utils;

namespace Procon.UI.API.ViewModels
{
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
