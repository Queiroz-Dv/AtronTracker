using Application.Interfaces.Services;
using Application.Records.Email;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class CriarConfirmacaoEmailCase(
        IEnderecoFrontendService EnderecoFrontendService,
        IConfirmacaoEmailCodigoService ConfirmacaoCodigoService,
        IConfirmacaoEmailRepository ConfirmacaoRepository)
    {
        private const int ValidadeConfirmacaoEmHoras = 24;

        private readonly IEnderecoFrontendService _enderecoFrontendService = EnderecoFrontendService;
        private readonly IConfirmacaoEmailCodigoService _confirmacaoCodigoService = ConfirmacaoCodigoService;
        private readonly IConfirmacaoEmailRepository _confirmacaoRepository = ConfirmacaoRepository;

        public async Task<Resultado<ConfirmacaoRecord>> ExecutarAsync(string codigo)
        {
            var (ConfirmacaoEmail, Identificador) = _confirmacaoCodigoService.CriarDadosConfirmacao(codigo, ValidadeConfirmacaoEmHoras);
            var gravado = await _confirmacaoRepository.GravarOuSubstituirAsync(ConfirmacaoEmail);
            var uriBase = _enderecoFrontendService.ObterUriBase();

            if (gravado)
            {
                var confirmacao = new ConfirmacaoRecord
                {
                    Link = $"{uriBase}/confirmar-email?usuarioCodigo={codigo}",
                    Identificador = Identificador
                };

                return Resultado<ConfirmacaoRecord>.Sucesso(confirmacao);
            }

            return Resultado<ConfirmacaoRecord>.Falha("Não foi possível criar a confirmação de email.");
        }
    }
}