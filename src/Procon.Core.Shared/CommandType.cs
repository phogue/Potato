using System;
using System.ComponentModel;

namespace Procon.Core.Shared {

    /// <summary>
    /// These commands can originate from a config file or from a connected layer client
    /// </summary>
    [Serializable]
    public enum CommandType {
        /// <summary>
        /// No command type specified
        /// </summary>
        None,
        /// <summary>
        /// General query to fetch all of the connections, security group and variables. Anything in
        /// the Instance class pretty much.
        /// </summary>
        [Description("This describes this instance query business.")]
        InstanceQuery,
        /// <summary>
        /// Sends a signal to the service controller to restart Procon, applying any updates.
        /// </summary>
        InstanceServiceRestart,
        /// <summary>
        /// Sends a signal to the service controller to stop Procon, install/update a package to
        /// the latest version and then start Procon.
        /// </summary>
        InstanceServiceMergePackage,
        /// <summary>
        /// Sends a signal to the service controller to stop Procon, uninstall a package if it is installed
        /// and then start Procon.
        /// </summary>
        InstanceServiceUninstallPackage,
        /// <summary>
        /// Queries information related specificy to a connection. Note that this will essentially query all
        /// other list queries below (NetworkProtocolQuery*) and populate a single request with the results.
        /// </summary>
        ConnectionQuery,

        /// <summary>
        /// Adds a new protocol connection
        /// </summary>
        InstanceAddConnection,
        /// <summary>
        /// Removes a protocol connection
        /// </summary>
        InstanceRemoveConnection,

        /// <summary>
        /// Pulls a localized text from the localization controller.
        /// </summary>
        LanguageLocalize,

        /// <summary>
        /// Enables a plugin, allowing events/commands to be passed through to the plugin
        /// </summary>
        PluginsEnable,
        /// <summary>
        /// Disables  plugin, preventing events/commands being tunneled
        /// </summary>
        PluginsDisable,

        /// <summary>
        /// Queries the permissions of a group
        /// </summary>
        SecurityQueryPermission,

        /// <summary>
        /// Set a group to have streaming permission set, a minimal amount of permissions
        /// to authenticate, get/set variables (flash) and query various sections of Procon.
        /// </summary>
        /// <remarks>All set permissions will be set to 1</remarks>
        SecuritySetPredefinedStreamPermissions,

        /// <summary>
        /// Sets a majority of the permissions an administrator would use. There are numerous commands
        /// that would only be useful for stream accounts, but I'm sure admins will want to have maximo
        /// power for no reson.
        /// </summary>
        /// <remarks>All set permissions will be set to 2</remarks>
        SecuritySetPredefinedAdministratorsPermissions,

        // Groups

        /// <summary>
        /// Adds a new group to the security controller
        /// </summary>
        SecurityAddGroup,
        /// <summary>
        /// Removes a group
        /// </summary>
        SecurityRemoveGroup,
        /// <summary>
        /// Sets a single permission on a group
        /// </summary>
        SecurityGroupSetPermission,
        /// <summary>
        /// Appends a single trait onto a permission
        /// </summary>
        SecurityGroupAppendPermissionTrait,
        /// <summary>
        /// Removes a single trait from a permission
        /// </summary>
        SecurityGroupRemovePermissionTrait,
        /// <summary>
        /// Sets the description of the permission
        /// </summary>
        SecurityGroupSetPermissionDescription,
        /// <summary>
        /// Copies the permissions from one group to another group
        /// </summary>
        SecurityGroupCopyPermissions,

        // Accounts

        /// <summary>
        /// Adds a new account to a group
        /// </summary>
        SecurityGroupAddAccount,
        /// <summary>
        /// Removes an account
        /// </summary>
        SecurityRemoveAccount,

        /// <summary>
        /// Checks if an account can execute a given command
        /// </summary>
        SecurityAccountAuthenticate,
        /// <summary>
        /// Sets a new password for an account, updating the password hash
        /// with the new hashed password.
        /// </summary>
        SecurityAccountSetPassword,
        /// <summary>
        /// Modifies the password hash (literal) for the account
        /// </summary>
        SecurityAccountSetPasswordHash,
        /// <summary>
        /// Sets the default language to use on an account
        /// </summary>
        SecurityAccountSetPreferredLanguageCode,

        // Players

        /// <summary>
        /// Adds a player to an account
        /// </summary>
        SecurityAccountAddPlayer,
        /// <summary>
        /// Removes a player from an account
        /// </summary>
        SecurityRemovePlayer,

        /// <summary>
        /// Queries the database controller, dispatching a query to an open driver.
        /// </summary>
        DatabaseQuery,

        /// <summary>
        /// Finds a match against a registered text command then dispatches the
        /// command for execution.
        /// </summary>
        TextCommandsExecute,
        /// <summary>
        /// Preview a text command match, but do not dispatch for execution.
        /// </summary>
        TextCommandsPreview,
        /// <summary>
        /// Adds a new text command
        /// </summary>
        TextCommandsRegister,
        /// <summary>
        /// Removes a text command
        /// </summary>
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
        
        /// <summary>
        /// Sets a volatile variable that will not be saved to the config.
        /// </summary>
        VariablesSet,
        /// <summary>
        /// Sets a variable which will be saved to a config
        /// </summary>
        VariablesSetA,
        /// <summary>
        /// Sets a variable which will be saved to the config, but only loaded as a volatile variable.
        /// </summary>
        VariablesSetF,
        /// <summary>
        /// Fetches a variable value
        /// </summary>
        VariablesGet,

        /// <summary>
        /// Fetches all events after an event id, provided the events remain in the stream.
        /// </summary>
        EventsFetchAfterEventId,
        /// <summary>
        /// Log a new event, assigning it an id and setting the stamp to the current date/time. 
        /// </summary>
        EventsLog,

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

    }
}
