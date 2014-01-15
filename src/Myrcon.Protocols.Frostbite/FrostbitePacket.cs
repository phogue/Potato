﻿using Procon.Net.Shared;

namespace Myrcon.Protocols.Frostbite {

    public class FrostbitePacket : IPacketWrapper {

        /// <summary>
        /// The underlying simple packet class 
        /// </summary>
        public IPacket Packet { get; set; }

        public static readonly string StringResponseOkay = "OK";

        public FrostbitePacket() {
            this.Packet = new Packet();
        }
    }
}