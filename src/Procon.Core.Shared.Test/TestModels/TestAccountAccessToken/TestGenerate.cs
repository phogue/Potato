using System;
using NUnit.Framework;
using Procon.Core.Shared.Models;

namespace Procon.Core.Shared.Test.TestModels.TestAccountAccessToken {
    [TestFixture]
    public class TestGenerate {

        public const String Identifer = "identifer";

        /// <summary>
        /// Tests a null (no) token is returned if no one owns the access token.
        /// </summary>
        [Test]
        public void TestNullTokenReturnedWhenAccountEmpty() {
            AccessTokenModel accessToken = new AccessTokenModel();

            var token = accessToken.Generate(TestGenerate.Identifer);

            Assert.IsNull(token);
        }

        /// <summary>
        /// Tests a null (no) token is returned if some one owns the token but they have no password hash set.
        /// </summary>
        [Test]
        public void TestNullTokenReturnedWhenAccountHashNull() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = null
                }
            };

            var token = accessToken.Generate(TestGenerate.Identifer);

            Assert.IsNull(token);
        }

        /// <summary>
        /// Tests a null (no) token is returned if some one owns the token but they have an empty password hash set.
        /// </summary>
        [Test]
        public void TestNullTokenReturnedWhenAccountHashEmpty() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = ""
                }
            };

            var token = accessToken.Generate(TestGenerate.Identifer);

            Assert.IsNull(token);
        }

        /// <summary>
        /// Tests a null (no) token is returned if the identifer is nulled out
        /// </summary>
        [Test]
        public void TestNullTokenReturnedWhenIdentiferNull() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = "password"
                }
            };

            var token = accessToken.Generate(null);

            Assert.IsNull(token);
        }

        /// <summary>
        /// Tests a null (no) token is returned if the identifer is empty
        /// </summary>
        [Test]
        public void TestNullTokenReturnedWhenIdentiferEmpty() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = "password"
                }
            };

            var token = accessToken.Generate("");

            Assert.IsNull(token);
        }

        /// <summary>
        /// Tests a token is returned when all required credentials exist.
        /// </summary>
        [Test]
        public void TestGoodTokenReturnedWhenAllCredentialsPassed() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = "password"
                }
            };

            var token = accessToken.Generate("192.168.1.1");

            Assert.IsNotNull(token);
        }

        /// <summary>
        /// Simply tests with identical credentials that two random tokens are generated.
        /// </summary>
        [Test]
        public void TestReturnedTokenIsRandom() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = "password"
                }
            };

            var tokenA = accessToken.Generate("192.168.1.1");
            var tokenB = accessToken.Generate("192.168.1.1");

            Assert.IsNotNull(tokenA);
            Assert.IsNotNull(tokenB);
            Assert.AreNotEqual(tokenA, tokenB);
        }

        /// <summary>
        /// Tests the last touched parameter is set 
        /// </summary>
        [Test]
        public void TestLastTouchedResetToCurrentDateTime() {
            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    PasswordHash = "password"
                },
                LastTouched = DateTime.Now.AddDays(-1)
            };

            var token = accessToken.Generate("192.168.1.1");

            Assert.IsNotNull(token);
            Assert.GreaterOrEqual(accessToken.LastTouched, DateTime.Now.AddMinutes(-1));
        }
    }
}
