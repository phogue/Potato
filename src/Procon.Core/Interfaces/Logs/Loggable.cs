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
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Procon.Core.Interfaces.Logs {
    using Procon.Core.Utils;
    public class Loggable {

        private static readonly Regex RemoveCaretCodes = new Regex(@"\^[0-9]|\^b|\^i|\^n", RegexOptions.Compiled);

        public delegate void WriteConsoleHandler(DateTime dtLoggedTime, string strLoggedText);

        private FileStream m_file;
        private StreamWriter m_fileWriter;

        public Loggable() {
            this.FileHostNamePort = String.Empty;
            this.LoggingStartedPrefix = "logging started";
            this.LoggingStoppedPrefix = "logging stopped";
            this.FileNameSuffix = String.Empty;

            //this.RemoveCaretCodes 
        }

        protected string FileHostNamePort { get; set; }

        protected string LoggingStartedPrefix { get; set; }

        protected string LoggingStoppedPrefix { get; set; }

        protected string FileNameSuffix { get; set; }

        private bool m_isLogging;
        public bool Logging {
            get {
                return m_isLogging;
            }
            set {

                if (value != this.m_isLogging) {

                    if (value == true) {

                        try {

                            if (Directory.Exists(Path.Combine(Defines.LOGS_DIRECTORY,this.FileHostNamePort)) == false) {
                                Directory.CreateDirectory(Path.Combine(Defines.LOGS_DIRECTORY, this.FileHostNamePort));
                            }

                            if (this.m_file == null) {
                                this.m_isLogging = true;

                                if ((this.m_file = new FileStream(Path.Combine(Path.Combine(Defines.LOGS_DIRECTORY, this.FileHostNamePort), DateTime.Now.ToString("yyyyMMdd") + "_" + this.FileNameSuffix + ".log"), FileMode.Append)) != null) {
                                    if ((this.m_fileWriter = new StreamWriter(this.m_file, Encoding.Unicode)) != null) {

                                        this.WriteLogLine("{0}: {1}", this.LoggingStartedPrefix, DateTime.Now.ToString("dddd, d MMMM yyyy HH:mm:ss"));
                                    }
                                }
                            }
                        }
                        catch (Exception) {
                            this.m_isLogging = false;
                        }
                    }
                    else {
                        if (this.m_fileWriter != null) {

                            this.WriteLogLine("{0}: {1}", this.LoggingStoppedPrefix, DateTime.Now.ToString("dddd, d MMMM yyyy HH:mm:ss"));

                            this.m_fileWriter.Close();
                            this.m_fileWriter.Dispose();
                            this.m_fileWriter = null;
                        }

                        if (this.m_file != null) {
                            this.m_file.Close();
                            this.m_file.Dispose();
                            this.m_file = null;
                        }

                        this.m_isLogging = false;
                    }
                }
            }
        }

        protected void WriteLogLine(string format, params object[] args) {
            if (this.Logging == true && this.m_fileWriter != null) {
                try {
                    this.m_fileWriter.WriteLine(Loggable.RemoveCaretCodes.Replace(String.Format(format, args), ""));
                    this.m_fileWriter.Flush();
                }
                catch (Exception) {
                }
            }
        }
    }
}
