using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Application.Validations
{
    public class SalarioMessageValidation : Notifiable, IMessageBaseService, IValidateModelService<Salario>
    {
        public void Validate(Salario entity)
        {
            if (entity.UsuarioId <= 0)
            {
                AdicionarErro("Identificador de usuário é inválido.");
            }

            if (entity.UsuarioCodigo.Length < 3)
            {
                AdicionarErro("O código de usuário informado é inválido. Quantidade de caracteres menor que 3 digítos, tente novamente.");
            }

            if (entity.UsuarioCodigo.Length > 10)
            {
                AdicionarErro("O código de usuário informado é inválido. Quantidade de caracteres maior que 10 digítos, tente novamente.");
            }

            if (entity.SalarioMensal <= 0)
            {
                AdicionarErro("Salário mensal não pode ser negativo.");
            }

            if (entity.Ano.Length < 4)
            {
                AdicionarErro("O ano é inválido. Tamanho permitido de 4 caracteres.");
            }

            if (entity.Ano.Length > 4)
            {
                AdicionarErro("O ano é inválido. Tamanho máximo de 4 caracteres.");
            }

            if (entity.MesId <= 0 || entity.MesId > 12)
            {
                AdicionarErro("Identificador do mês inválido.");
            }
        }
    }
}
