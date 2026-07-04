namespace WebApi.Helpers
{
    public static class ModuloPolicies
    {
        public const string Prefixo = "Modulo:";
        public const string AcaoAcessar = "Acessar";

        public const string Departamento = "Modulo:DPT";
        public const string Cargo = "Modulo:CRG";
        public const string Usuario = "Modulo:USR";
        public const string Tarefa = "Modulo:TAR";
        public const string PerfilDeAcesso = "Modulo:PERF";
        public const string RelacionamentoPerfilUsuario = "Modulo:RPERFUSR";
        public const string PlanejamentoCustos = "Modulo:PLC";

        public static string Montar(string codigoModulo, string acao = null)
        {
            return string.IsNullOrWhiteSpace(acao)
                ? $"{Prefixo}{codigoModulo}"
                : $"{Prefixo}{codigoModulo}:{acao}";
        }
    }
}
