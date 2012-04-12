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
using System.Windows;
using System.Windows.Data;
using Procon.Core;
using Procon.Core.Interfaces;
using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Layer;
using Procon.Core.Interfaces.Packages;
using Procon.Core.Interfaces.Security.Objects;
using Procon.Core.Interfaces.Variables;
using Procon.Net;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;

namespace Procon.UI.Old.Classes
{
    #region Main Window

    /// <summary>
    /// Converts an interface into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: [Local] MyLayerName
    /// Ex: [Remote] MyLayerName <Ip:Port>
    /// Parameter: Sub
    /// Ex: Connections Open: 0
    /// </summary>
    public class InterfaceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Interface i = (values[0] as Interface);
            String    s = (parameter as String);

            // Invalid Value or Parameter
            if (i == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                String layerName = i.Variables.Get(
                    CommandInitiator.Local, 
                    CommonVariableNames.LayerName, 
                    Local.Loc.Loc(null, "Procon.UI.MainWindow.Interfaces", "Unnamed"));
                String hostname  = i.Layer.Hostname;
                UInt16 port      = i.Layer.Port;


                if (i.Layer is LayerListener)
                    return String.Format("[{0}] {1}",
                        Local.Loc.Loc(null, "Procon.UI.MainWindow.Interfaces", "Local"),
                        layerName);
                else
                    return String.Format("[{0}] ({1}) {2} <{3}:{4}>",
                        Local.Loc.Loc(null, "Procon.UI.MainWindow.Interfaces", "Remote"),
                        Local.Loc.Loc(null, "Procon.UI.ConnectionState", i.Layer.ConnectionState.ToString()),
                        layerName,
                        hostname,
                        port);
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return String.Format("{0}: {1}",
                    Local.Loc.Loc(null, "Procon.UI.MainWindow.Interfaces", "ConnectionsOpen"),
                    i.Connections.Count);
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a connection into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: Battlefield Bad Company: 2 <Ip:Port>
    /// Parameter: Sub
    /// Ex: Logged In: Players 0/32
    /// </summary>
    public class ConnectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Connection c = (values[0] as Connection);
            String     s = (parameter as String);

            // Invalid Value or Parameter
            if (c == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                String connType = c.GameType.ToString();
                String hostname = c.Hostname;
                UInt16 port =     c.Port;


                return String.Format("{0} <{1}:{2}>",
                    Local.Loc.Loc(null, "Procon.UI.GameType", connType),
                    hostname,
                    port);
            }
            // Sub Layout
            else if (s == "Sub")
            {
                String connStatus     = c.GameState.ConnectionState.ToString();
                String curPlayerCount = c.GameState.Get(ServerVariableKey.PlayerCount, "?");
                String maxPlayerCount = c.GameState.Get(ServerVariableKey.MaxPlayerCount, "?");


                return String.Format("{0}: {1}/{2} {3}",
                    Local.Loc.Loc(null, "Procon.UI.ConnectionState", connStatus),
                    curPlayerCount,
                    maxPlayerCount,
                    Local.Loc.Loc(null, "Procon.UI.MainWindow.Connections", "Players"));
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a group into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: GroupName
    /// Parameter: Sub
    /// Ex: Assigned Accounts: 0
    /// </summary>
    public class GroupConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Group  g = (values[0] as Group);
            String s = (parameter as String);

            // Invalid Value or Parameter
            if (g == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return g.Name;
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return String.Format("{0}: {1}",
                    Local.Loc.Loc(null, "Procon.UI.MainWindow.Groups", "AssignedAccounts"),
                    g.AssignedAccounts.Count);
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts an account into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: AccountName
    /// Parameter: Sub
    /// Ex: Assigned Games: 0
    /// </summary>
    public class AccountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Account a = (values[0] as Account);
            String  s = (parameter as String);

            // Invalid Value or Parameter
            if (a == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return a.Username;
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return String.Format("{0}: {1}",
                    Local.Loc.Loc(null, "Procon.UI.MainWindow.Accounts", "AssignedGames"),
                    a.Assignments.Count);
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a package into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: PackageName
    /// Parameter: Sub
    /// Ex: PackageType - PackageState
    /// </summary>
    public class PackageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Package p = (values[0] as Package);
            String  s = (parameter as String);

            // Invalid Value or Parameter
            if (p == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return p.Name;
            }
            // Sub Layout
            else if (s == "Sub")
            {
                String packageType  = p.PackageType.ToString();
                String packageState = p.State.ToString();


                return String.Format("{0} - {1}",
                    Local.Loc.Loc(null, "Procon.UI.PackageType", packageType),
                    Local.Loc.Loc(null, "Procon.UI.PackageState", packageState));
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }



    /// <summary>
    /// Converts a server variable into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: ServerVariableName
    /// Parameter: Sub
    /// Ex: Value
    /// </summary>
    public class ServerVariableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ServerVariable sv = (values[0] as ServerVariable);
            String         s = (parameter as String);

            // Invalid Value or Parameter
            if (sv == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return Local.Loc.Loc(null, "Procon.UI.ServerVariable", sv.Key.ToString());
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return sv.Value.ToString();
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a permission into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: PermissionName
    /// Parameter: Sub
    /// Ex: Authority
    /// </summary>
    public class PermissionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Permission p = (values[0] as Permission);
            String     s = (parameter as String);

            // Invalid Value or Parameter
            if (p == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return Local.Loc.Loc(null, "Procon.UI.Permission", p.Name.ToString());
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return (p.Authority.HasValue) ? p.Authority.Value.ToString() : "0";
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts an account assignment into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: GameType
    /// Parameter: Sub
    /// Ex: UID
    /// </summary>
    public class AssignmentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AccountAssignment aa = (values[0] as AccountAssignment);
            String            s  = (parameter as String);

            // Invalid Value or Parameter
            if (aa == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return Local.Loc.Loc(null, "Procon.UI.GameType", aa.GameType.ToString());
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return aa.UID;
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #endregion
    #region GameWindow

    /// <summary>
    /// Localizes the passed in parameter.
    /// </summary>
    public class LocalizePlayerContextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Localizes the passed in parameter.
    /// </summary>
    public class LocalizeBanContextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a map from the map pool into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: MapName
    /// Parameter: Sub
    /// Ex: GameModeName
    /// </summary>
    public class MapPoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Map    m = (values[0] as Map);
            String s = (parameter as String);

            // Invalid Value or Parameter
            if (m == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                return m.FriendlyName;
            }
            // Sub Layout
            else if (s == "Sub")
            {
                return m.GameMode.FriendlyName;
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a map from the map list into readable text.
    /// The parameter passed in specifies which layout to retreive.
    /// Parameter: Main
    /// Ex: MapName
    /// Parameter: Sub
    /// Ex: GameModeName
    /// </summary>
    public class MapListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Map m = (values[0] as Map);
            String s = (parameter as String);

            // Invalid Value or Parameter
            if (m == null || s == null)
                return String.Empty;

            // Main Layout
            if (s == "Main")
            {
                if (m.FriendlyName != null)
                    return m.FriendlyName;
                return m.Name;
            }
            // Sub Layout
            else if (s == "Sub")
            {
                if (m.GameMode != null)
                    return m.GameMode.FriendlyName;
                return String.Empty;
            }
            // Rounds
            else if (s == "Round")
            {
                return String.Format("{0} {1}",
                    m.Rounds,
                    Local.Loc.Loc(null, "Procon.UI.GameWindow.MapList", "Rounds"));
            }
            // Invalid Layout
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #endregion
    #region Other

    /// <summary>
    /// Localizes the bound value.
    /// </summary>
    public class LocalizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Local.Loc.Loc(null, parameter.ToString(), value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts an enumeration into human readable text.
    /// </summary>
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Only convert enumerated values.
            if (value is Enum)
            {
                String path = "Procon.UI." + value.GetType().Name;
                String key = value.ToString();

                return Local.Loc.Loc(null, path, key);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts an interface into a boolean value whether
    /// it is a local interface or not.
    /// </summary>
    public class InterfaceIsLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is LocalInterface;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a connection state into a boolean value whether
    /// it is disconnected or not.
    /// </summary>
    public class LayerIsOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Only check connection types.
            if (value is ConnectionState)
            {
                ConnectionState connState = (ConnectionState)value;

                if (connState == ConnectionState.Disconnected)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts a connection state into a string to display in the
    /// UI.  The string is directly related to layer management.
    /// </summary>
    public class LayerStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Only check connection types.
            if (value is ConnectionState)
                return Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", value.ToString());
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Converts an interface into a boolean value whether
    /// it is a local interface or not.
    /// </summary>
    public class InterfaceVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is LocalInterface) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Checks to see if the passed in game type is a game that
    /// requires additional information to connect to.
    /// </summary>
    public class GameTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Only check game types.
            if (value is GameType)
            {
                GameType gameType = (GameType)value;

                if (gameType == GameType.COD_BO)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Checks to see if the passed in ban type is a ban that
    /// requires a duration.
    /// </summary>
    public class TimeSubsetVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Only check ban types.
            if (value is TimeSubsetContext)
            {
                TimeSubsetContext banType = (TimeSubsetContext)value;

                if (banType == TimeSubsetContext.Time)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #endregion







    #region Numbers

    /// <summary>
    /// Reduces a number by a percentage.
    /// </summary>
    public class NumberReduction : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// Double - The number to reduce.
        /// The parameter passed in is:
        /// Double - The percentage to reduce to.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Double number = (Double)value;
            Double percentage = Double.Parse(parameter as String);

            return number * percentage;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Checks to see if the passed in permission is a valid
    /// permission and if it has any power.  These are used to
    /// determine whether it should be displayed or not.
    /// </summary>
    public class PermissionHeight : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// The permission we're looking at.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Permission perm = (Permission)value;

            // Only display the permission if it is not of the specified
            // types, it's authority is set, and it's authority is > 0.
            if (perm.Name != CommandName.Action && perm.Name != CommandName.Authenticated &&
                perm.Name != CommandName.Hashed && perm.Name != CommandName.Login &&
                perm.Name != CommandName.None && perm.Name != CommandName.Salt &&
                perm.Authority.HasValue && perm.Authority.Value > 0)
                return Double.NaN;
            return 0.0;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #endregion
    #region Visibility

    /// <summary>
    /// Returns a boolean value representing whether the visibility property
    /// passed in is Visible or not.
    /// </summary>
    public class IsVisibleConverter : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// Visibility - We're checking whether or not it's visible.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        /// <summary>
        /// The value passed in is:
        /// Boolean - We're checking whether or not it's visible.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Boolean)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Checks the passed in number by a second specified number
    /// to see if the first is greater than the second.  Returns
    /// Visible on true, Hidden on false.
    /// </summary>
    public class NumberVisibility : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// Int - The number we're checking.
        /// The parameter passed in is:
        /// Int - The threshold to check against.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int32? number = (Int32?)value;
            Int32 threshold = Int32.Parse(parameter as String);


            if (number.HasValue && number.Value > threshold)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Checks to see if the passed in permission is a valid
    /// permission and if it has any power.  These are used to
    /// determine whether it should be displayed or not.
    /// </summary>
    public class PermissionVisibility : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// The permission we're looking at.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Permission perm = (Permission)value;

            // Only display the permission if it is not of the specified
            // types, it's authority is set, and it's authority is > 0.
            if (perm.Name != CommandName.Action && perm.Name != CommandName.Authenticated &&
                perm.Name != CommandName.Hashed && perm.Name != CommandName.Login &&
                perm.Name != CommandName.None && perm.Name != CommandName.Salt &&
                perm.Authority.HasValue && perm.Authority.Value > 0)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #endregion

    #region Localization

    #endregion



    /// <summary>
    /// Converts a date into a short time string.
    /// </summary>
    public class DateConverter : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// DateTime - The date to convert.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;

            return dateTime.ToShortTimeString();
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }



    /// <summary>
    /// Converts a null value into DependencyProperty.UnsetValue.
    /// </summary>
    public class NullToUnsetConverter : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// Object - the value to check.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null || (value is Team && (Team)value == Team.None) || (value is Squad && (Squad)value == Squad.None)) ? DependencyProperty.UnsetValue : value;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #region Game Window

    /// <summary>
    /// Retrieves a specified server variable and localizes it
    /// so it can be displayed in the UI.
    /// </summary>
    public class GameVariableConverter : IValueConverter
    {
        /// <summary>
        /// The value passed in is:
        /// GameState - The game we want to get information for.
        /// The parameter passed in is:
        /// String - The name of the variable we want to retreive.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                GameState         gameState = (GameState)value;
                ServerVariableKey key       = (ServerVariableKey)Enum.Parse(typeof(ServerVariableKey), parameter as String);

                return gameState.Get(key, String.Empty);
            }
            catch (Exception) { return String.Empty; }
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    #endregion
}
