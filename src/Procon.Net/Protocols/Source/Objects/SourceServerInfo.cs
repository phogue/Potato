using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;

namespace Procon.Net.Protocols.Source.Objects {

    [Serializable]
    public class SourceServerInfo  {

        private static readonly Regex StatusHeaderMatch = new Regex(@"hostname[: ]*(?<hostname>.*)[\r\n]*version[: ]*(?<version>.*)[\r\n]*udp/ip.*[\r\n]*map[: ]*(?<host_map>[a-zA-Z0-9_]*) .*[\r\n]*players[: ]*(?<currentplayers>[0-9]*) \((?<maxplayers>[0-9]*).*[\r\n]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string hostname { get; set; }
        public string version { get; set; }
        public string host_map { get; set; }

        public int currentplayers { get; set; }
        public int maxplayers { get; set; }

        public SourceServerInfo ParseStatusHeader(string text) {

            Match match = SourceServerInfo.StatusHeaderMatch.Match(text);

            if (match.Success == true) {
                this.hostname = match.Groups["hostname"].Value;
                this.version = match.Groups["version"].Value;
                this.host_map = match.Groups["host_map"].Value;
                this.currentplayers = int.Parse(match.Groups["currentplayers"].Value);
                this.maxplayers = int.Parse(match.Groups["maxplayers"].Value);
            }

            return this;
        }

        private void Parse(string key, string value) {
            PropertyInfo property = null;
            if ((property = this.GetType().GetProperty(key)) != null) {
                try {
                    object converted_value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(value);

                    if (converted_value != null) {
                        property.SetValue(this, converted_value, null);
                    }

                }
                catch (Exception) { }
            }
        }

    }
}
