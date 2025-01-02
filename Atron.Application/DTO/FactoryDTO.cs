using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    /// <summary>
    /// Classe utilizada utilizada para processos comuns entre as entidades
    /// </summary>
    public class FactoryDTO : BaseDTO
    {       
        public FactoryDTO()
        {
            IdSequencial = NovoSequencial();
        }

        // A geração dos identificadores tem como objetivo manter a identidade dos dados garantindo sua segurança
        // ou seja, para entidades de menor importância usaremos o índice clusterizado ao invés da criação
        // de identificadores únicos.

        // Para a geração dos ids estou utilizando a abordagem COMB (Combined Guid/TimeStamp) que substitui
        // uma parte do GUID por um valor que é garantido o seu acréscimo

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public Guid IdSequencial { get; set; }

        public int GerarIdentificador()
        {
            var sequencial = NovoSequencial();
            var sequencialToString = sequencial.ToString("N");
            var novoSequencial = Convert.ToInt32(sequencialToString);
            return novoSequencial;            
        }


        public Guid NovoSequencial()
        {
            // Cria uma instância do gerador de números aleatórios criptograficamente seguro.
            var rng = RandomNumberGenerator.Create();

            // Cria um array de bytes de tamanho 10 que será preenchido com valores aleatórios.
            byte[] randomBytes = new byte[10];

            // Preenche o array randomBytes com bytes aleatórios gerados pelo rng.
            rng.GetBytes(randomBytes);

            // Obtém o número de ticks (unidades de tempo de 100 nanosegundos) desde 1 de janeiro de 0001 até o momento atual.
            // Divide por 10000 para converter os ticks para milissegundos.
            long timestamp = DateTime.UtcNow.Ticks / 10000L;

            // Converte o valor de timestamp (long) para um array de bytes.
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            // Verifica se a arquitetura do sistema é "little-endian" (onde os bytes menos significativos vêm primeiro).
            // Se for "little-endian", inverte a ordem dos bytes de timestampBytes para "big-endian".
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            // Cria um array de bytes de tamanho 16 para armazenar o resultado final do GUID.
            byte[] guidBytes = new byte[16];

            // Copia os 10 bytes aleatórios para o início do array guidBytes.
            Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);

            // Copia os últimos 6 bytes do timestampBytes (ignorando os dois primeiros) para os últimos 6 bytes do array guidBytes.
            Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);

            // Cria um novo GUID a partir do array guidBytes e o retorna.
            return new Guid(guidBytes);
        }
    }

    public interface IBaseDTO
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }

    public class BaseDTO : IBaseDTO
    {        
        [MinLength(3, ErrorMessage = "O campo código deve conter 3 caracteres ou mais.")]
        [MaxLength(10, ErrorMessage = "O campo código deve conter até 10 caracteres ou menos.")]
        [DisplayName("Código")]
        public virtual string Codigo { get; set; }

        [MinLength(3, ErrorMessage = "O campo descrição deve conter 3 caracteres ou mais.")]
        [MaxLength(50, ErrorMessage = "O campo descrição deve conter até 50 caracteres ou menos.")]
        [DisplayName("Descrição")]
        public virtual string Descricao { get; set; }

        public string ObterCodigoComDescricao() => $"{Codigo} {Descricao}";
    }
}