using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Source {
    using Procon.Net.Utils;
    public class SourcePacket : Packet {

        public int? RequestId { get; set; }

        public SourceRequestType RequestType { get; set; }
        public SourceResponseType ResponseType { get; set; }

        public string String1 { get; set; }
        public string String2 { get; set; }

        public List<string> String1Words { get; set; }

        public SourcePacket()
            : base() {
                this.RequestId = 0;

                this.String1 = String.Empty;
                this.String2 = String.Empty;

                this.RequestType = SourceRequestType.None;
                this.ResponseType = SourceResponseType.None;
        }

        public SourcePacket(PacketOrigin origin, bool isResponse, int? sequenceId, SourceRequestType requestType, string string1, string string2)
            : base(origin, isResponse) {
                this.RequestId = sequenceId;
                this.RequestType = requestType;
                this.ResponseType = SourceResponseType.None;
                this.String1 = string1;
                this.String2 = string2;
                this.String1Words = string1.Wordify();
        }

        public override string ToDebugString() {
            return String.Format("[RId: {0}] [ReqT: {1}], [RespT: {2}], [S1: {3}] [S2: {4}]", this.RequestId, this.RequestType, this.ResponseType, this.String1.Trim('\r', '\n'), this.String2.Trim('\r', '\n'));
        }

        public override string ToString() {
            return this.String1;
        }

    }
}
