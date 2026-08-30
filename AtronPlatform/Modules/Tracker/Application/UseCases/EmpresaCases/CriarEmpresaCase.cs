using System.Linq;
using System.Threading.Tasks;
using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases
{
    public sealed class CriarEmpresaCase(
        EmpresaMapping mapping,
        IEmpresaRepository repository,
        IValidador<EmpresaDTO> validador)
    {
        public async Task<Resultado<EmpresaDTO>> ExecutarAsync(EmpresaDTO empresaDTO)
        {
            var erros = validador.Validar(empresaDTO);
            if (erros.Any())
                return Resultado<EmpresaDTO>.Falhas(erros);

            var codigo = EmpresaMapping.NormalizarCodigo(empresaDTO.Codigo);
            if (await repository.CodigoExisteAsync(codigo))
                return Resultado<EmpresaDTO>.Falha(string.Format(
                    NotificacoesPadronizadas.Erro_RegistroComDescricaoExistente,
                    codigo));

            var empresa = mapping.MapToEntity(empresaDTO);
            if (!await repository.CriarAsync(empresa))
                return Resultado<EmpresaDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            return Resultado<EmpresaDTO>
                .Sucesso(mapping.MapToDto(empresa))
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.ResourceManager.GetString("Mensagem_RegistroSalvo")!,
                    empresa.Codigo));
        }
    }
}
