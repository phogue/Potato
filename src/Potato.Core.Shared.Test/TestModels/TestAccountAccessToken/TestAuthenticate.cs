using System;
using NUnit.Framework;
using Potato.Core.Shared.Models;

namespace Potato.Core.Shared.Test.TestModels.TestAccountAccessToken {
    [TestFixture]
    public class TestAuthenticate {

        public AccessTokenModel Valid { get; set; }

        public const string Identifer = "identifer";

        [SetUp]
        public void Setup() {
            Valid = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = "Password Hash"
                }
            };
        }

        /// <summary>
        /// Tests all valid data will result in a successful authentication.
        /// </summary>
        [Test]
        public void TestSuccessfullyAuthenticate() {
            var token = Valid.Generate(Identifer);

            Assert.IsTrue(Valid.Authenticate(Valid.Id, token, Identifer));
        }

        /// <summary>
        /// Tests all valid data will result in a successful authentication will set the LastTouched to now.
        /// </summary>
        [Test]
        public void TestSuccessfullyAuthenticationTouchesTime() {
            var token = Valid.Generate(Identifer);

            Valid.LastTouched = DateTime.Now.AddDays(-1);

            Valid.Authenticate(Valid.Id, token, Identifer);

            Assert.GreaterOrEqual(Valid.LastTouched, DateTime.Now.AddMinutes(-1));
        }

        /// <summary>
        /// Tests that if the account holders password hash is modified at all (even if the underlying password is the same)
        /// then the access token is essentially revoked.
        /// </summary>
        [Test]
        public void TestAlteringPasswordHashNegatesAccessToken() {
            var token = Valid.Generate(Identifer);

            Valid.Account.PasswordHash = "Reset My Password";

            Assert.IsFalse(Valid.Authenticate(Valid.Id, token, Identifer));
        }

        /// <summary>
        /// Tests that a different identifier will negate the access token
        /// </summary>
        [Test]
        public void TestDifferentIdentifierNegatesAccessToken() {
            var token = Valid.Generate(Identifer);

            Assert.IsFalse(Valid.Authenticate(Valid.Id, token, "different"));
        }

        /// <summary>
        /// Tests that a different guid will fail authentication.
        /// </summary>
        [Test]
        public void TestMissmatchIdReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, Identifer));
        }

        /// <summary>
        /// Tests that an expired access token is not valid (2 days max)
        /// </summary>
        [Test]
        public void TestExpiredAccessTokenDenied() {
            var token = Valid.Generate(Identifer);

            Valid.LastTouched = DateTime.Now.AddDays(-3);

            Assert.IsFalse(Valid.Authenticate(Valid.Id, token, Identifer));
        }

        /// <summary>
        /// Tests that passing in a null token results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledTokenReturnsFalse() {
            Valid.Generate(Identifer);

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), null, Identifer));
        }

        /// <summary>
        /// Tests that passing in an empty token results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyTokenReturnsFalse() {
            Valid.Generate(Identifer);

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), "", Identifer));
        }

        /// <summary>
        /// Tests that passing in a null identifer results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledIdentiferReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, null));
        }

        /// <summary>
        /// Tests that passing in an empty identifer results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyIdentiferReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, ""));
        }

        /// <summary>
        /// Tests that passing in a null token hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledTokenHashReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Valid.TokenHash = null;

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, Identifer));
        }

        /// <summary>
        /// Tests that passing in an empty token hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyTokenHashReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Valid.TokenHash = "";

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, Identifer));
        }

        /// <summary>
        /// Tests that passing in a null account results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledAccountReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Valid.Account = null;

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, Identifer));
        }

        /// <summary>
        /// Tests that passing in a null password hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledPasswordHashReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Valid.Account.PasswordHash = null;

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, Identifer));
        }

        /// <summary>
        /// Tests that passing in an empty password hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyPasswordHashReturnsFalse() {
            var token = Valid.Generate(Identifer);

            Valid.Account.PasswordHash = "";

            Assert.IsFalse(Valid.Authenticate(Guid.NewGuid(), token, Identifer));
        }
    }
}
