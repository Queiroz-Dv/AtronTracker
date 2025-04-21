using System.Security.Claims;

namespace Shared.DTO.API
{
    public static class ClaimCode
    {
        // Padrão para o funcionamento do JWT
        public const string CODIGO_DEPARTAMENTO = "CodigoDepartamento";
        public const string CODIGO_CARGO = "CodigoCargo";
        public const string CODIGO_USUARIO = "CodigoUsuario";
        public const string NOME_USUARIO = "NomeUsuario";
        public const string EMAIL_USUARIO = "EmailUsuario";

        // Adicionais
        public const string CODIGO_UNIDADE = "CodigoUnidade";
        public const string NOME_UNIDADE = "NomeUnidade";
        public const string CODIGO_EMPRESA = "CodigoEmpresa";
        public const string NOME_EMPRESA = "NomeEmpresa";
    }
}