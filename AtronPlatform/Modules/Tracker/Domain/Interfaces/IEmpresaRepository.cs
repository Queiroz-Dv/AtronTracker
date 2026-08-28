#nullable enable

using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<Usuario?> ObterUsuarioAsync(string codigo);
        Task<bool> CodigoExisteAsync(string codigo);
        Task<UsuarioEmpresa?> ObterVinculoAsync(int usuarioId, string usuarioCodigo);
        Task CriarAsync(Empresa empresa);
        Task<IReadOnlyList<Empresa>> BuscarAtivasAsync(string? termo);
        Task<Empresa?> ObterAtivaAsync(int id);
        Task<SolicitacaoEmpresa?> ObterSolicitacaoPendenteAsync(int usuarioId, string usuarioCodigo, int empresaId);
        Task CriarSolicitacaoAsync(SolicitacaoEmpresa solicitacao);
        Task<UsuarioEmpresa?> ObterResponsavelAsync(int empresaId);
        Task<IReadOnlyList<SolicitacaoEmpresa>> ObterSolicitacoesPendentesAsync(int empresaId);
        Task<SolicitacaoEmpresa?> ObterSolicitacaoPendenteAsync(int solicitacaoId, int empresaId);
        Task AprovarSolicitacaoAsync(SolicitacaoEmpresa solicitacao, UsuarioEmpresa vinculo);
        Task AtualizarSolicitacaoAsync(SolicitacaoEmpresa solicitacao);
    }
}

