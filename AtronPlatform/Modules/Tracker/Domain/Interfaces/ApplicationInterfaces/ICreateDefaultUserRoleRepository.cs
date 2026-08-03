namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface ICreateDefaultUserRoleRepository
    {
        // Usuários padrão
        void CreateDefaultUsers();

        // Perfis de acesso
        void CreateDefaultRoles();
    }
}