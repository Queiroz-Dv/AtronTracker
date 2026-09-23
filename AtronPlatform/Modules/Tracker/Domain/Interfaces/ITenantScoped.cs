namespace Domain.Interfaces
{
    public interface ITenantScoped
    {
        int Id { get; }
        string? Codigo { get; }
        string ModuloCodigo { get; }
    }
}
