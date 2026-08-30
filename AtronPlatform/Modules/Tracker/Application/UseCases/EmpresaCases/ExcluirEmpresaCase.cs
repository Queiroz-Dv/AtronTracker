using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.EmpresaCases
{
    public sealed class ExcluirEmpresaCase(IEmpresaRepository repository)
    {
        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var empresa = await repository.ObterPorCodigoAsync(codigo, rastrear: true);
            if (empresa is null)
                return Resultado.Falha(string.Format(
                    NotificacoesPadronizadas.Erro_RegistroComDescricaoNaoEncontrado,
                    codigo));

            if (!await repository.RemoverAsync(empresa))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.Mensagem_RegistroRemovido,
                    codigo));
        }
    }
}