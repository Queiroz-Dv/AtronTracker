using System;
using System.Linq;

namespace Atron.Application.DTO
{
    public class Factory
    {
        public int GerarIdentificador()
        {
            // Gera um GUID
            Guid id = Guid.NewGuid();

            // Obtém os caracteres do guid
            string guid = id.ToString("N");

            // Obtém apenas os dígitos (números)
            string digitsOfId = new string(guid.Where(char.IsDigit).ToArray());

            // Separa do 0 até o 6
            digitsOfId = digitsOfId.Substring(0, 6);

            // Converte para int 32
            var newId = Convert.ToInt32(digitsOfId);           

            return newId;
        }               
    }
}