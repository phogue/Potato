using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils;
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
        public static Command DeserializeCommand(CommandServerPacket request) {
            Command command = null;

            try {
                switch (request.Headers[HttpRequestHeader.ContentType].ToLower()) {
                    case Mime.ApplicationJson:
                        command = JsonConvert.DeserializeObject<Command>(request.Content);
                        break;
                    default:
                        command = XDocument.Parse(request.Content).Root.FromXElement<Command>();
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
        public static String ResponseContentType(Command command) {
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
        public static CommandServerPacket CompleteResponsePacket(String contentType, CommandServerPacket response, CommandResult result) {
            switch (contentType) {
                case Mime.TextHtml:
                    response.Headers.Add(HttpRequestHeader.ContentType, Mime.TextHtml);
                    response.Content = result.Now.Content != null ? result.Now.Content.FirstOrDefault() : "";
                    response.StatusCode = HttpStatusCode.OK;
                    break;
                case Mime.ApplicationJson:
                    response.Headers.Add(HttpRequestHeader.ContentType, Mime.ApplicationJson);
                    response.Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings() {
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    response.StatusCode = HttpStatusCode.OK;
                    break;
                default: {
                    // Nothing specified (or xml specified), return xml by default.
                    response.Headers.Add(HttpRequestHeader.ContentType, Mime.ApplicationXml);

                    XDocument document = new XDocument();
                    document.Add(result.ToXElement());

                    // Standalone = yes for now. We should remove all external dtd's
                    // as we're using xml as a way of serialization, but not validating against
                    // anything. Perhaps we should?
                    response.Content = new XDeclaration("1.0", "utf-8", "yes").ToString() + document;
                    response.StatusCode = HttpStatusCode.OK;
                }
                    break;
            }

            return response;
        }
    }
}
