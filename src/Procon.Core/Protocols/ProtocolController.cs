using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Procon.Core.Shared;
using Procon.Net.Shared;
using Procon.Service.Shared;

namespace Procon.Core.Protocols {
    /// <summary>
    /// Manages loading and maintaining a list of available protocol assemblies.
    /// </summary>
    public class ProtocolController : CoreController, ISharedReferenceAccess {
        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// List of protocols and the assemblies to support them.
        /// </summary>
        public List<IProtocolAssemblyMetadata> Protocols { get; set; }

        /// <summary>
        /// Initializes the protocol controller with default values.
        /// </summary>
        public ProtocolController() : base() {
            this.Shared = new SharedReferences();

            this.Protocols = new List<IProtocolAssemblyMetadata>();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.ProtocolsFetchSupportedProtocols,
                    Handler = this.ProtocolsFetchSupportedProtocols
                }
            });
        }

        /// <summary>
        /// Fetches a list of assembly files to load.
        /// </summary>
        /// <returns></returns>
        public List<FileInfo> GetProtocolAssemblies() {
            return Directory.GetFiles(Defines.PackagesDirectory.FullName, @"*.Protocols.*.dll", SearchOption.AllDirectories)
                .Select(path => new FileInfo(path))
                .Where(file => Regex.Matches(file.FullName, file.Name.Replace(file.Extension, String.Empty)).Cast<Match>().Count() >= 2)
                .ToList();
        }

        /// <summary>
        /// Fetches a list of directories pointing to protocol packages.
        /// </summary>
        /// <returns></returns>
        public List<DirectoryInfo> GetProtocolPackages(List<FileInfo> assemblies) {
            return assemblies
                .Select(file => Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, file.Name.Replace(file.Extension, String.Empty)))
                .Where(path => path != null)
                .Distinct()
                .Select(path => new DirectoryInfo(path))
                .Where(package => package != null)
                .ToList();
        }

        /// <summary>
        /// Loads all of the protocols meta data
        /// </summary>
        public void LoadProtocolsMetadata() {
            this.Protocols.Clear();

            // List of all matching file assemblies
            List<FileInfo> assemblies = this.GetProtocolAssemblies();

            // List of the latest packages containing protocols
            List<DirectoryInfo> packages = this.GetProtocolPackages(assemblies);

            // We have all the possible names of assemblies, we have the list of latest packages
            // so now we get a list of distinct names and find packages containing both (Name + ".json" and Name + ".dll") files.
            List<String> names = assemblies.Select(assembly => assembly.Name.Replace(assembly.Extension, String.Empty)).Distinct().ToList();

            // Search for both files within a single package.
            foreach (DirectoryInfo package in packages) {
                foreach (String name in names) {
                    var json = Directory.GetFiles(Defines.PackagesDirectory.FullName, name + ".json", SearchOption.AllDirectories);
                    var dll = Directory.GetFiles(Defines.PackagesDirectory.FullName, name + ".dll", SearchOption.AllDirectories);

                    if (json.Length > 0 && dll.Length > 0) {
                        var meta = new ProtocolAssemblyMetadata() {
                            Name = name,
                            Assembly = new FileInfo(dll.First()),
                            Meta = new FileInfo(json.First()),
                            Directory = package
                        };

                        if (meta.Load() == true) {
                            this.Protocols.Add(meta);
                        }
                    }
                }
            }
        }

        public override ICoreController Execute() {
            // Load all available protocols
            this.LoadProtocolsMetadata();

            return base.Execute();
        }

        /// <summary>
        /// Fetches all of the protocols from the cache
        /// </summary>
        public ICommandResult ProtocolsFetchSupportedProtocols(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // Give a representation of what we know right now
                result = new CommandResult() {
                    CommandResultType = CommandResultType.Success,
                    Success = true,
                    Now = {
                        ProtocolTypes = this.Protocols.SelectMany(meta => meta.ProtocolTypes).Cast<ProtocolType>().ToList()
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Disposes all protocol information
        /// </summary>
        public override void Dispose() {
            this.Protocols.Clear();
            this.Protocols = null;

            base.Dispose();
        }
    }
}
