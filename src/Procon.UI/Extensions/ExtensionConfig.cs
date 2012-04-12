// Copyright 2011 Cameron 'Imisnew2' Gunnin
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
using System.IO;
using System.Xml.Linq;

namespace Procon.UI.Extensions
{
    /// <summary>
    /// Handles I/O related to a Procon Extension Config file.
    /// </summary>
    public class ExtensionConfig
    {
        #region Properties

        /// <summary>
        /// File information about the config that was read.
        /// </summary>
        public FileInfo  ConfigFile { get; protected set; }
        /// <summary>
        /// Document information about the config that was read.
        /// </summary>
        public XDocument ConfigNode { get; protected set; }

        #endregion Properties

        /// <summary>
        /// Reads in a Procon Extension Config file.  Sets the respective properties if successful.
        /// </summary>
        /// <param name="config">The configuration file.</param>
        public void ReadConfig(String config)
        {
            // Reset Document Information.
            ConfigNode = null;

            // Get Configuration Information.
            try               { ConfigFile = new FileInfo(config); }
            catch (Exception) { ConfigFile = null; }

            // Bad File.
            if (ConfigFile == null || !ConfigFile.Exists) return;

            // Get Document Information.
            try               { ConfigNode = XDocument.Load(ConfigFile.FullName); }
            catch (Exception) { ConfigNode = null; }
        }

        /// <summary>
        /// Writes out a Procon Extension Config file using the specified information.
        /// </summary>
        /// <param name="config">The configuration file.</param>
        /// <param name="assemblies">The assemblies to write out.</param>
        /// <param name="extensions">The extensions to write out.</param>
        public static void WriteConfig(String config, IEnumerable<String> assemblies, IEnumerable<String> extensions)
        {
            // Get Configuration Information.
            FileInfo            configFile;
            try               { configFile = new FileInfo(config); }
            catch (Exception) { configFile = null; }

            // Bad File.
            if (configFile == null) return;

            // Prepare Config File.
            XElement configNode = new XElement("Config");
            if (assemblies != null)
            {
                // Assemblies Node.
                XElement assembliesNode = new XElement("Assemblies");
                foreach (String assembly in assemblies)
                    assembliesNode.Add(new XElement("Assembly", assembly));
                configNode.Add(assembliesNode);
            }
            if (extensions != null)
            {
                // Extensions Node.
                XElement extensionsNode = new XElement("Extensions");
                foreach (String extension in extensions)
                    extensionsNode.Add(new XElement("Extension", extension));
                configNode.Add(extensionsNode);
            }
            File.WriteAllText(configFile.FullName, configNode.ToString());
        }

        /// <summary>
        /// Gets a list of paths to assemblies as extrapolated from the Procon Extension Config
        /// file.  Searches the config file for a list of "Assemblies" tags, then iterates through
        /// each one's "Assembly" tags to populate the list.
        /// </summary>
        public List<String> GetAssemblies()
        {
            // Bad File.
            if (ConfigNode == null) return new List<String>();

            // Get Assemblies from Config.
            List<String> assembliesInfo = new List<String>();
            foreach (XElement assembliesNode in ConfigNode.Descendants("Assemblies"))
                foreach (XElement assemblyNode in assembliesNode.Descendants("Assembly"))
                    assembliesInfo.Add(assemblyNode.Value);

            // Return Assemblies we found.
            return assembliesInfo;
        }

        /// <summary>
        /// Gets a list of names of extensions as extrapolated from the Procon Extension Config
        /// file.  Searches the config file for a list of "Extensions" tags, then iterates through
        /// each one's "Extension" tags to populate the list.
        /// </summary>
        public List<String> GetExtensions()
        {
            // Bad File.
            if (ConfigNode == null) return new List<String>();

            // Get Extension from Config.
            List<String> extensionsInfo = new List<String>();
            foreach (XElement extensionsNode in ConfigNode.Descendants("Extensions"))
                foreach (XElement extensionNode in extensionsNode.Descendants("Extension"))
                    extensionsInfo.Add(extensionNode.Value);

            // Return Extensions we found.
            return extensionsInfo;
        }
    }
}
