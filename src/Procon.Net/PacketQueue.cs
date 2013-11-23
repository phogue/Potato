using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net {
    /// <summary>
    /// Handles sending packets synchronously
    /// </summary>
    public class PacketQueue : IPacketQueue {

        /// <summary>
        /// A list of packets currently sent to the server and awaiting a response
        /// </summary>
        public Dictionary<int?, Packet> OutgoingPackets;

        /// <summary>
        /// A queue of packets to send to the server (waiting until the outgoing packets list is clear)
        /// </summary>
        public Queue<Packet> QueuedPackets;

        /// <summary>
        /// Lock for processing new queue items
        /// </summary>
        protected readonly Object QueueUnqueuePacketLock = new Object();

        public PacketQueue() {
            this.OutgoingPackets = new Dictionary<int?, Packet>();
            this.QueuedPackets = new Queue<Packet>();
        }

        /// <summary>
        /// Clears the current queue
        /// </summary>
        public void Clear() {
            this.OutgoingPackets.Clear();
            this.QueuedPackets.Clear();
        }

        /// <summary>
        /// Validates that packets are not 'lost' after being sent. If this is the case then the connection is shutdown
        /// to then be rebooted at a later time.
        /// 
        /// If a packet exists in our outgoing "SentPackets"
        /// </summary>
        public bool RestartConnectionOnQueueFailure() {
            bool failed = false;

            if (this.OutgoingPackets.Any(outgoingPacket => outgoingPacket.Value.Stamp < DateTime.Now.AddMinutes(-2)) == true) {
                this.OutgoingPackets.Clear();
                this.QueuedPackets.Clear();

                failed = true;
            }

            return failed;
        }

        /// <summary>
        /// Fetches the packet that initiated the request.
        /// </summary>
        /// <param name="recievedPacket">The response packet</param>
        /// <returns>The request packet</returns>
        public Packet GetRequestPacket(Packet recievedPacket) {

            Packet requestPacket = null;

            if (recievedPacket.RequestId != null && this.OutgoingPackets.ContainsKey(recievedPacket.RequestId) == true) {
                requestPacket = this.OutgoingPackets[recievedPacket.RequestId];
            }

            return requestPacket;
        }

        /// <summary>
        /// Dequeues the current packet. If a packet is returned then it should be sent
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public Packet PacketReceived(Packet packet) {
            Packet poppedPacket = null;

            lock (this.QueueUnqueuePacketLock) {
                // Pop the next packet if a packet is waiting to be sent.
                if (packet != null && packet.RequestId != null) {
                    if (this.OutgoingPackets.ContainsKey(packet.RequestId) == true) {
                        this.OutgoingPackets.Remove(packet.RequestId);
                    }
                }

                if (this.QueuedPackets.Count > 0) {
                    poppedPacket = this.QueuedPackets.Dequeue();
                }
            }

            return poppedPacket;
        }

        /// <summary>
        /// Enqueues a packet, also pops a packet for sending if a packet is waiting.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public Packet PacketSend(Packet packet) {
            Packet poppedPacket = null;

            lock (this.QueueUnqueuePacketLock) {
                // Pop the next packet if a packet is waiting to be sent.
                if (this.OutgoingPackets.Count > 0) {
                    // Add the packet to our queue to be sent at a later time.
                    this.QueuedPackets.Enqueue(packet);
                }
                else {
                    poppedPacket = packet;
                }
            }

            return poppedPacket;
        }

        /// <summary>
        /// Adds requested originating from the client that do not exist in our outgoing packets
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool BeforePacketSend(Packet packet) {

            if (packet.RequestId != null && packet.Origin == PacketOrigin.Client && packet.Type == PacketType.Request && this.OutgoingPackets.ContainsKey(packet.RequestId) == false) {
                this.OutgoingPackets.Add(packet.RequestId, packet);
            }

            return true;
        }
    }
}
