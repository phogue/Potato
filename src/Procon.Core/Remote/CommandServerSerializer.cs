﻿using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Procon.Core.Shared;
using Procon.Net.Shared.Serialization;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Core.Remote {
    /// <summary>
    /// Provides static helpers to serialize requests and responses for the command server. 
    /// </summary>
    public static class CommandServerSerializer {
        /// <summary>
        /// Takes in a request and does it's best to convert it to a command within Procon for execution.
        /// </summary>
        /// <param name="request">The http request for this command</param>
        /// <returns>The deserialized command or null if an error occured during deserialization</returns>
        public static ICommand DeserializeCommand(CommandServerPacket request) {
            ICommand command = null;

            try {
                switch (request.Headers[HttpRequestHeader.ContentType].ToLower()) {
                    default:
                        command = Core.Shared.Serialization.JsonSerialization.Minimal.Deserialize<Command>(request.Content);
                        break;
                }

                if (command != null) {
                    command.Origin = CommandOrigin.Remote;
                    command.RemoteRequest = request;
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
        /// <returns></returns>
        public static String ResponseContentType(ICommand command) {
            String contentType = command.Result != null ? command.Result.ContentType : null;
            
            // If no response type was specified elsewhere
            if (String.IsNullOrEmpty(contentType) == true) {
                // Then assume they want whatever data serialized and returned in whatever the request format was.
                contentType = command.RemoteRequest.Headers[HttpRequestHeader.ContentType] != null ? command.RemoteRequest.Headers[HttpRequestHeader.ContentType].ToLower() : Mime.ApplicationXml;
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
        public static CommandServerPacket CompleteResponsePacket(String contentType, CommandServerPacket response, ICommandResult result) {
            switch (contentType) {
                case Mime.TextHtml:
                    response.Headers.Add(HttpRequestHeader.ContentType, Mime.TextHtml);
                    response.Content = result.Now.Content != null ? result.Now.Content.FirstOrDefault() : "";
                    response.StatusCode = HttpStatusCode.OK;
                    break;
                default:
                    response.Headers.Add(HttpRequestHeader.ContentType, Mime.ApplicationJson);

                    response.Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings() {
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    response.StatusCode = HttpStatusCode.OK;
                    break;
            }

            return response;
        }
    }
}
