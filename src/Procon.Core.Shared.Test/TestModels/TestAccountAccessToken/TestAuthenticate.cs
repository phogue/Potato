using System;
using NUnit.Framework;
using Procon.Core.Shared.Models;

namespace Procon.Core.Shared.Test.TestModels.TestAccountAccessToken {
    [TestFixture]
    public class TestAuthenticate {

        public AccountAccessTokenModel Valid { get; set; }

        public const String Identifer = "identifer";

        [SetUp]
        public void Setup() {
            this.Valid = new AccountAccessTokenModel() {
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
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsTrue(this.Valid.Authenticate(this.Valid.Id, token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests all valid data will result in a successful authentication will set the LastTouched to now.
        /// </summary>
        [Test]
        public void TestSuccessfullyAuthenticationTouchesTime() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.LastTouched = DateTime.Now.AddDays(-1);

            this.Valid.Authenticate(this.Valid.Id, token, TestAuthenticate.Identifer);

            Assert.GreaterOrEqual(this.Valid.LastTouched, DateTime.Now.AddMinutes(-1));
        }

        /// <summary>
        /// Tests that if the account holders password hash is modified at all (even if the underlying password is the same)
        /// then the access token is essentially revoked.
        /// </summary>
        [Test]
        public void TestAlteringPasswordHashNegatesAccessToken() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.Account.PasswordHash = "Reset My Password";

            Assert.IsFalse(this.Valid.Authenticate(this.Valid.Id, token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that a different identifier will negate the access token
        /// </summary>
        [Test]
        public void TestDifferentIdentifierNegatesAccessToken() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsFalse(this.Valid.Authenticate(this.Valid.Id, token, "different"));
        }

        /// <summary>
        /// Tests that a different guid will fail authentication.
        /// </summary>
        [Test]
        public void TestMissmatchIdReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that an expired access token is not valid (2 days max)
        /// </summary>
        [Test]
        public void TestExpiredAccessTokenDenied() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.LastTouched = DateTime.Now.AddDays(-3);

            Assert.IsFalse(this.Valid.Authenticate(this.Valid.Id, token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in a null token results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledTokenReturnsFalse() {
            this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), null, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in an empty token results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyTokenReturnsFalse() {
            this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), "", TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in a null identifer results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledIdentiferReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, null));
        }

        /// <summary>
        /// Tests that passing in an empty identifer results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyIdentiferReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, ""));
        }

        /// <summary>
        /// Tests that passing in a null token hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledTokenHashReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.TokenHash = null;

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in an empty token hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyTokenHashReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.TokenHash = "";

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in a null account results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledAccountReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.Account = null;

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in a null password hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestNulledPasswordHashReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.Account.PasswordHash = null;

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, TestAuthenticate.Identifer));
        }

        /// <summary>
        /// Tests that passing in an empty password hash results in a failed authentication
        /// </summary>
        [Test]
        public void TestEmptyPasswordHashReturnsFalse() {
            var token = this.Valid.Generate(TestAuthenticate.Identifer);

            this.Valid.Account.PasswordHash = "";

            Assert.IsFalse(this.Valid.Authenticate(Guid.NewGuid(), token, TestAuthenticate.Identifer));
        }
    }
}
