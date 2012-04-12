// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;

namespace Procon.Net.Protocols.Frostbite {
    public class FrostbiteClient : TCPClient<FrostbitePacket> {

        protected Dictionary<UInt32?, FrostbitePacket> m_sentPackets;
        protected Queue<FrostbitePacket> m_quePackets;

        public FrostbiteClient(string hostname, ushort port)
            : base(hostname, port) {
                this.PacketSerializer = new FrostbitePacketSerializer();
        }

        protected override void ClearConnection() {
            base.ClearConnection();

            this.m_sentPackets = new Dictionary<UInt32?, FrostbitePacket>();
            this.m_quePackets = new Queue<FrostbitePacket>();
        }

        private bool QueueUnqueuePacket(bool blSendingPacket, FrostbitePacket cpPacket, out FrostbitePacket cpNextPacket) {
            cpNextPacket = null;
            bool blResponse = false;

            lock (new object()) {

                if (blSendingPacket == true) {
                    if (this.m_sentPackets.Count > 0) {
                        this.m_quePackets.Enqueue(cpPacket);
                        //if (this.PacketQueued != null) {
                        //    FrostbiteConnection.RaiseEvent(this.PacketQueued.GetInvocationList(), this, cpPacket, Thread.CurrentThread.ManagedThreadId);
                        //    //this.PacketQueued(cpPacket, Thread.CurrentThread.ManagedThreadId);
                        //}
                        blResponse = true;
                    }
                    else {
                        if (this.m_sentPackets.Count == 0 && this.m_quePackets.Count > 0) {
                            // TODO: I've seen it slip in here once, but that was when I had
                            // combined the events and commands streams.  Have not seen it since, but need to make sure.

                            //throw new Exception();
                        }
                        else {
                            // No packets waiting for response, free to send the new packet.
                            blResponse = false;
                        }
                    }
                }
                else {
                    // Else it's being called from recv and cpPacket holds the processed RequestPacket.

                    // Remove the packet 
                    if (cpPacket != null) {
                        if (this.m_sentPackets.ContainsKey(cpPacket.SequenceId) == true) {
                            this.m_sentPackets.Remove(cpPacket.SequenceId);
                        }
                    }

                    if (this.m_quePackets.Count > 0) {
                        cpNextPacket = this.m_quePackets.Dequeue();
                        //if (this.PacketDequeued != null) {
                        //    FrostbiteConnection.RaiseEvent(this.PacketDequeued.GetInvocationList(), this, cpNextPacket, Thread.CurrentThread.ManagedThreadId);
                        //    //this.PacketDequeued(cpNextPacket, Thread.CurrentThread.ManagedThreadId);
                        //}
                        blResponse = true;
                    }
                    else {
                        blResponse = false;
                    }

                }

                return blResponse;
            }
        }

        /*
        protected override FrostbitePacket CreatePacket(byte[] packet) {
            return new FrostbitePacket(packet);
        }

        protected override UInt32 DecodePacketSize(byte[] packet) {
            return FrostbitePacket.DecodePacketSize(packet);
        }

        protected override UInt32 GetPacketHeaderSize() {
            return FrostbitePacket.INT_PACKET_HEADER_SIZE;
        }
        */
        public FrostbitePacket GetRequestPacket(FrostbitePacket recievedPacket) {

            FrostbitePacket requestPacket = null;

            if (this.m_sentPackets.ContainsKey(recievedPacket.SequenceId) == true) {
                requestPacket = this.m_sentPackets[recievedPacket.SequenceId];
            }

            return requestPacket;
        }

        protected override void OnPacketReceived(FrostbitePacket packet) {
            base.OnPacketReceived(packet);

            // Respond with "OK" to all server events.
            if (packet.Origin == PacketOrigin.Server && packet.IsResponse == false) {
                base.Send(new FrostbitePacket(PacketOrigin.Server, true, packet.SequenceId, FrostbitePacket.STRING_RESPONSE_OKAY));
            }

            // Pop the next packet if a packet is waiting to be sent.
            FrostbitePacket nextPacket = null;
            if (this.QueueUnqueuePacket(false, packet, out nextPacket) == true) {
                this.Send(nextPacket);
            }
        }

        public override void Send(FrostbitePacket packet) {

            if (packet.SequenceId == null) {
                packet.SequenceId = this.AcquireSequenceNumber;
            }

            // QueueUnqueuePacket
            FrostbitePacket nullPacket = null;

            if (packet.Origin == PacketOrigin.Server && packet.IsResponse == true) {
                // I don't think this will ever be encountered since OnPacketReceived calls the base.Send.
                base.Send(packet);
            }
            else {
                // Null return because we're not popping a packet, just checking to see if this one needs to be queued.
                if (this.QueueUnqueuePacket(true, (FrostbitePacket)packet, out nullPacket) == false) {
                    // No need to queue, queue is empty.  Send away..
                    base.Send(packet);
                }
            }
        }

        protected override void OnBeforePacketSend(FrostbitePacket packet, out bool isProcessed) {

            if (packet.Origin == PacketOrigin.Client && packet.IsResponse == false && this.m_sentPackets.ContainsKey(packet.SequenceId) == false) {
                this.m_sentPackets.Add(packet.SequenceId, packet);
            }

            base.OnBeforePacketSend(packet, out isProcessed);
        }

    }
}
