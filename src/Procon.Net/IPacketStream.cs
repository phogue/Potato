namespace Procon.Net {
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
