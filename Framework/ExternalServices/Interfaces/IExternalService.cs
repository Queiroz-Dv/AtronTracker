namespace ExternalServices.Interfaces
{
    public interface IExternalService<DTO>
    {
        Task<List<DTO>> ObterTodos();
        Task<DTO> ObterPorCodigo(string codigo);
        Task Criar(DTO dto);
        Task Atualizar(string codigo, DTO dto);
        Task Remover(string codigo);
    }
}