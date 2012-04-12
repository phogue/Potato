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
using System.ComponentModel;
using System.Xml.Linq;
using Procon.Net.Utils;

namespace Procon.Net.Protocols.Objects
{
    [Serializable]
    public class Map : ProtocolObject
    {
        // This is a list of keys used to access common map information.  The values for these
        // keys should be set by children classes, because only they know which pieces of
        // information correspond to the common keys.  This is called normalization.
        //
        // See "Player.cs" for an example.
        #region Default (Normalized) Keys used to access common map information.

        protected static readonly string C_INDEX        = "procon.Index";
        protected static readonly string C_ROUNDS       = "procon.Rounds";
        protected static readonly string C_NAME         = "procon.Name";
        protected static readonly string C_FRIENDY_NAME = "procon.FriendlyName";
        protected static readonly string C_GAME_MODE    = "procon.Mode";

        #endregion



        /// <summary>Represents how this map object is supposed to be interpreted.</summary>
        public MapActionType MapActionType { get; set; }

        /// <summary>This map's order as represented by a 0-based index.</summary>
        public virtual int Index
        {
            get { return TryGetVariable<int>(C_INDEX, 0); }
            set
            {
                DataAddSet(C_INDEX, value);
                OnPropertyChanged("Index");
            }
        }
        /// <summary>This map's number of times it repeats before ending.</summary>
        public virtual int Rounds
        {
            get { return TryGetVariable<int>(C_ROUNDS, 0); }
            set
            {
                DataAddSet(C_ROUNDS, value);
                OnPropertyChanged("Rounds");
            }
        }

        /// <summary>This map's name as it is used via Rcon.</summary>
        public virtual string Name
        {
            get { return TryGetVariable<string>(C_NAME, null); }
            set
            {
                DataAddSet(C_NAME, value);
                OnPropertyChanged("Name");
            }
        }
        /// <summary>This map's human-readable name.</summary>
        public virtual string FriendlyName
        {
            get { return TryGetVariable<string>(C_FRIENDY_NAME, null); }
            set
            {
                DataAddSet(C_FRIENDY_NAME, value);
                OnPropertyChanged("FriendlyName");
            }
        }

        /// <summary>This map's game mode.</summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual GameMode GameMode { get; set; }



        /// <summary>Initializes the map with some default values.</summary>
        public Map() {
            DataAdd(C_NAME,         new DataVariable(C_NAME,         String.Empty));
            DataAdd(C_FRIENDY_NAME, new DataVariable(C_FRIENDY_NAME, String.Empty));
            DataAdd(C_GAME_MODE,    new DataVariable(C_GAME_MODE,    new GameMode()));
        }

        /// <summary>Deserializes map information received via a network.</summary>
        public virtual Map Deserialize(XElement element) {
            Name          = element.ElementValue("name");
            FriendlyName  = element.ElementValue("friendlyname");
            MapActionType = MapActionType.Pooled;
            return this;
        }
    }
}
