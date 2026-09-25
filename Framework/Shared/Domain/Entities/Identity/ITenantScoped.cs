namespace Shared.Domain.Entities.Identity
{
    public interface ITenantScoped
    {
        int Id { get; }
        string? Codigo { get; }
    }
}