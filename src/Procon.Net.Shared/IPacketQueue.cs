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
namespace Procon.Net.Shared {
    /// <summary>
    /// Handles queing of packets, sending packets in a sequential order.
    /// </summary>
    public interface IPacketQueue {
        /// <summary>
        /// Clears the current queue
        /// </summary>
        void Clear();

        /// <summary>
        /// Validates that packets are not 'lost' after being sent. If this is the case then the connection is shutdown
        /// to then be rebooted at a later time.
        /// 
        /// If a packet exists in our outgoing "SentPackets"
        /// </summary>
        bool RestartConnectionOnQueueFailure();

        /// <summary>
        /// Fetches the packet that initiated the request.
        /// </summary>
        /// <param name="recievedPacket">The response packet</param>
        /// <returns>The request packet</returns>
        IPacketWrapper GetRequestPacket(IPacketWrapper recievedPacket);

        /// <summary>
        /// Dequeues the current packet. If a packet is returned then it should be sent
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        IPacketWrapper PacketReceived(IPacketWrapper packet);

        /// <summary>
        /// Enqueues a packet, also pops a packet for sending if a packet is waiting.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        IPacketWrapper PacketSend(IPacketWrapper packet);
    }
}
