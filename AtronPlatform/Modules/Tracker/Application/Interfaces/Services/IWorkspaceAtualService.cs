namespace Application.Interfaces.Services;

public interface IWorkspaceAtualService
{
    void Definir(int workspaceId);
    int? ObterId();
    void Remover();
}
