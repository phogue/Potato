using System.Text;

namespace Procon.Net.Protocols.CallOfDuty.BlackOps {
    using Procon.Net.Attributes;
    using Procon.Net.Utils.HTTP;

    //[GameType(Type = CommonGameType.COD_BO, Name = "Call of Duty: Blackops", Provider = "Myrcon")]
    public class CallOfDutyBlackOpsGame : CallOfDutyGame {

        public CallOfDutyBlackOpsGame(string hostName, ushort port) : base(hostName, port) {
            
        }

        protected override void Execute(string hostName, ushort port) {
            base.Execute(hostName, port);

            Request timeout = new Request("http://logs.gameservers.com/timeout");
            timeout.RequestComplete += new Request.RequestEventDelegate(timeout_RequestComplete);
            timeout.BeginRequest();
        }

        private void timeout_RequestComplete(Request sender) {
            string data = Encoding.ASCII.GetString(sender.CompleteFileData);

            short timeout = 0;

            if (short.TryParse(data, out timeout) == true) {
                this.LogFile.Interval = timeout;
            }
        }
    }
}
