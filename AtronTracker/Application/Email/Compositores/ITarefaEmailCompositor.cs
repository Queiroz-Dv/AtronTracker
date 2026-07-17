using Application.DTO;
using Domain.Entities;
using Shared.Application.DTOS.Requests;

namespace Application.Email.Compositores;

public interface ITarefaEmailCompositor
{
    EmailRequest ComporAtribuicao(TarefaDTO tarefa, Usuario usuario);
}
