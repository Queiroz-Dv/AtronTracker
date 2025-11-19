using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Application.Validations
{
    public class TarefaMessageValidation : Notifiable, IMessageBaseService, IValidateModelService<Tarefa>
    {
        public void Validate(Tarefa entity)
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

            if (entity.Titulo.Length > 50)
            {
                AdicionarErro("O título da tarefa informada é inválido. Quantidade de caracteres maior que 50 digítos, tente novamente.");
            }

            if (entity.Conteudo.Length > 2500)
            {
                AdicionarErro("O título da tarefa informada é inválido. Quantidade de caracteres maior que 50 digítos, tente novamente.");
            }

            if (entity.DataInicial > entity.DataFinal)
            {
                AdicionarErro("Data inicial da tarefa é maior que a data final. Tente novamente.");
            }
        }
    }
}