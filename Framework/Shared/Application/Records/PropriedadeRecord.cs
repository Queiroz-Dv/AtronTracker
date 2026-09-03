namespace Shared.Application.Records
{
    public record PropriedadeRecord<T, TProp>(Func<T, TProp> Valor, string NomePropriedade);
}