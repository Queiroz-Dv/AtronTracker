namespace ExternalServices.Interfaces
{
    public interface IExternalService<DTO>
    {
        Task<List<DTO>> ObterTodos();

        Task<List<T>> ObterTodos<T>();
        Task<DTO> ObterPorCodigo(string codigo);
        Task<DTO> ObterPorId(string id);
        Task Criar(DTO dto);
        Task Atualizar(string codigo, DTO dto);
        Task AtualizarPorId(string id, DTO dto);
        Task Remover(string codigo);
    }
}