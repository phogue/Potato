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

using System.Linq;
using NUnit.Framework;

namespace Potato.Net.Shared.Test {
    [TestFixture]
    public class PacketStreamTest {

        /// <summary>
        /// Tests that data can be pushed onto the end of the packet stream, if the packet stream is uninitialized.
        /// </summary>
        [Test]
        public void TestEmptyPacketStreamPush() {
            var stream = new PacketStream();

            stream.Push(new byte[] { 0x01 }, 1);

            Assert.AreEqual(1, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
        }

        /// <summary>
        /// Tests that a size is returned of 0, even on an uninitialized stream.
        /// </summary>
        [Test]
        public void TestEmptyPacketStreamSizeZero() {
            var stream = new PacketStream();

            Assert.AreEqual(0, stream.Size());
        }

        /// <summary>
        /// Tests that a size can be obtained from an initialized stream.
        /// </summary>
        [Test]
        public void TestEmptyPacketStreamSizeOne() {
            var stream = new PacketStream();

            stream.Push(new byte[] { 0x01 }, 1);

            Assert.AreEqual(1, stream.Size());
        }

        /// <summary>
        /// Test that pushing an empty array of data onto an empty set results in no exception, but remains an empty stream.
        /// </summary>
        [Test]
        public void TestEmptyPacketStreamPushEmptyData() {
            var stream = new PacketStream();

            stream.Push(new byte[0], 0);

            Assert.AreEqual(0, stream.Data.Length);
        }

        /// <summary>
        /// Tests that pushing a null data array onto the packet stream results in no changes and no exceptions.
        /// </summary>
        [Test]
        public void TestEmptyPacketStreamPushNullData() {
            var stream = new PacketStream();

            stream.Push(null, 0);

            Assert.AreEqual(0, stream.Data.Length);
        }

        /// <summary>
        /// Tests that single data can be appended to the end of an existing stream.
        /// </summary>
        [Test]
        public void TestSingleAppendPacketStreamPush() {
            var stream = new PacketStream() {
                Data = new byte[] {0x01}
            };

            stream.Push(new byte[] { 0x02 }, 1);

            Assert.AreEqual(2, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
            Assert.AreEqual(0x02, stream.Data[1]);
        }

        /// <summary>
        /// Tests that an empty data array can be appended to the end of an existing stream, resulting in no change to the stream
        /// with no exceptions posted.
        /// </summary>
        [Test]
        public void TestSingleAppendPacketStreamPushEmptyData() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01 }
            };

            stream.Push(new byte[0], 0);

            Assert.AreEqual(1, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
        }

        /// <summary>
        /// Tests that pushing a null data array onto an established stream results in no changes and no exceptions.
        /// </summary>
        [Test]
        public void TestSingleAppendPacketStreamPushNullData() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01 }
            };

            stream.Push(null, 0);

            Assert.AreEqual(1, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
        }

        /// <summary>
        /// Tests that multiple pushes can be done in a row, with multiple bytes.
        /// </summary>
        [Test]
        public void TestMultipleAppendPacketStreamPush() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01 }
            };

            stream.Push(new byte[] { 0x02 }, 1);

            stream.Push(new byte[] { 0x03, 0x04 }, 2);

            Assert.AreEqual(4, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
            Assert.AreEqual(0x02, stream.Data[1]);
            Assert.AreEqual(0x03, stream.Data[2]);
            Assert.AreEqual(0x04, stream.Data[3]);
        }

        /// <summary>
        /// Tests a single byte can be pulled from the packet stream, without altering the stream.
        /// </summary>
        [Test]
        public void TestSinglePacketStreamPeekShift() {
            var stream = new PacketStream() {
                Data = new byte[] {0x01, 0x02, 0x03, 0x04}
            };

            Assert.AreEqual(0x01, stream.PeekShift(1).First());
            Assert.AreEqual(4, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
            Assert.AreEqual(0x02, stream.Data[1]);
            Assert.AreEqual(0x03, stream.Data[2]);
            Assert.AreEqual(0x04, stream.Data[3]);
        }

        /// <summary>
        /// Tests pulling multiple bytes off the stream for a peek will not alter the stream.
        /// </summary>
        [Test]
        public void TestMultiplePacketStreamPeekShift() {
            var stream = new PacketStream() {
                Data = new byte[] {0x01, 0x02, 0x03, 0x04}
            };

            Assert.AreEqual(0x01, stream.PeekShift(2).First());
            Assert.AreEqual(0x02, stream.PeekShift(2).Last());
            Assert.AreEqual(4, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
            Assert.AreEqual(0x02, stream.Data[1]);
            Assert.AreEqual(0x03, stream.Data[2]);
            Assert.AreEqual(0x04, stream.Data[3]);
        }

        /// <summary>
        /// Tests that peeking at no bytes returns an empty array with no changes to the packet stream.
        /// </summary>
        [Test]
        public void TestMultiplePacketStreamPeekShiftZeroBytes() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            Assert.AreEqual(0, stream.PeekShift(0).Length);
            Assert.AreEqual(4, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
            Assert.AreEqual(0x02, stream.Data[1]);
            Assert.AreEqual(0x03, stream.Data[2]);
            Assert.AreEqual(0x04, stream.Data[3]);
        }

        /// <summary>
        /// Tests that a new packet stream with no data initialized will return null when asked for any data.
        /// </summary>
        [Test]
        public void TestUninitializedPacketStreamPeekShift() {
            var stream = new PacketStream();

            Assert.IsNull(stream.PeekShift(1));
        }

        /// <summary>
        /// Tests that requesting a peek at more data than is available will return null
        /// </summary>
        [Test]
        public void TestIndexBoundsPacketStreamPeekShift() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            Assert.IsNull(stream.PeekShift(5));
        }

        /// <summary>
        /// Tests a single byte can be pulled from the packet stream, removing it from the start of the stream
        /// </summary>
        [Test]
        public void TestSinglePacketStreamShift() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            var data = stream.Shift(1);

            Assert.AreEqual(0x01, data.First());
            Assert.AreEqual(3, stream.Data.Length);
            Assert.AreEqual(0x02, stream.Data[0]);
            Assert.AreEqual(0x03, stream.Data[1]);
            Assert.AreEqual(0x04, stream.Data[2]);
        }

        /// <summary>
        /// Tests pulling multiple bytes off the stream will remove them from the stream
        /// </summary>
        [Test]
        public void TestMultiplePacketStreamShift() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            var data = stream.Shift(2);

            Assert.AreEqual(0x01, data.First());
            Assert.AreEqual(0x02, data.Last());
            Assert.AreEqual(2, stream.Data.Length);
            Assert.AreEqual(0x03, stream.Data[0]);
            Assert.AreEqual(0x04, stream.Data[1]);
        }

        /// <summary>
        /// Tests that all data can be moved off the start of the array, resulting in an empty Data array.
        /// </summary>
        [Test]
        public void TestAllDataPacketStreamShift() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            var data = stream.Shift(4);

            Assert.AreEqual(0x01, data[0]);
            Assert.AreEqual(0x02, data[1]);
            Assert.AreEqual(0x03, data[2]);
            Assert.AreEqual(0x04, data[3]);
            Assert.AreEqual(0, stream.Data.Length);
        }

        /// <summary>
        /// Tests that a new packet stream with no data initialized will return null when asked for any data.
        /// </summary>
        [Test]
        public void TestUninitializedPacketStreamShift() {
            var stream = new PacketStream();

            Assert.IsNull(stream.Shift(1));
        }

        /// <summary>
        /// Tests that shifting no bytes returns an empty array with no changes to the packet stream.
        /// </summary>
        [Test]
        public void TestMultiplePacketStreamShiftZeroBytes() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            Assert.AreEqual(0, stream.Shift(0).Length);
            Assert.AreEqual(4, stream.Data.Length);
            Assert.AreEqual(0x01, stream.Data[0]);
            Assert.AreEqual(0x02, stream.Data[1]);
            Assert.AreEqual(0x03, stream.Data[2]);
            Assert.AreEqual(0x04, stream.Data[3]);
        }

        /// <summary>
        /// Tests that requesting a peek at more data than is available will return null
        /// </summary>
        [Test]
        public void TestIndexBoundsPacketStreamShift() {
            var stream = new PacketStream() {
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            Assert.IsNull(stream.Shift(5));
        }
    }
}
