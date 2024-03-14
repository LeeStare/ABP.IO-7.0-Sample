//-----------------------------------------------------------------------
// <copyright file="EncryptionMethods.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

namespace TaipeiFishTradingSystem.Utils
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// 加密相關方法
    /// </summary>
    public class EncryptionMethods
    {

        #region DES加密

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data"> 加密資料 </param>
        /// <param name="key"> 8位字元的金鑰字串 </param>
        /// <param name="iv"> 8位字元的初始化向量字串 </param>
        /// <returns></returns>
        public static string DESEncrypt(string data, string key, string iv)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            SymmetricAlgorithm cryptoProvider = DES.Create();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密資料</param>
        /// <param name="key">8位字元的金鑰字串(需要和加密時相同)</param>
        /// <param name="iv">8位字元的初始化向量字串(需要和加密時相同)</param>
        /// <returns></returns>
        public static string DESDecrypt(string data, string key, string iv)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null!;
            }

            SymmetricAlgorithm cryptoProvider = DES.Create();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        #endregion DES加密

        #region AES加密

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText"> 加密資料 </param>
        /// <param name="stringKey"> 128位字元的金鑰字串 </param>
        /// <param name="stringIV"> 128位字元的初始化向量字串 </param>
        /// <returns> Encrypted </returns>
        public static byte[] EncryptStringToBytes_Aes(string plainText, string stringKey, string stringIV)
        {
            byte[] Key = Encoding.UTF8.GetBytes(stringKey);
            byte[] IV = Encoding.UTF8.GetBytes(stringIV);

            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
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
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">解密資料</param>
        /// <param name="stringKey">128位字元的金鑰字串(需要和加密時相同)</param>
        /// <param name="stringIV">128位字元的初始化向量字串(需要和加密時相同)</param>
        /// <returns> 明文 </returns>
        public static string DecryptStringFromBytes_Aes(byte[] cipherText, string stringKey, string stringIV)
        {
            byte[] Key = Encoding.UTF8.GetBytes(stringKey);
            byte[] IV = Encoding.UTF8.GetBytes(stringIV);

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null!;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
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
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        #endregion AES加密

        #region RSA

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="DataToEncrypt">加密資料</param>
        /// <param name="RSAKeyInfo"> 公鑰 </param>
        /// <param name="DoOAEPPadding">是否補足</param>
        /// <returns> 明文 </returns>
        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //to include the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null!;
            }
        }

        #endregion RSA

        #region SHA256

        /// <summary>
        /// SHA256雜湊
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Get_SHA256_Hash(string value)
        {
            using var hash = SHA256.Create();
            var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            var response = Convert.ToBase64String(byteArray).ToLower();
            return response;
        }

        #endregion SHA256
    }
}