namespace Shared.Application.Records
{
    public record RegraValidacoesRecord<T, TProp>
    {
        public List<Func<TProp, bool>> PorValor { get; } = [];

        public List<Func<T, TProp, bool>> ComEntidade { get; } = [];
    }
}