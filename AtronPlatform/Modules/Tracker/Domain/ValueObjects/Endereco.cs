#nullable enable

namespace Domain.ValueObjects
{
    public sealed record Endereco
    {
        public string Logradouro { get; init; } = string.Empty;
    }
}
