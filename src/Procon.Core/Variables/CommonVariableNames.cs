namespace Procon.Core.Variables {

    /// <summary>
    /// All variable names are case-insensitive.
    /// </summary>
    public enum CommonVariableNames {
        /// <summary>
        /// (string) The default language to use when a user does not have 
        /// </summary>
        LocalizationDefaultLanguageCode,
        /// <summary>
        /// (string) Prefix for in game commands.  Default is "!"
        /// </summary>
        TextCommandPublicPrefix,
        /// <summary>
        /// (string) Prefix for in game commands.  Default is "#"
        /// </summary>
        TextCommandProtectedPrefix,
        /// <summary>
        /// (string) Prefix for in game commands.  Default is "@"
        /// </summary>
        TextCommandPrivatePrefix,
        /// <summary>
        /// (int) Maximum number of connections to game servers allowed
        /// </summary>
        MaximumGameConnections,
        /// <summary>
        /// (int) The maximum amount of time an event should be held in memory before being flushed to a log.
        /// </summary>
        MaximumEventsTimeSeconds,
        /// <summary>
        /// (bool) True to log events to file, false to just discard the events.
        /// </summary>
        WriteLogEventsToFile,
        /// <summary>
        /// (bool) Flag turning te daemon on/off
        /// </summary>
        DaemonEnabled,
        /// <summary>
        /// (uint) The port to listen for connections on.
        /// </summary>
        DaemonListenerPort,
        /// <summary>
        /// (string) The path to the certificate file. If the file does not exist then the contents of
        /// the variable will be used as the certificate.
        /// </summary>
        DaemonListenerCertificate,
        /// <summary>
        /// List[String] list of strings to ignore if they match
        /// </summary>
        GameEventsIgnoreList,
        /// <summary>
        /// List[String] list of configuration options for the database.
        /// </summary>
        DatabaseConfigGroups,
        /// <summary>
        /// (String) The name of the driver to use.
        /// </summary>
        DatabaseDriverName,
        /// <summary>
        /// (String) The hostname for the database driver to connect to.
        /// </summary>
        DatabaseHostname,
        /// (String) <summary>
        /// The port for the database driver to use.
        /// </summary>
        DatabasePort,
        /// <summary>
        /// (ushort) The database Uid to use by the database driver during authentication.
        /// </summary>
        DatabaseUid,
        /// <summary>
        /// (String) The database Password to use by the database driver during authentication.
        /// </summary>
        DatabasePassword,
        /// <summary>
        /// (String) The maximum number of rows to fetch from a query. This is 20 by default.
        /// </summary>
        DatabaseMaximumSelectedRows,
        /// <summary>
        /// List[String] list of configuration options for the push end points.
        /// </summary>
        EventsPushConfigGroups,
        /// <summary>
        /// (String) The Uri end point to push new events to
        /// </summary>
        EventsPushUri,
        /// <summary>
        /// (int) The interval in seconds to push events to the uri end point.
        /// </summary>
        EventPushIntervalSeconds,
        /// <summary>
        /// (string/PushEventsEndPointContentType) The content type to serialize the events list with. The default (if nothing set) is xml.
        /// </summary>
        EventPushContentType,
        /// <summary>
        /// (string) The key/password to be sent along with request to the push end point. Optional.
        /// </summary>
        EventPushStreamKey,
        /// <summary>
        /// (string) Url to download the procon repository. This repository is automatically
        /// added to procon. While it is also a define in the Procon.Core.dll we use it as
        /// variable that can be set for consistency and unit testing.
        /// </summary>
        PackagesProcon2RepositoryUrl
    }
}
