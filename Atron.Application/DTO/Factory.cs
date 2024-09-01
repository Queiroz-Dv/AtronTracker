using System;
using System.Security.Cryptography;

namespace Atron.Application.DTO
{
    /// <summary>
    /// Classe utilizada utilizada para processos comuns entre as entidades
    /// </summary>
    public class Factory
    {
        // A geração dos identificadores tem como objetivo manter a identidade dos dados garantindo sua segurança
        // ou seja, para entidades de menor importância usaremos o índice clusterizado ao invés da criação
        // de identificadores únicos.

        // Para a geração dos ids estou utilizando a abordagem COMB (Combined Guid/TimeStamp) que substitui
        // uma parte do GUID por um valor que é garantido o seu acréscimo

        public int GerarIdentificador()
        {

            var sequencial = NovoSequencial();
            var sequencialToString = sequencial.ToString("N");

            var novoSequencial = Convert.ToInt32(sequencialToString);
            return novoSequencial;
            //// Gera um GUID
            //Guid id = Guid.NewGuid();

            //// Obtém os caracteres do guid
            //string guid = id.ToString("N");

            //// Obtém apenas os dígitos (números)
            //string digitsOfId = new string(guid.Where(char.IsDigit).ToArray());

            //// Separa do 0 até o 6
            //digitsOfId = digitsOfId.Substring(0, 6);

            //// Converte para int 32
            //var newId = Convert.ToInt32(digitsOfId);

            //return newId;
        }


        public Guid NovoSequencial()
        {
            var rng = RandomNumberGenerator.Create();
            byte[] randomBytes = new byte[10];
            rng.GetBytes(randomBytes);

            long timestamp = DateTime.UtcNow.Ticks / 10000L;
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] guidBytes = new byte[16];

            Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
            Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);

            return new Guid(guidBytes);
        }
    }   
}