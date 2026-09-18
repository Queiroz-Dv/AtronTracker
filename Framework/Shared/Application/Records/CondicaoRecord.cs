namespace Shared.Application.Records
{
    public record CondicaoRecord<T>(Func<T, bool> Valor);
}