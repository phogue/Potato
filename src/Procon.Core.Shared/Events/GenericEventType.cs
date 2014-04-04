using System;

namespace Procon.Core.Shared.Events {

    /// <summary>
    /// Common event names that originate from Procon.Core.
    /// </summary>
    [Serializable]
    public enum GenericEventType {
        /// <summary>
        /// No event specified.
        /// </summary>
        None,

        /// <summary>
        /// A connection has been added.
        /// </summary>
        InstanceConnectionAdded,
        /// <summary>
        /// A connection has been removed.
        /// </summary>
        InstanceConnectionRemoved,
        /// <summary>
        /// The instance has started up.
        /// </summary>
        InstanceServiceStarted,
        /// <summary>
        /// The instance has posted a restart signal.
        /// </summary>
        InstanceServiceRestarting,
        /// <summary>
        /// The instance has posted a merge package signal.
        /// </summary>
        InstanceServiceMergePackage,
        /// <summary>
        /// The instance has posted an uninstall package signal.
        /// </summary>
        InstanceServiceUninstallPackage,

        /// <summary>
        /// A new group has been added.
        /// </summary>
        SecurityGroupAdded,
        /// <summary>
        /// A group has been removed, along with all of the accounts attached to it.
        /// </summary>
        SecurityGroupRemoved,
        /// <summary>
        /// An authority level on a permission attached to a group has been modified.
        /// </summary>
        SecurityGroupPermissionAuthorityChanged,
        /// <summary>
        /// A trait has been appended to a permission.
        /// </summary>
        SecurityGroupPermissionTraitAppended,
        /// <summary>
        /// A trait has been removed from a permission.
        /// </summary>
        SecurityGroupPermissionTraitRemoved,
        /// <summary>
        /// A security group has cloned its permissions from another group.
        /// </summary>
        SecurityGroupPermissionsCopied,
        /// <summary>
        /// An account has been added to a group.
        /// </summary>
        SecurityAccountAdded,
        /// <summary>
        /// An account has been removed (deleted permanently)
        /// </summary>
        SecurityAccountRemoved,
        /// <summary>
        /// A player has been added to an account
        /// </summary>
        SecurityPlayerAdded,
        /// <summary>
        /// A player has been removed from an account
        /// </summary>
        SecurityPlayerRemoved,

        /// <summary>
        /// The command server has started up, listening for incoming connections
        /// </summary>
        CommandServerStarted,
        /// <summary>
        /// The command server has been stopped and is no longer listening for incoming connections
        /// </summary>
        CommandServerStopped,

        /// <summary>
        /// The repository controller has rebuilt what it knows about the packages (local, remote, updated etc.)
        /// </summary>
        PackagesCacheRebuilt,

        /// <summary>
        /// A variable has been set
        /// </summary>
        VariablesSet,
        /// <summary>
        /// A variable has been set and will be archived.
        /// </summary>
        VariablesSetA,
        /// <summary>
        /// A variable has been set and will be saved to the config, but will be volatile thereafter.
        /// </summary>
        VariablesSetF,

        /// <summary>
        /// A text command has been registered with the text controller
        /// </summary>
        TextCommandRegistered,
        /// <summary>
        /// A text command has been removed from the text controller.
        /// </summary>
        TextCommandUnregistered,
        /// <summary>
        /// A text command has been executed (matched & dispatched to plugin)
        /// </summary>
        TextCommandExecuted,
        /// <summary>
        /// A text command has been previewed (matched, but not executed)
        /// </summary>
        TextCommandPreviewed,

        /// <summary>
        /// A plugin has been enabled
        /// </summary>
        PluginsEnabled,
        /// <summary>
        /// A plugin has been disabled.
        /// </summary>
        PluginsDisabled,
        /// <summary>
        /// A plugin has completed it's load.
        /// </summary>
        PluginsLoaded,
        /// <summary>
        /// A plugin is being unloaded. Think of this as a destructor for the plugin.
        /// </summary>
        PluginsUnloading,
        
        /// <summary>
        /// The config for is being loaded
        /// </summary>
        ConfigLoading,
        /// <summary>
        /// The config for has been loaded
        /// </summary>
        ConfigLoaded
    }
}