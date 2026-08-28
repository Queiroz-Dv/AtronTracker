using Domain.Entities;
using Domain.Enums;

namespace Domain.Extensions
{
    public static class EmpresaCadastroExtensions
    {
        public static UsuarioEmpresa ConcluirCadastro(this Empresa empresa, Usuario responsavel)
        {
            var vinculo = empresa.CriarResponsavelInicial(responsavel);
            empresa.Usuarios.Add(vinculo);
            empresa.Status = StatusEmpresa.Ativa;
            return vinculo;
        }
    }
}
