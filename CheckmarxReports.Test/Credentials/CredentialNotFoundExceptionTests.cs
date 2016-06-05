using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.Credentials;
using NUnit.Framework;

namespace CheckmarxReports.Test.Credentials
{
    [TestFixture]
    public class CredentialNotFoundExceptionTests
    {
        [Test]
        public void Ctor()
        {
            Assert.That(new CredentialNotFoundException("foo"), 
                Has.Property("Message").EqualTo("Credentials not found for server 'foo'"));
        }
    }
}
