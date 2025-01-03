using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sempi5.Domain.TokenEntity
{
    /// <summary>
    /// Provides methods for encrypting and decrypting strings using AES encryption.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    public class Cryptography
    {
        private readonly string key = "E546C8DF278CD5931069B522E695D4F2";

        /// <summary>
        /// Encrypts a plain text string using AES encryption.
        /// The resulting ciphertext is base64-encoded and the IV is prepended to the encrypted data.
        /// </summary>
        /// <param name="plainText">The string to be encrypted.</param>
        /// <returns>A base64-encoded encrypted string with the IV prepended.</returns>
        public string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(iv, 0, iv.Length);
                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                    }
                    string encrypted = Convert.ToBase64String(memoryStream.ToArray());

                    return encrypted.Replace('/', '_');
                }
            }
        }

        /// <summary>
        /// Decrypts an AES-encrypted base64-encoded string.
        /// The IV is extracted from the beginning of the encrypted data and used for decryption.
        /// </summary>
        /// <param name="cipherText">The base64-encoded encrypted string to be decrypted.</param>
        /// <returns>The decrypted plain text string.</returns>
        public string DecryptString(string cipherText)
        {
            cipherText = cipherText.Replace('_', '/');
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                using (var memoryStream = new MemoryStream(cipher))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
