using System;

namespace Procon.Core.Shared {

    /// <summary>
    /// These commands can originate from a config file or from a connected layer client
    /// </summary>
    [Serializable]
    public enum CommandType {
        None,

        /// <summary>
        /// General query to fetch all of the connections, security group and variables. Anything in
        /// the Instance class pretty much.
        /// </summary>
        InstanceQuery,

        /// <summary>
        /// Sends a signal to the service controller to restart Procon, applying any updates.
        /// </summary>
        InstanceServiceRestart,

        /// <summary>
        /// Queries information related specificy to a connection. Note that this will essentially query all
        /// other list queries below (NetworkProtocolQuery*) and populate a single request with the results.
        /// </summary>
        ConnectionQuery,

        LanguageLocalize,
        
        InstanceAddConnection,
        InstanceRemoveConnection,

        /// <summary>
        /// Enables a plugin, allowing events/commands to be passed through to the plugin
        /// </summary>
        PluginsEnable,
        /// <summary>
        /// Disables  plugin, preventing events/commands being tunneled
        /// </summary>
        PluginsDisable,

        #region Security

        SecurityQueryPermission,

        // Groups

        SecurityAddGroup,
        SecurityRemoveGroup,
        SecurityGroupSetPermission,
        SecurityGroupCopyPermissions,

        // Accounts

        SecurityGroupAddAccount,
        SecurityRemoveAccount,

        SecurityAccountAuthenticate,
        SecurityAccountSetPassword,
        SecurityAccountSetPasswordHash,

        SecurityAccountSetPreferredLanguageCode,

        // Players

        SecurityAccountAddPlayer,
        SecurityRemovePlayer,

        #endregion

        /// <summary>
        /// Queries the database controller, dispatching a query to an open driver.
        /// </summary>
        DatabaseQuery,

        TextCommandsExecute,
        TextCommandsPreview,
        TextCommandsRegister,
        TextCommandsUnregister,

        /// <summary>
        /// Installs or updates a package to the latest version available.
        /// </summary>
        PackagesMergePackage,
        /// <summary>
        /// Uninstalls a package.
        /// </summary>
        PackagesUninstallPackage,
        /// <summary>
        /// Checks for the latest versions of all packages from source repositories. This
        /// command is dispatched asynchronously, so a later even will then need to
        /// be listened for when the packages list finally updates itself.
        /// </summary>
        PackagesFetchPackages,

        VariablesSet,
        VariablesGet,
        VariablesSetA,

        EventsFetchAfterEventId,

        // Game queries

        /// <summary>
        /// Fetches a list of players in the server right now
        /// </summary>
        NetworkProtocolQueryPlayers,
        /// <summary>
        /// Fetches a full list of bans on the server, right now.
        /// </summary>
        NetworkProtocolQueryBans,
        /// <summary>
        /// Fetches a list of maps currently running on the server
        /// </summary>
        NetworkProtocolQueryMaps,
        /// <summary>
        /// Fetches a list of maps available on the server right now
        /// </summary>
        NetworkProtocolQueryMapPool,
        /// <summary>
        /// Fetches a list of variables currently set on the server.
        /// </summary>
        NetworkProtocolQuerySettings,


        // Game actions

        /// <summary>
        /// Raw packet to send to the server.
        /// </summary>
        NetworkProtocolActionRaw,
        NetworkProtocolActionChat,
        NetworkProtocolActionKill,
        NetworkProtocolActionMap,
        NetworkProtocolActionMove,
        NetworkProtocolActionKick,
        NetworkProtocolActionBan
    }
}
