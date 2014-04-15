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
namespace Potato.Net.Shared {
    /// <summary>
    /// A packet stream to pull completed packets from
    /// </summary>
    public interface IPacketStream {
        /// <summary>
        /// Returns the size of the stored Data.
        /// </summary>
        /// <returns>The length of the data array</returns>
        int Size();

        /// <summary>
        /// Appends data onto the end of the stream
        /// </summary>
        /// <param name="data">The byte array to append to the stream</param>
        /// <param name="length">The number of bytes to read off the data to append to the stream</param>
        void Push(byte[] data, int length);

        /// <summary>
        /// Removes a specified number of bytes from the stream, returning them in a new byte array.
        /// </summary>
        /// <param name="length">The length of bytes to pull from the start of the stream</param>
        /// <returns>The shifted bytes of length, if available. Null if nothing can be shifted.</returns>
        byte[] Shift(uint length);

        /// <summary>
        /// Peeks at the start of the stream, but does not remove the data.
        /// </summary>
        /// <param name="length">The number of bytes to peek at</param>
        /// <returns>The peeked data of length, or null if no data exists</returns>
        byte[] PeekShift(uint length);
    }
}
