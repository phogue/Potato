using System.Linq;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Shared;

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestCommandInstanceServiceMergePackage {
        /// <summary>
        /// Tests that attempting the command without any users in the security controller will
        /// result in insufficient permissions
        /// </summary>
        [Test]
        public void TestResultInsufficientPermissions() {
            InstanceController instance = new InstanceController();

            CommandResultArgs result = instance.Tunnel(CommandBuilder.InstanceServiceMergePackage("localhost", "id").SetOrigin(CommandOrigin.Remote).SetUsername("Phogue"));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that passing in an empty uri will result in an invalid parameter status
        /// </summary>
        [Test]
        public void TestResultInvalidParameterUri() {
            InstanceController instance = new InstanceController();

            CommandResultArgs result = instance.Tunnel(CommandBuilder.InstanceServiceMergePackage("", "id").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        /// Tests that passing in an empty packageId will result in an invalid parameter status
        /// </summary>
        [Test]
        public void TestResultInvalidParameterPackageId() {
            InstanceController instance = new InstanceController();

            CommandResultArgs result = instance.Tunnel(CommandBuilder.InstanceServiceMergePackage("localhost", "").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        /// Tests that with permissions the result will be a success.
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            InstanceController instance = new InstanceController();

            CommandResultArgs result = instance.Tunnel(CommandBuilder.InstanceServiceMergePackage("localhost", "id").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests that a service message is set when successfully executing a merge package command.
        /// </summary>
        [Test]
        public void TestMessageLogged() {
            InstanceController instance = new InstanceController();

            instance.Tunnel(CommandBuilder.InstanceServiceMergePackage("localhost", "id").SetOrigin(CommandOrigin.Local));

            Assert.IsNotNull(instance.ServiceMessage);
            Assert.AreEqual("merge-package", instance.ServiceMessage.Name);
            Assert.AreEqual("localhost", instance.ServiceMessage.Arguments["uri"]);
            Assert.AreEqual("id", instance.ServiceMessage.Arguments["packageid"]);
        }

        /// <summary>
        /// Tests that an event is logged for a restart when successfully executing a restart command.
        /// </summary>
        [Test]
        public void TestEventLogged() {
            EventsController events = new EventsController();
            InstanceController instance = new InstanceController {
                Shared = {
                    Events = events
                }
            };

            instance.Tunnel(CommandBuilder.InstanceServiceMergePackage("localhost", "id").SetOrigin(CommandOrigin.Local));

            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.AreEqual("InstanceServiceMergePackage", events.LoggedEvents.First().Name);
        }
    }
}
