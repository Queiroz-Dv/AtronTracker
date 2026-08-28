using Domain.Entities;
using Domain.Enums;

namespace Domain.Extensions;

public static class SolicitacaoEmpresaDecisaoExtensions
{
    public static void Aprovar(this SolicitacaoEmpresa solicitacao)
        => solicitacao.Status = StatusSolicitacaoEmpresa.Aprovada;

    public static void Recusar(this SolicitacaoEmpresa solicitacao)
        => solicitacao.Status = StatusSolicitacaoEmpresa.Recusada;

    public static UsuarioEmpresa CriarMembro(this SolicitacaoEmpresa solicitacao)
        => new()
        {
            EmpresaId = solicitacao.EmpresaId,
            Empresa = solicitacao.Empresa,
            UsuarioId = solicitacao.UsuarioId,
            UsuarioCodigo = solicitacao.UsuarioCodigo,
            Usuario = solicitacao.Usuario,
            Papel = PapelUsuarioEmpresa.Membro,
            Status = StatusUsuarioEmpresa.Ativo
        };
}
