using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class SalarioMessageValidation : MessageModel, IMessages, IValidateModel<Salario>
    {
        public void Validate(Salario entity)
        {
            if (entity.UsuarioId <= 0)
            {
                AddError("Identificador de usuário é inválido.");
            }

            if (entity.UsuarioCodigo.Length < 3)
            {
                AddError("O código de usuário informado é inválido. Quantidade de caracteres menor que 3 digítos, tente novamente.");
            }

            if (entity.UsuarioCodigo.Length > 10)
            {
                AddError("O código de usuário informado é inválido. Quantidade de caracteres maior que 10 digítos, tente novamente.");
            }

            if (entity.SalarioMensal <= 0)
            {
                AddError("Salário mensal não pode ser negativo.");
            }

            if (entity.Ano.Length < 4)
            {
                AddError("O ano é inválido. Tamanho permitido de 4 caracteres.");
            }

            if (entity.Ano.Length > 4)
            {
                AddError("O ano é inválido. Tamanho máximo de 4 caracteres.");
            }

            if (entity.MesId <= 0 || entity.MesId > 12)
            {
                AddError("Identificador do mês inválido.");
            }
        }
    }
}
