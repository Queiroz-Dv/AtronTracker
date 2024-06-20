using System;
using System.Linq;

namespace Atron.Application.DTO
{
    public class Factory
    {
        public int GerarIdentificador()
        {
            Guid id = Guid.NewGuid();

            string guid = id.ToString("N");

            string digitsOfId = new string(guid.Where(char.IsDigit).ToArray());
            digitsOfId = digitsOfId.Substring(0, 6);

            var newId = Convert.ToInt32(digitsOfId);           

            return newId;
        }               
    }
}