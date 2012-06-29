using System;
using System.Xml.Linq;

using Procon.Net.Utils;

namespace Procon.Net.Protocols.Objects
{
    [Serializable]
    public class GameMode : ProtocolObject
    {
        // This is a list of keys used to access common game mode information.  The values
        // for these keys should be set by children classes, because only they know which
        // pieces of information correspond to the common keys.  This is called normalization.
        //
        // See "Player.cs" for an example.
        #region Default (Normalized) Keys used to access common game mode information.

        protected static readonly string C_NAME         = "procon.Name";
        protected static readonly string C_FRIENDY_NAME = "procon.FriendlyName";
        protected static readonly string C_TEAM_COUNT   = "procon.TeamCount";

        #endregion

        public GameMode()
        {
            DataSet(new DataVariable(C_NAME,         null, false, "GameMode.NAME",          "GameMode.NAME_Description"));
            DataSet(new DataVariable(C_FRIENDY_NAME, null, false, "GameMode.FRIENDLY_NAME", "GameMode.FRIENDLY_NAME_Description"));
            DataSet(new DataVariable(C_TEAM_COUNT,   null, false, "GameMode.TEAM_COUNT",    "GameMode.TEAM_COUNT_Description"));

            Name         = String.Empty;
            FriendlyName = String.Empty;
            TeamCount    = 0;
        }

        /// <summary>This game mode's name as it is used via Rcon.</summary>
        public string Name
        {
            get { return TryGetVariable<string>(C_NAME, null); }
            set {
                if (Name != value) {
                    DataSet(C_NAME, value);
                    OnPropertyChanged("Name");
        } } }
        /// <summary>This game mode's human-readable name.</summary>
        public string FriendlyName
        {
            get { return TryGetVariable<string>(C_FRIENDY_NAME, null); }
            set {
                if (FriendlyName != value) {
                    DataSet(C_FRIENDY_NAME, value);
                    OnPropertyChanged("FriendlyName");
        } } }
        /// <summary>This game mode's number of teams, not including spectator/neutral.</summary>
        public int TeamCount
        {
            get { return TryGetVariable<int>(C_TEAM_COUNT, 0); }
            set {
                if (TeamCount != value) {
                    DataSet(C_TEAM_COUNT, value);
                    OnPropertyChanged("TeamCount");
        } } }


        /// <summary>Deserializes game mode information received via a network.</summary>
        public virtual GameMode Deserialize(XElement element) {
            Name         = element.ElementValue("name");
            FriendlyName = element.ElementValue("friendlyname");
            TeamCount    = int.Parse(element.ElementValue("teamcount"));
            return this;
        }
    }
}
