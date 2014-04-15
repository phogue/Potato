using System;
using System.Collections.Generic;

namespace Potato.Net.Protocols.Source {
    using Potato.Net.Protocols.Source.Logging.BroadcastListener;
    using Potato.Net.Protocols.Source.Logging.BroadcastService;

    // todo Use Potato.Net.PacketQueue

    public class SourceClient : Potato.Net.TcpClient {
        protected Dictionary<int?, Packet> SentPackets;
        protected Queue<Packet> QueuePackets;

        protected SourceBroadcastListener BroadcastListener { get; set; }

        public ushort SourceLogServicePort { get; set; }
        public ushort SourceLogListenPort { get; set; }

        protected readonly Object QueueUnqueuePacketLock = new Object();

        public SourceClient(string hostname, ushort port) : base(hostname, port) {
            this.SentPackets = new Dictionary<int?, Packet>();
            this.QueuePackets = new Queue<Packet>();

            this.PacketSerializer = new SourcePacketSerializer();

            this.ConnectionStateChanged += new ConnectionStateChangedHandler(SourceClient_ConnectionStateChanged);
        }

        private void SourceClient_ConnectionStateChanged(IClient sender, ConnectionState newState) {
            if (newState == Net.ConnectionState.ConnectionLoggedIn) {
                // Start the broadcast service. This will work or exit if the port is already bound to.
                new SourceBroadcastService(this.SourceLogServicePort, this.SourceLogListenPort).Connect();

                // Now setup a listener to catch relevent console log packets and dispatch them
                this.BroadcastListener = new SourceBroadcastListener(this.SourceLogListenPort);
                this.BroadcastListener.PacketReceived += new ClientBase.PacketDispatchHandler(BroadcastListener_PacketReceived);
                this.BroadcastListener.Connect();
            }
        }

        private void BroadcastListener_PacketReceived(IClient sender, Packet packet) {

            // TODO: don't compare by string. it's slow.
            if (this.ConnectionState == Net.ConnectionState.ConnectionLoggedIn && System.String.Compare(this.RemoteEndPoint.ToString(), packet.RemoteEndPoint.ToString(), System.StringComparison.Ordinal) == 0) {

                this.OnPacketReceived(packet as SourcePacket);
            }
        }

        private bool QueueUnqueuePacket(bool blSendingPacket, Packet cpPacket, out Packet cpNextPacket) {
            cpNextPacket = null;
            bool blResponse = false;

            lock (this.QueueUnqueuePacketLock) {

                // @todo look at moving the queue from Frostbite to a util location so it may be used here.

                if (blSendingPacket == true) {
                    if (this.SentPackets.Count > 0) {
                        this.QueuePackets.Enqueue(cpPacket);
                        //if (this.PacketQueued != null) {
                        //    FrostbiteConnection.RaiseEvent(this.PacketQueued.GetInvocationList(), this, cpPacket, Thread.CurrentThread.ManagedThreadId);
                        //    //this.PacketQueued(cpPacket, Thread.CurrentThread.ManagedThreadId);
                        //}
                        blResponse = true;
                    }
                    else {
                        if (this.SentPackets.Count == 0 && this.QueuePackets.Count > 0) {
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
                    if (cpPacket != null && cpPacket.RequestId != null) {
                        if (this.SentPackets.ContainsKey(cpPacket.RequestId) == true) {
                            this.SentPackets.Remove(cpPacket.RequestId);
                        }
                    }

                    if (this.QueuePackets.Count > 0) {
                        cpNextPacket = this.QueuePackets.Dequeue();
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

        public SourcePacket GetRequestPacket(SourcePacket recievedPacket) {

            SourcePacket requestPacket = null;

            if (recievedPacket.RequestId != null && this.SentPackets.ContainsKey(recievedPacket.RequestId) == true) {
                requestPacket = this.SentPackets[recievedPacket.RequestId] as SourcePacket;
            }

            return requestPacket;
        }

        protected override void OnPacketReceived(Packet packet) {
            base.OnPacketReceived(packet);

            // Pop the next packet if a packet is waiting to be sent.
            Packet nextPacket = null;
            if (this.QueueUnqueuePacket(false, packet, out nextPacket) == true) {
                this.Send(nextPacket);
            }
        }

        public override void Send(Packet packet) {

            if (packet.RequestId == null) {
                packet.RequestId = this.AcquireSequenceNumber;
            }

            // QueueUnqueuePacket

            if (packet.Origin == PacketOrigin.Server && packet.Type == PacketType.Response) {
                // I don't think this will ever be encountered since OnPacketReceived calls the base.Send.
                base.Send(packet);
            }
            else {
                // Null return because we're not popping a packet, just checking to see if this one needs to be queued.
                Packet nullPacket = null;
                if (this.QueueUnqueuePacket(true, packet, out nullPacket) == false) {
                    // No need to queue, queue is empty.  Send away..
                    base.Send(packet);
                }
            }
        }

        protected override bool BeforePacketSend(Packet packet) {
            SourcePacket sourcePacket = packet as SourcePacket;

            if (sourcePacket != null && sourcePacket.RequestId != null) {
                if (packet.Origin == PacketOrigin.Client && packet.Type == PacketType.Request && this.SentPackets.ContainsKey(sourcePacket.RequestId) == false) {
                    this.SentPackets.Add(sourcePacket.RequestId, sourcePacket);
                }
            }



            return base.BeforePacketSend(packet);
        }


    }
}
