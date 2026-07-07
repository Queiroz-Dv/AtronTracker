using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Application.Extensions
{
    public static class CryptoHelper
    {
        // Chave secreta de 32 bytes para o AES-256 (256 bits).
        // DEVE estar sincronizada com a chave do frontend (CryptoJS).
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("AtronTrackerSecretKeyAES256Bits!");
        // Opcional: Vetor de inicialização (IV) fixo ou lido do prefixo. Aqui usamos um fixo padrão fácil para sincro, ou ler dos primeiros 16 bytes.
        // Se no front usarmos CryptoJS.AES.encrypt(texto, chave), ele gera um salt e IV aleatório, que fica contido no Base64 resultante e tem o prefixo "Salted__".
        // Para descriptografar um valor do CryptoJS padrão:
        
        public static string DecryptCryptoJsAes(string encryptedBase64)
        {
            if (string.IsNullOrWhiteSpace(encryptedBase64)) return encryptedBase64;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);

                // CryptoJS starts with "Salted__" (8 bytes) + salt (8 bytes) = 16 bytes total prefix
                // If it doesn't start with "Salted__", consider it plain AES or fallback
                string prefix = Encoding.ASCII.GetString(cipherBytes, 0, 8);
                if (prefix != "Salted__")
                {
                    // Tratar como outro esquema ou retornar throw.
                    return encryptedBase64; // fallback para devolver a string
                }

                byte[] salt = new byte[8];
                Array.Copy(cipherBytes, 8, salt, 0, 8);

                byte[] actualCipherText = new byte[cipherBytes.Length - 16];
                Array.Copy(cipherBytes, 16, actualCipherText, 0, cipherBytes.Length - 16);

                // EvpKDF (simulando a derivação de chave do CryptoJS: MD5 hash)
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
                // Se falhar a descriptografia, retorna fallback
                return string.Empty;
            }
        }

        // DeriveEvpKDF = método que transforma a senha secreta em chave + IV compatíveis com CryptoJS
        private static void DeriveEvpKDF(byte[] password, byte[] salt, out byte[] key, out byte[] iv)
        {
            // EvpKDF using MD5 to generate 32 bytes key and 16 bytes IV
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
        /// <summary>
        /// Criptografa uma string utilizando AES-256 compatível com CryptoJS.
        /// O resultado é um Base64 com prefixo "Salted__" + salt + ciphertext.
        /// </summary>
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
                        // Escrever prefixo "Salted__" + salt
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

        /// <summary>
        /// Gera um identificador temporário de 9 dígitos alfanuméricos,
        /// embutindo o código do usuário (3 caracteres) nas posições 0, 4 e 8.
        /// As posições 1,2,3,5,6,7 são preenchidas com caracteres aleatórios.
        /// </summary>
        public static string GerarIdentificadorTemporario(string usuarioCodigo)
        {
            const string chars = "0123456789";
            var resultado = new char[9];
            var random = new Random();

            // Posições fixas para o código do usuário: 0, 4, 8
            resultado[0] = usuarioCodigo[0];
            resultado[4] = usuarioCodigo[1];
            resultado[8] = usuarioCodigo[2];

            // Preencher posições restantes com caracteres aleatórios
            int[] posicoesAleatorias = { 1, 2, 3, 5, 6, 7 };
            foreach (var pos in posicoesAleatorias)
            {
                resultado[pos] = chars[random.Next(chars.Length)];
            }

            return new string(resultado);
        }
    }
}
