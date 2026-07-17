using Application.DTO;
using Application.Email.Compositores;
using Application.Interfaces.Services;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaNotificacaoService : ITarefaNotificacaoService
    {
        private readonly IEmailService _emailService;
        private readonly ITarefaEmailCompositor _emailCompositor;

        public TarefaNotificacaoService(IEmailService emailService, ITarefaEmailCompositor emailCompositor)
        {
            _emailService = emailService;
            _emailCompositor = emailCompositor;
        }

        public async Task<Resultado> NotificarAtribuicaoAsync(TarefaDTO tarefa, Usuario usuario)
        {
            if (usuario is null)
            {
                return Resultado.Sucesso();
            }

            if (!usuario.ReceberNotificacaoTarefaPorEmail || usuario.Email.IsNullOrEmpty())
            {
                return Resultado.Sucesso();
            }

            var mensagem = _emailCompositor.ComporAtribuicao(tarefa, usuario);

            return await _emailService.EnviarAsync(mensagem);
        }

    }
}
