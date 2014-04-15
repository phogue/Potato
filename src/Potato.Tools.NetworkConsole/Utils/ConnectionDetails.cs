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
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace Potato.Tools.NetworkConsole.Utils {
    public class ConnectionDetails {

        public string GameName { get; set; }
        public string Hostname { get; set; }
        public ushort Port { get; set; }
        public string Password { get; set; }
        public string Additional { get; set; }

        [XmlIgnore, JsonIgnore]
        public bool IsLoaded { get; protected set; }

        public ConnectionDetails() {
            this.GameName = String.Empty;
            this.Hostname = String.Empty;
            this.Password = String.Empty;
            this.Additional = String.Empty;
        }

        public ConnectionDetails Read() {
            try {
                if (File.Exists(Defines.CONFIGS_DIRECTORY_Potato_NET_CONSOLE_XML) == true) {
                    XmlSerializer config = new XmlSerializer(typeof(ConnectionDetails));

                    TextReader input = new StreamReader(Defines.CONFIGS_DIRECTORY_Potato_NET_CONSOLE_XML);
                    ConnectionDetails cd = (ConnectionDetails)config.Deserialize(input);

                    this.GameName = cd.GameName;
                    this.Hostname = cd.Hostname;
                    this.Port = cd.Port;
                    this.Password = cd.Password;
                    this.Additional = cd.Additional;

                    input.Close();

                    this.IsLoaded = true;
                }
            }
            catch (Exception) { }

            return this;
        }

        public ConnectionDetails Write() {

            try {
                if (Directory.Exists(Defines.CONFIGS_DIRECTORY) == false) {
                    Directory.CreateDirectory(Defines.CONFIGS_DIRECTORY);
                }

                XmlSerializer config = new XmlSerializer(typeof(ConnectionDetails));

                TextWriter output = new StreamWriter(Defines.CONFIGS_DIRECTORY_Potato_NET_CONSOLE_XML);

                config.Serialize(output, this);

                output.Flush();
                output.Close();

                this.IsLoaded = true;
            }
            catch (Exception) { }

            return this;
        }
    }
}
