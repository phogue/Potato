using System;
using System.Collections.Generic;
using System.IO;

namespace Procon.Net.Shared {
    /// <summary>
    /// Wraps an assembly reference and supported protocol types.
    /// </summary>
    public interface IProtocolAssemblyMetadata {
        /// <summary>
        /// The name of the file/directory without extension
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// The reference to the dll file
        /// </summary>
        FileInfo Assembly { get; set; }

        /// <summary>
        /// The reference to the meta file
        /// </summary>
        FileInfo Meta { get; set; }

        /// <summary>
        /// The directory holding the assembly and config information
        /// </summary>
        DirectoryInfo Directory { get; set; }

        /// <summary>
        /// The supported protocol types provided by the assembly
        /// </summary>
        List<IProtocolType> ProtocolTypes { get; set; } 
    }
}
