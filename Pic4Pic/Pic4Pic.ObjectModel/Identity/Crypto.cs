using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public partial class Crypto
    {
        public static string GetRandomKeyFor128BitAES()
        {
            byte[] key = GetRandomBytes(16);
            return Convert.ToBase64String(key);
        }

        public static byte[] GetRandomBytes(int size)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            byte[] data = new byte[size];
            for (int x = 0; x < size; x++)
            {
                data[x] = (byte)rand.Next(0, 255);
            }

            return data;
        }

        public static string EncodeBase64(string plainText)
        {
            byte[] encodedBytes = new UTF8Encoding().GetBytes(plainText);
            return Convert.ToBase64String(encodedBytes);
        }

        public static string DecodeBase64(string encodedText)
        {
            byte[] decodedBytes = Convert.FromBase64String(encodedText);
            return new UTF8Encoding().GetString(decodedBytes);
        }

        public static string EncryptAES(string plainText, string secretKey, string iv)
        {
            if (String.IsNullOrWhiteSpace(plainText))
            {
                throw new Pic4PicException("Text to enctrypt is null or empty");
            }

            if (String.IsNullOrWhiteSpace(secretKey))
            {
                throw new Pic4PicException("Key to be used in AES is null or empty");
            }

            if (String.IsNullOrWhiteSpace(iv))
            {
                throw new Pic4PicException("IV to be used in AES is null or empty");
            }

            byte[] keyBytes = Convert.FromBase64String(secretKey);
            byte[] ivBytes = Convert.FromBase64String(iv);

            byte[] encryptedBytes = EncryptAES(plainText, keyBytes, ivBytes);
            if (encryptedBytes == null || encryptedBytes.Length == 0)
            {
                throw new Pic4PicException("Encryption returned null");
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        private static byte[] EncryptAES(string plainText, byte[] key, byte[] iv)
        {
            // Create an AesCryptoServiceProvider object with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public static string DecryptAES(string cipherText, string secretKey, string iv)
        {
            if (String.IsNullOrWhiteSpace(cipherText))
            {
                throw new Pic4PicException("Cypher Text to dectrypt is null or empty");
            }

            if (String.IsNullOrWhiteSpace(secretKey))
            {
                throw new Pic4PicException("Key to be used in AES is null or empty");
            }

            if (String.IsNullOrWhiteSpace(iv))
            {
                throw new Pic4PicException("IV to be used in AES is null or empty");
            }

            byte[] decodedCipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = Convert.FromBase64String(secretKey);
            byte[] ivBytes = Convert.FromBase64String(iv);

            string decrypted = DecryptAES(decodedCipherBytes, keyBytes, ivBytes);
            if (String.IsNullOrWhiteSpace(decrypted))
            {
                throw new Pic4PicException("Decryption returned null or empty");
            }

            return decrypted;
        }

        private static string DecryptAES(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

