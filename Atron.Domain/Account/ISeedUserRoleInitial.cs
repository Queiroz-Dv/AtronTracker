namespace Atron.Domain.Account
{
    public interface ISeedUserRoleInitial
    {
        // Usuários padrão
        void SeedUsers();

        // Perfis de acesso
        void SeedRoles();
    }
}