using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.Credentials;
using NUnit.Framework;

namespace CheckmarxReports.Test.Credentials
{
    [TestFixture]
    public class EncryptedCredentialTests
    {
        [Test]
        [TestCase(null, "password", "userNameIv", "passswordIv", "UserName")]
        [TestCase("", "password", "userNameIv", "passswordIv", "UserName")]
        [TestCase(" ", "password", "userNameIv", "passswordIv", "UserName")]
        [TestCase("userName", null, "userNameIv", "passswordIv", "Password")]
        [TestCase("userName", "", "userNameIv", "passswordIv", "Password")]
        [TestCase("userName", " ", "userNameIv", "passswordIv", "Password")]
        [TestCase("userName", "password", null, "passswordIv", "UserNameIv")]
        [TestCase("userName", "password", "", "passswordIv", "UserNameIv")]
        [TestCase("userName", "password", " ", "passswordIv", "UserNameIv")]
        [TestCase("userName", "password", "userNameIv", null, "PasswordIv")]
        [TestCase("userName", "password", "userNameIv", "", "PasswordIv")]
        [TestCase("userName", "password", "userNameIv", " ", "PasswordIv")]
        public void AssertValid(string userName, string password, string userNameIv, string passwordIv, string expectedInvalidArgument)
        {
            EncryptedCredential encryptedCredential;

            encryptedCredential = new EncryptedCredential()
            {
                UserName = userName,
                Password =  password,
                UserNameIv = userNameIv,
                PasswordIv = passwordIv
            };

            if (expectedInvalidArgument == null)
            {
                Assert.That(
                    () => encryptedCredential.AssertValid(),
                    Throws.Nothing);
            }
            else
            {
                Assert.That(
                    () => encryptedCredential.AssertValid(),
                    Throws.TypeOf<ValidationException>()
                          .And.Property("ValidationResult").Property("MemberNames").EquivalentTo(new [] { expectedInvalidArgument} ));
            }
        }
    }
}
