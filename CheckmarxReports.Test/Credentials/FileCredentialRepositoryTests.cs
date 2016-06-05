using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.Credentials;
using NUnit.Framework;

namespace CheckmarxReports.Test.Credentials
{
    [TestFixture]
    public class FileCredentialRepositoryTests
    {
        [Test]
        public void Ctor()
        {
            FileCredentialRepository fileCredentialRepository;
            const string filePath = "foo.json";

            fileCredentialRepository = new FileCredentialRepository(filePath);
            Assert.That(
                fileCredentialRepository,
                Has.Property("FilePath").EqualTo(filePath));
            Assert.That(
                fileCredentialRepository,
                Has.Property("Scope").EqualTo(DataProtectionScope.CurrentUser));
        }

        [Test]
        public void Encryption_Decryption()
        {
            FileCredentialRepository fileCredentialRepository;
            EncryptedCredential encryptedCredential;
            const string filePath = "foo.json";
            const string userName = "userName";
            const string password = "password";
            string decryptedUserName;
            string decryptedPassword;

            try
            {
                fileCredentialRepository = new FileCredentialRepository(filePath);

                encryptedCredential = fileCredentialRepository.Encrypt(userName, password);

                encryptedCredential.AssertValid();
                Assert.That(encryptedCredential, Has.Property("UserName").Not.EqualTo(userName));
                Assert.That(encryptedCredential, Has.Property("Password").Not.EqualTo(password));

                fileCredentialRepository.Decrypt(encryptedCredential, out decryptedUserName, out decryptedPassword);

                Assert.That(decryptedUserName, Is.EqualTo(userName));
                Assert.That(decryptedPassword, Is.EqualTo(password));
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void Save_Load()
        {
            FileCredentialRepository fileCredentialRepository;
            const string filePath = "foo.json";
            const string server = "server";
            const string userName = "userName";
            const string password = "password";
            string loadedUserName;
            string loadedPassword;

            try
            {
                DeleteFile(filePath);

                fileCredentialRepository = new FileCredentialRepository(filePath);

                Assert.That(fileCredentialRepository.Contains(server), Is.False,
                    "No existing credentials");

                fileCredentialRepository.Save(server, userName, password);

                Assert.That(fileCredentialRepository.Contains(server), Is.True,
                    "Credentials not saved after Save");

                fileCredentialRepository.Load(server, out loadedUserName, out loadedPassword);

                Assert.That(fileCredentialRepository.Contains(server), Is.True,
                    "Credentials not saved after load");
                Assert.That(loadedUserName, Is.EqualTo(userName));
                Assert.That(loadedPassword, Is.EqualTo(password));
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Test]
        public void Save_Two_Servers_Load()
        {
            FileCredentialRepository fileCredentialRepository;
            const string filePath = "foo.json";
            const string server1 = "server";
            const string userName1 = "userName";
            const string password1 = "password";
            const string server2 = "server2";
            const string userName2 = "userName2";
            const string password2 = "password2";
            string loadedUserName;
            string loadedPassword;

            try
            {
                DeleteFile(filePath);

                fileCredentialRepository = new FileCredentialRepository(filePath);

                Assert.That(fileCredentialRepository.Contains(server1), Is.False,
                    "No existing credentials for 1");
                Assert.That(fileCredentialRepository.Contains(server2), Is.False,
                    "No existing credentials for 2");

                fileCredentialRepository.Save(server1, userName1, password1);

                Assert.That(fileCredentialRepository.Contains(server1), Is.True,
                    "Credentials not saved after Save for 1");
                Assert.That(fileCredentialRepository.Contains(server2), Is.False,
                    "Credentials saved after Save for 1 only");

                fileCredentialRepository.Save(server2, userName2, password2);

                Assert.That(fileCredentialRepository.Contains(server1), Is.True,
                    "Credentials not saved after Save for 1 and 2");
                Assert.That(fileCredentialRepository.Contains(server2), Is.True,
                    "Credentials not saved after Save for 2 and 2");

                fileCredentialRepository.Load(server1, out loadedUserName, out loadedPassword);

                Assert.That(fileCredentialRepository.Contains(server1), Is.True,
                    "Credentials not saved after load");
                Assert.That(loadedUserName, Is.EqualTo(userName1));
                Assert.That(loadedPassword, Is.EqualTo(password1));
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        /// <summary>
        /// Clean up test files.
        /// </summary>
        /// <param name="filePath"></param>
        private void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
