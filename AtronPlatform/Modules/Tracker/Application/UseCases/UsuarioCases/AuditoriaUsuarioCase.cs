using Domain.Entities;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class AuditoriaUsuarioCase(IAuditoriaService auditoriaService)
    {
        private readonly IAuditoriaService _auditoriaService = auditoriaService;
        private const string UsuarioContexto = nameof(Usuario);

        public async Task  ExecutarAsync(Usuario usuario)
        {
            await _auditoriaService.RegistrarServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuario.Codigo,
                Contexto = UsuarioContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuario.Codigo,
                    Contexto = UsuarioContexto,
                    Descricao = $"Usuario {usuario.Codigo} criado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });
        }

        public async Task RegistrarAtualizacaoAsync(Usuario usuario)
        {
            await _auditoriaService.AtualizarServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuario.Codigo,
                Contexto = UsuarioContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuario.Codigo,
                    Contexto = UsuarioContexto,
                    Descricao = $"Usuário {usuario.Codigo} atualizado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });
        }
    }
}
