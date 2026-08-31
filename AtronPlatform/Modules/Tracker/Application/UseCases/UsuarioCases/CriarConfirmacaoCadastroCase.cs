using Application.Interfaces.Services;
using Application.Records.Usuario;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class CriarConfirmacaoCadastroCase(
    IConfirmacaoEmailRepository confirmacaoEmailRepository,
    IConfirmacaoEmailCodigoService confirmacaoEmailCodigoService,
    IEnderecoFrontendService enderecoFrontendService)
{
    private const int ValidadeConfirmacaoEmHoras = 24;

    public async Task<Resultado<ConfirmacaoCadastroCriadaRecord>> ExecutarAsync(string usuarioCodigo)
    {
        var dados = confirmacaoEmailCodigoService.CriarDadosConfirmacao(usuarioCodigo, ValidadeConfirmacaoEmHoras);

        if (!await confirmacaoEmailRepository.GravarOuSubstituirAsync(dados.ConfirmacaoEmail))
            return Resultado<ConfirmacaoCadastroCriadaRecord>.Falha(AuthResource.Erro_GerarCodigoConfirmacao);

        var uriBase = enderecoFrontendService.ObterUriBase();
        var confirmacao = new ConfirmacaoCadastroCriadaRecord(
            $"{uriBase}/confirmar-email?usuarioCodigo={usuarioCodigo}",
            dados.Identificador,
            ValidadeConfirmacaoEmHoras);

        return Resultado<ConfirmacaoCadastroCriadaRecord>.Sucesso(confirmacao);
    }
}
