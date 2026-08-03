using Application.DTO;
using Domain.Entities;
using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;

namespace Application.Email.Compositores;

public interface ITarefaEmailCompositor
{
    Resultado<EmailRequest> ComporAtribuicao(TarefaDTO tarefa, Usuario usuario);
}
