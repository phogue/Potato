using System;

namespace Procon.Core.Shared.Models {

    /// <summary>
    /// All variable names are case-insensitive.
    /// </summary>
    [Serializable]
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
        MaximumProtocolConnections,
        /// <summary>
        /// (int) The maximum amount of time an event should be held in memory before being flushed to a log.
        /// </summary>
        MaximumEventsTimeSeconds,
        /// <summary>
        /// (bool) True to log events to file, false to just discard the events.
        /// </summary>
        WriteLogEventsToFile,
        /// <summary>
        /// (bool) Flag turning te command sever on/off
        /// </summary>
        CommandServerEnabled,
        /// <summary>
        /// (uint) The port to listen for connections on.
        /// </summary>
        CommandServerPort,
        /// <summary>
        /// (string) The path to the certificate file. If unset then Procon's default /Certificates/CommandServer.pfx
        /// will be used.
        /// </summary>
        CommandServerCertificatePath,
        /// <summary>
        /// (string) The password to the certificate store
        /// </summary>
        CommandServerCertificatePassword,
        /// <summary>
        /// List[String] list of strings to ignore if they match
        /// </summary>
        ProtocolEventsIgnoreList,
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
        /// (String) The name of the database to use once connected.
        /// </summary>
        DatabaseName,
        /// <summary>
        /// (Bool) True if the database should exist in memory only
        /// </summary>
        DatabaseMemory,
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
        /// List[String] A list of ignored event names that won't be logged to the file.
        /// </summary>
        EventsLogIgnoredNames,
        /// <summary>
        /// List[String] list of configuration options for the packages.
        /// </summary>
        PackagesConfigGroups,
        /// <summary>
        /// The full uri of the respository
        /// </summary>
        PackagesRepositoryUri
    }
}
