using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Potato.Core.Shared;
using Potato.Net.Shared.Utils;
using Potato.Net.Shared.Utils.HTTP;
using Potato.Service.Shared;

namespace Potato.Core.Interface {
    /// <summary>
    /// Handles serving a basic interface over http as well as supplying some assets
    /// </summary>
    public class InterfaceController : CoreController {

        /// <summary>
        /// The root path of the interface files
        /// </summary>
        public DirectoryInfo StaticInterfacePath { get; set; }

        /// <summary>
        /// Loads all the files from the static interface path
        /// </summary>
        public ConcurrentDictionary<String, byte[]> StaticCache { get; set; } 

        /// <summary>
        /// Initializes the controller with the default values and routes
        /// </summary>
        public InterfaceController() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    NamePattern = new Regex("/.*"),
                    Handler = this.ServeFile
                }
            });

            this.StaticCache = new ConcurrentDictionary<String, byte[]>();

            this.StaticInterfacePath = this.BuildStaticInterfacePath();
        }

        /// <summary>
        /// Loads the static cache files for later serving
        /// </summary>
        /// <returns></returns>
        public override ICoreController Execute() {
            this.BuildStaticCache();

            return base.Execute();
        }

        /// <summary>
        /// Finds the index.html in the core content/Interface directory or in the lib/net40 directory
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo BuildStaticInterfacePath() {
            DirectoryInfo path = new DirectoryInfo(Defines.PackageMyrconPotatoCoreLibNet40.FullName);
            
            var index = Defines.SearchPaths("index.html", new List<String>() {
                Path.Combine(Defines.PackageMyrconPotatoCoreContent.FullName, Defines.InterfaceDirectoryName),
                Path.Combine(Defines.PackageMyrconPotatoCoreLibNet40.FullName, Defines.InterfaceDirectoryName)
            }).FirstOrDefault();

            if (String.IsNullOrEmpty(index) == false) {
                path = new FileInfo(index).Directory;
            }

            return path;
        }

        /// <summary>
        /// Loads all of the files in the interface directory so files can be served from memory
        /// </summary>
        public void BuildStaticCache() {
            foreach (String path in Directory.GetFiles(this.StaticInterfacePath.FullName, "*", SearchOption.AllDirectories)) {
                var contents = File.ReadAllBytes(path);

                this.StaticCache.AddOrUpdate(path.ReplaceFirst(this.StaticInterfacePath.FullName, "").Slug(), key => contents, (key, value) => contents);
            }
        }

        /// <summary>
        /// Serves static files from 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected ICommandResult ServeFile(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            var file = (command.Name.Equals("/") ? "/index.html" : command.Name).Slug();

            if (this.StaticCache.ContainsKey(file)) {
                byte[] contents;

                if (this.StaticCache.TryGetValue(file, out contents)) {
                    result = new CommandResult() {
                        Now = new CommandData() {
                            Content = new List<String>() {
                                // todo check utf8 in request
                                Encoding.UTF8.GetString(contents)
                            }
                        },
                        ContentType = Mime.TextHtml,
                        CommandResultType = CommandResultType.Success,
                        Success = true
                    };
                }
                else {
                    result = new CommandResult() {
                        CommandResultType = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = new CommandResult() {
                    CommandResultType = CommandResultType.DoesNotExists,
                    Success = false
                };
            }

            return result;
        }
    }
}
