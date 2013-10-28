using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Procon.Core.Events;
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils.HTTP;

namespace TestPlugin.Tests {
    using Procon.Core;
    using Pages;

    public class TestPluginsWebUi : ExecutableBase {

        [CommandAttribute(Name = "TestPlugin/index")]
        protected CommandResultArgs TestPluginWebUiIndex(Command command, String name, int score) {
            Player player = new Player() {
                Name = name,
                Score = score
            };
            
            IndexPageView index = new IndexPageView() {
                Player = player
            };

            command.Result = new CommandResultArgs() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        index.TransformText()
                    }
                },
                ContentType = Mime.TextHtml,
                Status = CommandResultType.Success,
                Success = true,
                Message = name
            };

            return command.Result;
        }
    }
}
