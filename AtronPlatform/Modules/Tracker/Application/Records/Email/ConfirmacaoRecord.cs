namespace Application.Records.Email
{
    public record ConfirmacaoRecord
    {
        public string Link { get; init; }
        public string Identificador { get; set; }
    }
}