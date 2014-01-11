using NUnit.Framework;
using Procon.Core.Remote;

namespace Procon.Core.Test.Remote.TestCommandServerController {
    [TestFixture]
    public class TestDispose {
        /// <summary>
        ///     Tests variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestCommandServerDisposed() {
            var commandServer = new CommandServerController();

            commandServer.Dispose();

            Assert.IsNull(commandServer.CommandServerListener);
            Assert.IsNull(commandServer.TunnelObjects);
        }
    }
}
