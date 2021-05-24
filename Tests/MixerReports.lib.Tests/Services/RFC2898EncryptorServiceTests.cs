using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MixerReports.lib.Interfaces;
using MixerReports.lib.Services;

namespace MixerReports.lib.Tests.Services
{
    [TestClass]
    public class RFC2898EncryptorServiceTests
    {
        private IEncryptService _encrypt = new RFC2898EncryptorService();
        [TestMethod]
        public void EncryptTest_Hello_And_Decrypt_with_Password()
        {
            const string expected = "Hello World!";
            const string key = "password12345";

            var encrypt = _encrypt.Encrypt(expected, key);
            var decrypt = _encrypt.Decrypt(encrypt, key);

            Assert.AreEqual(decrypt, expected);
        }
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void EncryptTest_Wrong_Decryption_thrown_CryptographicException()
        {
            const string str = "Hello World!";
            const string password = "password12345";

            var encryptedStr = _encrypt.Encrypt(str, password);
            var wrong = _encrypt.Decrypt(encryptedStr, "123");
        }
    }
}
