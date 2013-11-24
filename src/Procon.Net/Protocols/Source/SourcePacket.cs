using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Source {
    using Procon.Net.Utils;
    public class SourcePacket : Packet {

        public SourceRequestType RequestType { get; set; }
        public SourceResponseType ResponseType { get; set; }

        public string String1 { get; set; }
        public string String2 { get; set; }

        public SourcePacket()
            : base() {
                this.RequestId = 0;

                this.String1 = String.Empty;
                this.String2 = String.Empty;

                this.RequestType = SourceRequestType.None;
                this.ResponseType = SourceResponseType.None;
        }

        public SourcePacket(PacketOrigin origin, PacketType type, int? sequenceId, SourceRequestType requestType, string string1, string string2)
            : base(origin, type) {
                this.RequestId = sequenceId;
                this.RequestType = requestType;
                this.ResponseType = SourceResponseType.None;
                this.String1 = string1;
                this.String2 = string2;
                this.Words = string1.Wordify();
        }

        public override string ToDebugString() {
            return String.Format("[RId: {0}] [ReqT: {1}], [RespT: {2}], [S1: {3}] [S2: {4}]", this.RequestId, this.RequestType, this.ResponseType, this.String1.Trim('\r', '\n'), this.String2.Trim('\r', '\n'));
        }

        public override string ToString() {
            return this.String1;
        }

    }
}
