using System;
using System.Collections.Generic;
using System.Timers;

namespace Potato.Net.Protocols.CallOfDuty {
    public class CallOfDutyClient : Potato.Net.UdpClient {

        private Timer packetRecheck = new Timer(15);

        private CallOfDutyPacket recvPacketBuffer;
        private Queue<CallOfDutyPacket> packetQueue;

        protected readonly Object SendDequeuedLock = new Object();

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
        protected override void OnPacketReceived(Packet packet) {

            CallOfDutyPacket callOfDutyPacket = packet as CallOfDutyPacket;

            if (callOfDutyPacket != null) {
                if (callOfDutyPacket.IsEOP == true && this.recvPacketBuffer == null) {
                    this.packetQueue.Dequeue();

                    base.OnPacketReceived(packet);
                }
                else if (callOfDutyPacket.IsEOP == true && this.recvPacketBuffer != null) {
                    if (this.packetQueue.Count > 0) {
                        this.packetQueue.Dequeue();
                    }

                    base.OnPacketReceived(recvPacketBuffer.Combine(callOfDutyPacket));
                    this.recvPacketBuffer = null;
                }
                else if (callOfDutyPacket.IsEOP == false && this.recvPacketBuffer == null) {
                    this.recvPacketBuffer = callOfDutyPacket;
                }
                else if (callOfDutyPacket.IsEOP == false && this.recvPacketBuffer != null) {
                    this.recvPacketBuffer = this.recvPacketBuffer.Combine(callOfDutyPacket);
                }
                // else - the hell happened?
            }
        }

        private void SendDequeued() {

            lock (this.SendDequeuedLock) {
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

        public override void Connect() {
            this.packetRecheck.Start();

            base.Connect();
        }

        public override void Send(Packet packet) {

            if (packet != null) {
                this.packetQueue.Enqueue(packet as CallOfDutyPacket);

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
