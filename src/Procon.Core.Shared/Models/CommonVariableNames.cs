#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;

namespace Procon.Core.Shared.Models {

    /// <summary>
    /// All variable names are case-insensitive.
    /// </summary>
    [Serializable]
    public enum CommonVariableNames {
        /// <summary>
        /// (string) The password for encrypting/descrypting configs
        /// </summary>
        InstanceConfigPassword,
        /// <summary>
        /// (int) The maximum number of tokens allowed per account.
        /// </summary>
        /// <remarks>Default used is 5 if not set.</remarks>
        SecurityMaximumAccessTokensPerAccount,
        /// <summary>
        /// (int) The maximum amount of time a token can go without being touched before it is considered expired.
        /// </summary>
        /// <remarks>Default used is 172800 (2 days) if not set.</remarks>
        SecurityMaximumAccessTokenLastTouchedLengthSeconds,
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
        /// (String) The uri of the respository in the group
        /// </summary>
        PackagesRepositoryUri,
        /// <summary>
        /// (String) The full default uri of the source respository to download core procon updates from.
        /// </summary>
        PackagesDefaultSourceRepositoryUri
    }
}
