using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class TarefaMessageValidation : MessageModel, IMessages, IValidateModel<Tarefa>
    {
        public  void Validate(Tarefa entity)
        {
            if (entity.UsuarioId == 0)
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

            if (entity.Titulo.Length > 50)
            {
                AddError("O título da tarefa informada é inválido. Quantidade de caracteres maior que 50 digítos, tente novamente.");
            }

            if (entity.Conteudo.Length > 2500)
            {
                AddError("O título da tarefa informada é inválido. Quantidade de caracteres maior que 50 digítos, tente novamente.");
            }

            if (entity.DataInicial > entity.DataFinal)
            {
                AddError("Data inicial da tarefa é maior que a data final. Tente novamente.");
            }
        }
    }
}