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

namespace Potato.Net.Shared {
    public class PacketStream : IPacketStream {

        /// <summary>
        /// Buffer for the data currently being read from the stream. This is appended to the received buffer.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Lock used when altering the Data array.
        /// </summary>
        protected readonly object DataLock = new object();

        public PacketStream() {
            Data = new byte[0];
        }

        /// <summary>
        /// Returns the size of the stored Data.
        /// </summary>
        /// <returns>The length of the data array</returns>
        public int Size() {
            lock (DataLock) {
                return Data.Length;
            }
        }

        /// <summary>
        /// Appends data onto the end of the stream
        /// </summary>
        /// <param name="data">The byte array to append to the stream</param>
        /// <param name="length">The number of bytes to read off the data to append to the stream</param>
        public void Push(byte[] data, int length) {
            if (Data != null && data != null) {
                lock (DataLock) {
                    Array.Resize(ref Data, Data.Length + length);

                    Array.Copy(data, 0, Data, Data.Length - length, length);
                }
            }
        }

        /// <summary>
        /// Removes a specified number of bytes from the stream, returning them in a new byte array.
        /// </summary>
        /// <param name="length">The length of bytes to pull from the start of the stream</param>
        /// <returns>The shifted bytes of length, if available. Null if nothing can be shifted.</returns>
        public byte[] Shift(uint length) {
            var shifted = PeekShift(length);

            if (shifted != null) {
                lock (DataLock) {
                    // Shift all data from the end of the array to the start.
                    Array.Copy(Data, shifted.Length, Data, 0, Data.Length - shifted.Length);

                    // Now chop off the end
                    Array.Resize(ref Data, Data.Length - shifted.Length);
                }
            }

            return shifted;
        }

        /// <summary>
        /// Peeks at the start of the stream, but does not remove the data.
        /// </summary>
        /// <param name="length">The number of bytes to peek at</param>
        /// <returns>The peeked data of length, or null if no data exists</returns>
        public byte[] PeekShift(uint length) {
            byte[] shifted = null;

            if (Data != null && Data.Length >= length) {
                shifted = new byte[length];

                lock (DataLock) {
                    Array.Copy(Data, shifted, length);
                }
            }

            return shifted;
        }
    }
}
