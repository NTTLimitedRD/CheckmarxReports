using System;
using System.Runtime.Remoting.Channels;
using NUnit.Framework;

namespace CheckmarxReports.Test
{
    [TestFixture]
    public class CheckmarxApiSessionTests
    {
        [Test]
        [TestCase(null, "username", "password", "server")]
        [TestCase("", "username", "password", "server")]
        [TestCase(" ", "username", "password", "server")]
        [TestCase("server", null, "password", "username")]
        [TestCase("server", "", "password", "username")]
        [TestCase("server", " ", "password", "username")]
        [TestCase("server", "username", null, "password")]
        [TestCase("server", "username", "", "password")]
        [TestCase("server", "username", " ", "password")]
        public void Ctor_InvalidArg(string server, string username, string password, string expectedInvalidParamName)
        {
            Assert.That(() => new CheckmarxApiSession(server, username, password),
                Throws.ArgumentException.And.Property("ParamName").EqualTo(expectedInvalidParamName));
        }
    }
}
