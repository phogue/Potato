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
using System.Linq;
using System.Text;
using System.Timers;

namespace Procon.Net.Protocols.CallOfDuty {
    public class CallOfDutyClient : UDPClient<CallOfDutyPacket> {

        private Timer packetRecheck = new Timer(15);

        private CallOfDutyPacket recvPacketBuffer;
        private Queue<CallOfDutyPacket> packetQueue;

        public CallOfDutyClient(string hostname, ushort port)
            : base(hostname, port) {
                this.PacketSerializer = new CallOfDutyPacketSerializer();

                this.packetQueue = new Queue<CallOfDutyPacket>();
                this.recvPacketBuffer = null;

                this.packetRecheck.Elapsed += new ElapsedEventHandler(packetRecheck_Elapsed);
        }

        /// <summary>
        /// This is not a perfect implementation for a udp buffer given
        /// that it relies on the packets being transferred in order.
        /// Very wrong, but at the time of writing and looking at the cod
        /// protocol I cannot see a way of application level ordering
        /// or I lack the experience with UDP (none - first time using it)
        /// to know of another way to reorder the packets without
        /// any sequence ids or timestamps.
        /// </summary>
        /// <param name="packet"></param>
        protected override void OnPacketReceived(CallOfDutyPacket packet) {

            //CallOfDutyPacket codPacket = (CallOfDutyPacket)packet;

            if (packet.IsEOP == true && this.recvPacketBuffer == null) {
                this.packetQueue.Dequeue();

                base.OnPacketReceived(packet);
            }
            else if (packet.IsEOP == true && this.recvPacketBuffer != null) {
                if (this.packetQueue.Count > 0) {
                    this.packetQueue.Dequeue();
                }

                base.OnPacketReceived(recvPacketBuffer.Combine(packet));
                this.recvPacketBuffer = null;
            }
            else if (packet.IsEOP == false && this.recvPacketBuffer == null) {
                this.recvPacketBuffer = packet;
            }
            else if (packet.IsEOP == false && this.recvPacketBuffer != null) {
                this.recvPacketBuffer = this.recvPacketBuffer.Combine(packet);
            }
            // else - the hell happened?
        }

        private void SendDequeued() {

            lock (new Object()) {
                if (this.packetQueue.Count > 0) {
                    CallOfDutyPacket pending = this.packetQueue.Peek();

                    if (pending.SentAt < DateTime.Now.AddMilliseconds(-100)) {
                        if (pending.SentTimes > 10) {
                            // Give up, the packet was doomed.
                            this.packetQueue.Dequeue();
                        }
                        else {
                            // Re-attempt
                            base.Send(pending.Prepare());
                        }
                    }
                }
                else if (this.packetQueue.Count == 0) {
                    this.packetRecheck.Stop();
                }
            }

        }

        private void packetRecheck_Elapsed(object sender, ElapsedEventArgs e) {
            this.SendDequeued();
        }

        public override void AttemptConnection() {
            this.packetRecheck.Start();

            base.AttemptConnection();
        }

        public override void Send(CallOfDutyPacket packet) {

            if (packet != null) {
                this.packetQueue.Enqueue(packet);

                if (this.packetRecheck.Enabled == false) {
                    this.packetRecheck.Start();
                }
            }

            //this.SendDequeued();

            /*
            if (this.packetQueue.Count == 1) {
                // Send straight away

                CallOfDutyPacket pending = this.packetQueue.Peek();
                pending.SentAt = DateTime.Now;
                base.Send(pending);

                this.packetRecheck.Start();
            }
            */
        }

        public override void Shutdown() {

            if (packetRecheck != null) {
                packetRecheck.Stop();
                // packetRecheck.Elapsed -= new ElapsedEventHandler(packetRecheck_Elapsed);
            }

            base.Shutdown();
        }
    }
}
