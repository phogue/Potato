using System;
using NUnit.Framework;
using Procon.Core.Shared.Models;

namespace Procon.Core.Shared.Test.TestModels.TestAccountAccessToken {
    [TestFixture]
    public class TestDispose {

        /// <summary>
        /// Tests that all values are defaluted in the access token when disposed.
        /// </summary>
        [Test]
        public void TestAllValuesDefaulted() {
            AccountAccessToken accessToken = new AccountAccessToken() {
                Account = new AccountModel(),
                TokenHash = "Something",
                LastTouched = DateTime.Now,
                Id = Guid.NewGuid()
            };

            accessToken.Dispose();

            Assert.IsNull(accessToken.Account);
            Assert.IsNull(accessToken.TokenHash);
            Assert.AreEqual(Guid.Empty, accessToken.Id);
        }
    }
}
