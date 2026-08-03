using Domain.Entities;
using Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Linq;

namespace Application.Validations
{
    public class PerfilDeAcessoMessageValidation : Notifiable, IMessageBaseService, IValidateModelService<PerfilDeAcesso>
    {
        public void Validate(PerfilDeAcesso entity)
        {
            if (string.IsNullOrEmpty(entity.Descricao) ||
                 string.IsNullOrEmpty(entity.Codigo))
            {
                AdicionarErro(PerfilDeAcessoResource.Erro_DadosObrigatorios);
            }

            if (entity.Codigo.Length > 10)
            {
                AdicionarErro(PerfilDeAcessoResource.Erro_CodigoLongo);
            }

            if (entity.Codigo.Length < 3)
            {
                AdicionarErro(PerfilDeAcessoResource.Erro_CodigoPequeno);
            }

            if (entity.Descricao.Length < 3)
            {
                AdicionarErro(PerfilDeAcessoResource.Erro_DescricaoPequena);
            }

            if (entity.Descricao.Length > 50)
            {
                AdicionarErro(PerfilDeAcessoResource.Erro_DescricaoLonga);
            }

            if (!entity.PerfilDeAcessoModulos.Any())
            {
                AdicionarErro(PerfilDeAcessoResource.Erro_SemModulos);
            }
        }
    }
}
