using Domain.Entities;
using Application.Resources;
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
                AdicionarErro(TarefaResource.Erro_IdentificadorUsuarioInvalido);
            }

            if (entity.UsuarioCodigo.Length < 3)
            {
                AdicionarErro(string.Format(TarefaResource.Erro_CodigoTamanhoMinimo, TarefaResource.Campo_Usuario));
            }

            if (entity.UsuarioCodigo.Length > 10)
            {
                AdicionarErro(string.Format(TarefaResource.Erro_CodigoTamanhoMaximo, TarefaResource.Campo_Usuario));
            }

            if (entity.Titulo.Length > 50)
            {
                AdicionarErro(TarefaResource.Erro_TituloTamanhoMaximo);
            }

            if (entity.Conteudo.Length > 2500)
            {
                AdicionarErro(TarefaResource.Erro_ConteudoTamanhoMaximo);
            }

            if (entity.DataInicial > entity.DataFinal)
            {
                AdicionarErro(TarefaResource.Erro_PeriodoInvalido);
            }
        }
    }
}
