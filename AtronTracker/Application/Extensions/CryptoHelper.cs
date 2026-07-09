using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Application.Extensions
{
    public static class CryptoHelper
    {       
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("AtronTrackerSecretKeyAES256Bits!");
             
        public static string DecryptCryptoJsAes(string encryptedBase64)
        {
            if (string.IsNullOrWhiteSpace(encryptedBase64)) return encryptedBase64;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);
                
                string prefix = Encoding.ASCII.GetString(cipherBytes, 0, 8);
                if (prefix != "Salted__")
                {                    
                    return encryptedBase64; 
                }

                byte[] salt = new byte[8];
                Array.Copy(cipherBytes, 8, salt, 0, 8);

                byte[] actualCipherText = new byte[cipherBytes.Length - 16];
                Array.Copy(cipherBytes, 16, actualCipherText, 0, cipherBytes.Length - 16);

                DeriveEvpKDF(Key, salt, out byte[] derivedKey, out byte[] iv);

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = derivedKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (MemoryStream msDecrypt = new MemoryStream(actualCipherText))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        
        private static void DeriveEvpKDF(byte[] password, byte[] salt, out byte[] key, out byte[] iv)
        {
            byte[] derivedBytes = new byte[48];
            byte[] block = null;
            int offset = 0;

            using (MD5 md5 = MD5.Create())
            {
                while (offset < 48)
                {
                    if (block != null)
                    {
                        byte[] input = new byte[block.Length + password.Length + salt.Length];
                        Buffer.BlockCopy(block, 0, input, 0, block.Length);
                        Buffer.BlockCopy(password, 0, input, block.Length, password.Length);
                        Buffer.BlockCopy(salt, 0, input, block.Length + password.Length, salt.Length);
                        block = md5.ComputeHash(input);
                    }
                    else
                    {
                        byte[] input = new byte[password.Length + salt.Length];
                        Buffer.BlockCopy(password, 0, input, 0, password.Length);
                        Buffer.BlockCopy(salt, 0, input, password.Length, salt.Length);
                        block = md5.ComputeHash(input);
                    }

                    int copyLen = Math.Min(block.Length, 48 - offset);
                    Buffer.BlockCopy(block, 0, derivedBytes, offset, copyLen);
                    offset += copyLen;
                }
            }

            key = new byte[32];
            iv = new byte[16];
            Buffer.BlockCopy(derivedBytes, 0, key, 0, 32);
            Buffer.BlockCopy(derivedBytes, 32, iv, 0, 16);
        }
        
        public static string EncryptCryptoJsAes(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) return plainText;
            try
            {
                byte[] salt = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                DeriveEvpKDF(Key, salt, out byte[] derivedKey, out byte[] iv);

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = derivedKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        msEncrypt.Write(Encoding.ASCII.GetBytes("Salted__"), 0, 8);
                        msEncrypt.Write(salt, 0, 8);

                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                            csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                            csEncrypt.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
       
        public static string GerarIdentificadorTemporario(string usuarioCodigo)
        {
            const string chars = "0123456789";
            var resultado = new char[9];
            var random = new Random();

            resultado[0] = usuarioCodigo[0];
            resultado[4] = usuarioCodigo[1];
            resultado[8] = usuarioCodigo[2];

            int[] posicoesAleatorias = { 1, 2, 3, 5, 6, 7 };
            foreach (var pos in posicoesAleatorias)
            {
                resultado[pos] = chars[random.Next(chars.Length)];
            }

            return new string(resultado);
        }
    }
}