using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaNotificacaoService : ITarefaNotificacaoService
    {
        private readonly IEmailService _emailService;

        public TarefaNotificacaoService(IEmailService emailService)
        {
            _emailService = emailService;
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

            var mensagem = new EmailRequest
            {
                EmailsDestino = [usuario.Email],
                Assunto = $"Nova tarefa atribuida: {tarefa.Titulo}",
                Mensagem = GerarCorpoEmailTarefa(tarefa, usuario)
            };

            return await _emailService.EnviarAsync(mensagem);
        }

        private static string GerarCorpoEmailTarefa(TarefaDTO tarefa, Usuario usuario)
        {
            var nomeUsuario = WebUtility.HtmlEncode($"{usuario.Nome} {usuario.Sobrenome}".Trim());
            var titulo = WebUtility.HtmlEncode(tarefa.Titulo);
            var conteudo = WebUtility.HtmlEncode(tarefa.Conteudo ?? string.Empty);
            var estado = WebUtility.HtmlEncode(ObterDescricaoEstado(tarefa));

            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                            .container {{ max-width: 640px; margin: 0 auto; background-color: #ffffff; padding: 28px; border-radius: 8px; }}
                            .header {{ border-bottom: 2px solid #007bff; padding-bottom: 16px; }}
                            .header h1 {{ color: #007bff; margin: 0; font-size: 22px; }}
                            .content {{ padding: 18px 0; color: #333; line-height: 1.5; }}
                            .task-box {{ background-color: #f8f9fa; border-left: 4px solid #007bff; padding: 14px; margin-top: 14px; }}
                            .task-box p {{ margin: 6px 0; }}
                            .footer {{ border-top: 1px solid #eee; padding-top: 14px; color: #666; font-size: 12px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Nova tarefa atribuida</h1>
                            </div>
                            <div class='content'>
                                <p>Ola, <strong>{nomeUsuario}</strong>.</p>
                                <p>Uma nova tarefa foi atribuida a você.</p>
                                <div class='task-box'>
                                    <p><strong>Titulo:</strong> {titulo}</p>
                                    <p><strong>Conteudo:</strong> {conteudo}</p>
                                    <p><strong>Data inicial:</strong> {tarefa.DataInicial:dd/MM/yyyy}</p>
                                    <p><strong>Data final:</strong> {tarefa.DataFinal:dd/MM/yyyy}</p>
                                    <p><strong>Estado inicial:</strong> {estado}</p>
                                </div>
                            </div>
                            <div class='footer'>
                                <p>Este e um e-mail automático do Sistema Atron.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        private static string ObterDescricaoEstado(TarefaDTO tarefa)
        {
            if (tarefa.EstadoDaTarefa is not null && !tarefa.EstadoDaTarefa.Descricao.IsNullOrEmpty())
            {
                return tarefa.EstadoDaTarefa.Descricao;
            }

            return "Nao informado";
        }
    }
}
