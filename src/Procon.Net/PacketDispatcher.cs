using System.Collections.Generic;
using System.Linq;

namespace Procon.Net {
    public class PacketDispatcher : IPacketDispatcher {

        /// <summary>
        /// Array of dispatch handlers used to locate an appropriate method to call
        /// once we receieve a packet.
        /// </summary>
        public Dictionary<PacketDispatch, PacketDispatchHandler> Handlers;

        /// <summary>
        /// What method should be called when matched against a packet dispatch object.
        /// </summary>
        /// <param name="request">What was sent to the server or what was just received from the server</param>
        /// <param name="response">What was receieved from the server, or what we should send to the server.</param>
        public delegate void PacketDispatchHandler(IPacketWrapper request, IPacketWrapper response);

        /// <summary>
        /// What method to call when dispatching, but unable to find specific dispatch method.
        /// </summary>
        /// <param name="identifer">What was used to look for a dispatch method</param>
        /// <param name="request">What was sent to the server or what was just received from the server</param>
        /// <param name="response">What was receieved from the server, or what we should send to the server.</param>
        public delegate void MissingPacketDispatchHandler(PacketDispatch identifer, IPacketWrapper request, IPacketWrapper response);

        /// <summary>
        /// Handler to dispatch to if a request fails.
        /// </summary>
        public MissingPacketDispatchHandler MissingDispatchHandler { get; set; }

        public PacketDispatcher() {
            this.Handlers = new Dictionary<PacketDispatch, PacketDispatchHandler>();
        }

        /// <summary>
        /// Appends a dispatch handler, first checking if an existing dispatch exists for this exact
        /// packet. If it exists then it will be overridden.
        /// </summary>
        /// <param name="handlers">A dictionary of handlers to append to the dispatch handlers.</param>
        public void Append(Dictionary<PacketDispatch, PacketDispatchHandler> handlers) {
            foreach (var handler in handlers) {
                if (this.Handlers.ContainsKey(handler.Key) == false) {
                    this.Handlers.Add(handler.Key, handler.Value);
                }
                else {
                    this.Handlers[handler.Key] = handler.Value;
                }
            }
        }

        public virtual void Dispatch(IPacketWrapper packet) {
            
        }

        /// <summary>
        /// Dispatches a recieved packet. Each game implementation needs to supply its own dispatch
        /// method as the protocol may be very different and have additional requirements beyond a 
        /// simple text match.
        /// </summary>
        /// <param name="identifer"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void Dispatch(PacketDispatch identifer, IPacketWrapper request, IPacketWrapper response) {

            var dispatchMethods = this.Handlers.Where(dispatcher => dispatcher.Key.Name == identifer.Name)
                .Where(dispatcher => dispatcher.Key.Origin == PacketOrigin.None || dispatcher.Key.Origin == identifer.Origin)
                .Select(dispatcher => dispatcher.Value)
                .ToList();

            if (dispatchMethods.Any()) {
                foreach (PacketDispatchHandler handler in dispatchMethods) {
                    handler(request, response);
                }
            }
            else {
                this.MissingDispatch(identifer, request, response);
            }
        }

        /// <summary>
        /// Called when dispatching, but no method matches the exact identifier used.
        /// </summary>
        /// <param name="identifer"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void MissingDispatch(PacketDispatch identifer, IPacketWrapper request, IPacketWrapper response) {
            if (this.MissingDispatchHandler != null) {
                this.MissingDispatchHandler(identifer, request, response);
            }
        }
    }
}
