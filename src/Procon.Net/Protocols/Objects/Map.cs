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
        
        protected static readonly string C_TYPE         = "procon.Type";
        protected static readonly string C_INDEX        = "procon.Index";
        protected static readonly string C_ROUNDS       = "procon.Rounds";
        protected static readonly string C_NAME         = "procon.Name";
        protected static readonly string C_FRIENDY_NAME = "procon.FriendlyName";
        protected static readonly string C_GAME_MODE    = "procon.Mode";

        #endregion

        public Map() {
            DataSet(new DataVariable(C_TYPE,         null, false, "Map.TYPE",          "Map.TYPE_Description"));
            DataSet(new DataVariable(C_INDEX,        null, false, "Map.INDEX",         "Map.INDEX_Description"));
            DataSet(new DataVariable(C_ROUNDS,       null, false, "Map.ROUNDS",        "Map.ROUNDS_Description"));
            DataSet(new DataVariable(C_NAME,         null, false, "Map.NAME",          "Map.NAME_Description"));
            DataSet(new DataVariable(C_FRIENDY_NAME, null, false, "Map.FRIENDLY_NAME", "Map.FRIENDLY_NAME_Description"));
            DataSet(new DataVariable(C_GAME_MODE,    null, false, "Map.GAME_MODE",     "Map.GAME_MODE_Description"));

            Name         = String.Empty;
            FriendlyName = String.Empty;
            GameMode     = new GameMode();
        }

        /// <summary>Represents how this map object is supposed to be interpreted.</summary>
        public virtual MapActionType MapActionType
        {
            get { return TryGetVariable<MapActionType>(C_TYPE, Objects.MapActionType.Append); }
            set {
                if (MapActionType != value) {
                    DataSet(C_TYPE, value);
                    OnPropertyChanged("MapActionType");
        } } }

        /// <summary>This map's order as represented by a 0-based index.</summary>
        public virtual int Index
        {
            get { return TryGetVariable<int>(C_INDEX, 0); }
            set {
                if (Index != value) {
                    DataSet(C_INDEX, value);
                    OnPropertyChanged("Index");
        } } }
        /// <summary>This map's number of times it repeats before ending.</summary>
        public virtual int Rounds
        {
            get { return TryGetVariable<int>(C_ROUNDS, 0); }
            set {
                if (Rounds != value) {
                    DataSet(C_ROUNDS, value);
                    OnPropertyChanged("Rounds");
        } } }

        /// <summary>This map's name as it is used via Rcon.</summary>
        public virtual string Name
        {
            get { return TryGetVariable<string>(C_NAME, null); }
            set {
                if (Name != value) {
                    DataSet(C_NAME, value);
                    OnPropertyChanged("Name");
        } } }
        /// <summary>This map's human-readable name.</summary>
        public virtual string FriendlyName
        {
            get { return TryGetVariable<string>(C_FRIENDY_NAME, null); }
            set {
                if (FriendlyName != value) {
                    DataSet(C_FRIENDY_NAME, value);
                    OnPropertyChanged("FriendlyName");
        } } }

        /// <summary>This map's game mode.</summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual GameMode GameMode
        {
            get { return TryGetVariable<GameMode>(C_GAME_MODE, null); }
            set {
                if (GameMode != value) {
                    DataSet(C_GAME_MODE, value);
                    OnPropertyChanged("GameMode");
        } } }



        /// <summary>Deserializes map information received via a network.</summary>
        public virtual Map Deserialize(XElement element) {
            Name          = element.ElementValue("name");
            FriendlyName  = element.ElementValue("friendlyname");
            MapActionType = MapActionType.Pooled;
            return this;
        }
    }
}
