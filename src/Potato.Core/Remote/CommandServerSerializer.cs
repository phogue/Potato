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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Potato.Core.Shared;
using Potato.Core.Shared.Remote;
using Potato.Net.Protocols.CommandServer;
using Potato.Net.Shared;
using Potato.Net.Shared.Serialization;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Remote {
    /// <summary>
    /// Provides static helpers to serialize requests and responses for the command server. 
    /// </summary>
    public static class CommandServerSerializer {
        /// <summary>
        /// Takes in a request and does it's best to convert it to a command within Potato for execution.
        /// </summary>
        /// <param name="request">The http request for this command</param>
        /// <returns>The deserialized command or null if an error occured during deserialization</returns>
        public static ICommand DeserializeCommand(CommandServerPacket request) {
            ICommand command = null;

            try {
                if (string.IsNullOrEmpty(request.Content)) {
                    command = new Command() {
                        Name = request.Request.LocalPath
                    };
                }
                else {
                    command = Core.Shared.Serialization.JsonSerialization.Minimal.Deserialize<Command>(request.Content);
                }

                if (command != null) {
                    command.Origin = CommandOrigin.Remote;

                    var commandRequest = new HttpCommandRequest() {
                        Content = new List<string>() {
                            request.Header,
                            request.Content
                        },
                        Packets = new List<IPacket>() {
                            request.Packet
                        }
                    };

                    // If no valid authentication exists after deserialization but we have http authorization
                    if (command.Authentication.Valid() == false && request.Authorization.Length == 2) {
                        command.Authentication.Username = request.Authorization.First();
                        command.Authentication.PasswordPlainText = request.Authorization.Last();
                    }

                    commandRequest.AppendTags(request.Headers);
                    commandRequest.AppendTags(request.Query);

                    command.Request = commandRequest;
                }
            }
            catch {
                command = null;
            }

            return command;
        }

        /// <summary>
        /// Extracts the content type from the command and it's result.
        /// </summary>
        /// <param name="command">The command that has been dispatched for execution</param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ResponseContentType(ICommand command, CommandServerPacket request) {
            var contentType = command.Result != null ? command.Result.ContentType : null;
            
            // If no response type was specified elsewhere
            if (string.IsNullOrEmpty(contentType) == true) {
                // Then assume they want whatever data serialized and returned in whatever the request format was.
                contentType = command.Request.Tags.ContainsKey(HttpRequestHeader.ContentType.ToString()) == true ? command.Request.Tags[HttpRequestHeader.ContentType.ToString()].ToLower() : Mime.DefaultMimeTypeGivenMethod(request.Method);
            }

            return contentType;
        }

        /// <summary>
        /// Serializes the result of the issued command back into the http response for the user.
        /// </summary>
        /// <param name="contentType">The content type to serialize the response to</param>
        /// <param name="response">The existing response packet to be modified with additional data/changes</param>
        /// <param name="result">The result of the command issued in the request</param>
        /// <returns>The existing response packet, modified with the result of the command execution.</returns>
        public static CommandServerPacket CompleteResponsePacket(string contentType, CommandServerPacket response, ICommandResult result) {
            switch (contentType) {
                case Mime.ApplicationJavascript:
                case Mime.TextCss:
                case Mime.TextHtml:
                    response.Headers.Add(HttpResponseHeader.ContentType, contentType);
                    response.Content = result.Now.Content != null ? result.Now.Content.FirstOrDefault() : "";
                    response.StatusCode = HttpStatusCode.OK;
                    break;
                default:
                    response.Headers.Add(HttpResponseHeader.ContentType, Mime.ApplicationJson);

                    using (var writer = new StringWriter()) {
                        Core.Shared.Serialization.JsonSerialization.Minimal.Serialize(writer, result);

                        response.Content = writer.ToString();
                    }

                    response.StatusCode = HttpStatusCode.OK;
                    break;
            }

            return response;
        }
    }
}
