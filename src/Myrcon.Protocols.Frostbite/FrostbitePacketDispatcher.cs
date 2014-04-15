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
using Potato.Net;
using Potato.Net.Shared;

namespace Myrcon.Protocols.Frostbite {
    public class FrostbitePacketDispatcher : PacketDispatcher {

        /// <summary>
        /// Handles a queue of packets, used to find the request packet.
        /// </summary>
        public IPacketQueue PacketQueue { get; set; }

        public override void Dispatch(IPacketWrapper wrapper) {

            if (wrapper.Packet.Origin == PacketOrigin.Client && wrapper.Packet.Type == PacketType.Response) {
                FrostbitePacket requestPacket = this.PacketQueue.GetRequestPacket(wrapper) as FrostbitePacket;

                // If the request packet is valid and has at least one word.
                if (requestPacket != null && requestPacket.Packet.Words.Count >= 1) {

                    // If the sent command was successful
                    if (wrapper.Packet.Words.Count >= 1 && String.CompareOrdinal(wrapper.Packet.Words[0], FrostbitePacket.StringResponseOkay) == 0) {
                        this.Dispatch(new PacketDispatch() {
                            Name = requestPacket.Packet.Words[0],
                            Origin = requestPacket.Packet.Origin
                        }, requestPacket, wrapper);
                    }
                    else { // The command sent failed for some reason.
                        this.Dispatch(new PacketDispatch() {
                            Name = wrapper.Packet.Words[0],
                            Origin = wrapper.Packet.Origin
                        }, requestPacket, wrapper);
                    }
                }

            }
            else if (wrapper.Packet.Words.Count >= 1 && wrapper.Packet.Origin == PacketOrigin.Server && wrapper.Packet.Type == PacketType.Request) {
                this.Dispatch(new PacketDispatch() {
                    Name = wrapper.Packet.Words[0],
                    Origin = wrapper.Packet.Origin
                }, wrapper, null);
            }
        }
    }
}
