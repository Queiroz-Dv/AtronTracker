using Application.DTO;
using Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;

namespace Application.Validacoes
{
    public sealed class WorkspaceValidacoes : Validador<WorkspaceDTO>
    {
        public WorkspaceValidacoes()
        {
            RegrasParaCodigo();
            RegraParaDescricao();
            RegraParaResponsavel();
        }

        private void RegrasParaCodigo()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .TamanhoMaiorQue(10)
                .ComMensagem(WorkspaceResource.Erro_TamanhoCodigoWorkspace);
        }

        private void RegraParaDescricao()
        {
            RegraPara(x => x.Descricao)
                .NaoVazio()
                .TamanhoMaiorQue(50)
                .ComMensagem(WorkspaceResource.Erro_TamanhoDescricaoWorkspace);
        }

        private void RegraParaResponsavel()
        {
            RegraPara(work => work.Responsavel).NaoVazio();
            RegraPara(work => work.Responsavel.Id).NaoVazio();
            RegraPara(work => work.Responsavel.Codigo).NaoVazio();
            RegraPara(work => work.Responsavel.Email).NaoVazio();
        }
    }
}