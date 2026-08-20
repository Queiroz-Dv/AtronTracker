using Application.DTO;
using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;

namespace Application.Email.Compositores;

public interface ITarefaEmailCompositor
{
    Resultado<EmailRequest> ComporAtribuicao(TarefaDTO tarefa, UsuarioDTO usuario);
}
