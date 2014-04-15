using System;

namespace Potato.Net.Protocols.CallOfDuty {
    public class CallOfDutyPacket : Packet {

        public DateTime SentAt { get; set; }
        public int SentTimes { get; set; }

        public string Command { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }

        // Is End Of Packet (has \n\n at the end)
        public bool IsEOP { get; set; }

        public CallOfDutyPacket()
            : base() {

        }

        public CallOfDutyPacket(PacketOrigin origin, PacketType type, string password, string message)
            : base(origin, type) {

                this.Password = password;
                this.Message = message;
        }

        public CallOfDutyPacket Prepare() {
            this.SentAt = DateTime.Now;
            this.SentTimes++;

            return this;
        }

        public CallOfDutyPacket Combine(CallOfDutyPacket b) {

            if (String.Compare(this.Command, b.Command, true) == 0) {
                this.Message = this.Message + b.Message;
                this.IsEOP = b.IsEOP;
            }

            return this;
        }

        public override string ToString() {
            return this.Message;
        }

    }
}
