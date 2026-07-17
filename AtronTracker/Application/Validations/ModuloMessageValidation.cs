using Domain.Entities;
using Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Application.Validations
{
    public class ModuloMessageValidation : Notifiable, IMessageBaseService, IValidateModelService<Modulo>
    {
        public void Validate(Modulo entity)
        {
            if (string.IsNullOrEmpty(entity.Descricao) ||
                string.IsNullOrEmpty(entity.Codigo))
            {
                AdicionarErro(ModuloResource.Erro_DadosObrigatorios);
            }

            if (entity.Codigo.Length > 10)
            {
                AdicionarErro(ModuloResource.Erro_CodigoLongo);
            }

            if (entity.Codigo.Length < 3)
            {
                AdicionarErro(ModuloResource.Erro_CodigoPequeno);
            }

            if (entity.Descricao.Length < 3)
            {
                AdicionarErro(ModuloResource.Erro_DescricaoPequena);
            }

            if (entity.Descricao.Length > 50)
            {
                AdicionarErro(ModuloResource.Erro_DescricaoLonga);
            }
        }
    }
}
