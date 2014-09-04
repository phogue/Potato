using System;
using System.Collections.Generic;
using Potato.Core.Properties;
using Potato.Core.Remote.Interface.Pages;
using Potato.Core.Shared;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Remote.Interface {
    /// <summary>
    /// Handles serving a basic interface over http as well as supplying some assets
    /// </summary>
    public class InterfaceController : CoreController {

        public InterfaceController() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "/",
                    Handler = this.Index
                }
            });

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "/assets/css/style.css",
                    Handler = this.AssetsCssStyle
                }
            });
        }

        protected ICommandResult Index(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            IndexPageView index = new IndexPageView();
            
            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        index.TransformText()
                    }
                },
                ContentType = Mime.TextHtml,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }

        protected ICommandResult AssetsCssStyle(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<String>() {
                        Resources.Style
                    }
                },
                ContentType = Mime.TextCss,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }
    }
}
