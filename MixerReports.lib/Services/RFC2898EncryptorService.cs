using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MixerReports.lib.Interfaces;

namespace MixerReports.lib.Services
{
    public class RFC2898EncryptorService : IEncryptService
    {
        private static readonly byte[] Salt =
        {
            0x3f, 0x34, 0x12, 0x78,
            0x22, 0xb6, 0xc9, 0xf5,
            0x98, 0xa9, 0x1a, 0x99,
            0x51, 0x30, 0x02, 0x11,
        };
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary> Шифрование текста </summary>
        /// <param name="text">оригинальный текст</param>
        /// <param name="password">ключ</param>
        /// <returns>зашифрованный текст</returns>
        public string Encrypt(string text, string password)
        {
            var encoding = Encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(text);
            var encryptedBytes = Encrypt(bytes, password);
            return Convert.ToBase64String(encryptedBytes);
        }
        /// <summary> Расшифрование текста </summary>
        /// <param name="text">зашифрованный текст</param>
        /// <param name="password">ключ</param>
        /// <returns>оригинальный текст</returns>
        public string Decrypt(string text, string password)
        {
            var encryptedBytes = Convert.FromBase64String(text);
            var bytes = Decrypt(encryptedBytes, password);
            var encoding = Encoding ?? Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        public byte[] Encrypt(byte[] data, string password)
        {
            var algorithm = getAlgorithmCryptoTransform(password);
            using var stream = new MemoryStream();
            using var cryproStream = new CryptoStream(stream, algorithm, CryptoStreamMode.Write);
            cryproStream.Write(data, 0, data.Length);
            cryproStream.FlushFinalBlock();
            return stream.ToArray();
        }
        public byte[] Decrypt(byte[] data, string password)
        {
            var algorithm = getInverseAlgorithmCryptoTransform(password);
            using var stream = new MemoryStream();
            using var cryproStream = new CryptoStream(stream, algorithm, CryptoStreamMode.Write);
            cryproStream.Write(data, 0, data.Length);
            cryproStream.FlushFinalBlock();
            return stream.ToArray();
        }

        private static ICryptoTransform getAlgorithmCryptoTransform(string password)
        {
            var algorithm = Algorithm(password);
            return algorithm.CreateEncryptor();
        }
        private static ICryptoTransform getInverseAlgorithmCryptoTransform(string password)
        {
            var algorithm = Algorithm(password);
            return algorithm.CreateDecryptor();
        }
        private static Rijndael Algorithm(string password)
        {
            var pdb = new Rfc2898DeriveBytes(password, Salt);
            var algorithm = Rijndael.Create();
            algorithm.Key = pdb.GetBytes(32);
            algorithm.IV = pdb.GetBytes(16);
            return algorithm;
        }
    }
    /// <summary> Функции-расширения для строк </summary>
    public static class ExtendedRfc2898Encoder
    {
        /// <summary> Функция-расширение шифрование текста  </summary>
        public static string Encrypt(this string Source, string password = "SilikatPlus")
        {
            var encryptor = new RFC2898EncryptorService();
            return encryptor.Encrypt(Source, password);
        }
        /// <summary> Функция-расширение дешифрование текста </summary>
        public static string Decrypt(this string Source, string password = "SilikatPlus")
        {
            var encryptor = new RFC2898EncryptorService();
            return encryptor.Decrypt(Source, password);
        }
    }
}
