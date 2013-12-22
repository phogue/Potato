using System;
using Procon.Net.Shared;

namespace Procon.Net {
    public class PacketStream : IPacketStream {

        /// <summary>
        /// Buffer for the data currently being read from the stream. This is appended to the received buffer.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Lock used when altering the Data array.
        /// </summary>
        protected readonly object DataLock = new Object();

        public PacketStream() {
            this.Data = new byte[0];
        }

        /// <summary>
        /// Returns the size of the stored Data.
        /// </summary>
        /// <returns>The length of the data array</returns>
        public int Size() {
            lock (this.DataLock) {
                return this.Data.Length;
            }
        }

        /// <summary>
        /// Appends data onto the end of the stream
        /// </summary>
        /// <param name="data">The byte array to append to the stream</param>
        /// <param name="length">The number of bytes to read off the data to append to the stream</param>
        public void Push(byte[] data, int length) {
            if (this.Data != null && data != null) {
                lock (this.DataLock) {
                    Array.Resize(ref this.Data, this.Data.Length + length);

                    Array.Copy(data, 0, this.Data, this.Data.Length - length, length);
                }
            }
        }

        /// <summary>
        /// Removes a specified number of bytes from the stream, returning them in a new byte array.
        /// </summary>
        /// <param name="length">The length of bytes to pull from the start of the stream</param>
        /// <returns>The shifted bytes of length, if available. Null if nothing can be shifted.</returns>
        public byte[] Shift(uint length) {
            byte[] shifted = this.PeekShift(length);

            if (shifted != null) {
                lock (this.DataLock) {
                    // Shift all data from the end of the array to the start.
                    Array.Copy(this.Data, shifted.Length, this.Data, 0, this.Data.Length - shifted.Length);

                    // Now chop off the end
                    Array.Resize(ref this.Data, this.Data.Length - shifted.Length);
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

            if (this.Data != null && this.Data.Length >= length) {
                shifted = new byte[length];

                lock (this.DataLock) {
                    Array.Copy(this.Data, shifted, length);
                }
            }

            return shifted;
        }
    }
}
