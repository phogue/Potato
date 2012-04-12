// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Procon.Net.Console.Utils {
    public class ConnectionDetails {

        public string GameName { get; set; }
        public string Hostname { get; set; }
        public ushort Port { get; set; }
        public string Password { get; set; }
        public string Additional { get; set; }

        [XmlIgnore]
        public bool IsLoaded { get; protected set; }

        public ConnectionDetails() {
            this.GameName = String.Empty;
            this.Hostname = String.Empty;
            this.Password = String.Empty;
            this.Additional = String.Empty;
        }

        public ConnectionDetails Read() {
            try {
                if (File.Exists(Defines.CONFIGS_DIRECTORY_PROCON_NET_CONSOLE_XML) == true) {
                    XmlSerializer config = new XmlSerializer(typeof(ConnectionDetails));

                    TextReader input = new StreamReader(Defines.CONFIGS_DIRECTORY_PROCON_NET_CONSOLE_XML);
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

                TextWriter output = new StreamWriter(Defines.CONFIGS_DIRECTORY_PROCON_NET_CONSOLE_XML);

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
