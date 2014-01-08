using System.Linq;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Shared;

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestCommandInstanceServiceRestart {
        /// <summary>
        /// Tests that attempting the command without any users in the security controller will
        /// result in insufficient permissions
        /// </summary>
        [Test]
        public void TestResultInsufficientPermissions() {
            Instance instance = new Instance();

            CommandResultArgs result = instance.Tunnel(CommandBuilder.InstanceServiceRestart().SetOrigin(CommandOrigin.Remote).SetUsername("Phogue"));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that with permissions the result will be a success.
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            Instance instance = new Instance();

            CommandResultArgs result = instance.Tunnel(CommandBuilder.InstanceServiceRestart().SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests that a service message is set when successfully executing a restart command.
        /// </summary>
        [Test]
        public void TestMessageLogged() {
            Instance instance = new Instance();

            instance.Tunnel(CommandBuilder.InstanceServiceRestart().SetOrigin(CommandOrigin.Local));

            Assert.IsNotNull(instance.ServiceMessage);
            Assert.AreEqual("restart", instance.ServiceMessage.Name);
        }

        /// <summary>
        /// Tests that an event is logged for a restart when successfully executing a restart command.
        /// </summary>
        [Test]
        public void TestEventLogged() {
            EventsController events = new EventsController();
            Instance instance = new Instance {
                Shared = {
                    Events = events
                }
            };

            instance.Tunnel(CommandBuilder.InstanceServiceRestart().SetOrigin(CommandOrigin.Local));

            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.AreEqual("InstanceServiceRestarting", events.LoggedEvents.First().Name);
        }
    }
}
