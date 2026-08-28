using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using Application.DTO.Request;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Application.Validador
{
    public sealed class EmpresaCadastroValidador : IValidador<EmpresaCadastroRequest>
    {
        public IEnumerable<NotificationMessage> Validar(EmpresaCadastroRequest? request)
        {
            var mensagens = new NotificationBag();
            if (request is null)
            {
                mensagens.AdicionarErro(EmpresaResource.Erro_RegistroNulo);
                return mensagens.Messages;
            }

            ValidarCampo(request.Codigo, 25, EmpresaResource.Campo_Codigo, mensagens);
            ValidarCampo(request.NomeFantasia, 150, EmpresaResource.Campo_NomeFantasia, mensagens);
            ValidarCampo(request.Endereco?.Logradouro, 200, EmpresaResource.Campo_Endereco, mensagens);
            ValidarCampo(request.Numero, 20, EmpresaResource.Campo_Telefone, mensagens);
            ValidarCampo(request.Email, 254, EmpresaResource.Campo_Email, mensagens);

            if (!string.IsNullOrWhiteSpace(request.Email)
                && (!MailAddress.TryCreate(request.Email, out var email) || email.Address != request.Email))
                mensagens.AdicionarErro(EmpresaResource.Erro_EmailInvalido);

            return mensagens.Messages;
        }

        public IEnumerable<NotificationMessage> ValidarResponsavel(Usuario? usuario)
        {
            var mensagens = new NotificationBag();
            if (usuario is null || usuario.Id <= 0 || string.IsNullOrWhiteSpace(usuario.Codigo)
                || usuario.Inativo || !usuario.EmailConfirmado)
                mensagens.AdicionarErro(EmpresaResource.Erro_ResponsavelInvalido);
            return mensagens.Messages;
        }

        public IEnumerable<NotificationMessage> ValidarConclusao(Empresa empresa)
        {
            var mensagens = new NotificationBag();
            if (empresa.Status != StatusEmpresa.Pendente || empresa.Usuarios.Count != 0)
                mensagens.AdicionarErro(EmpresaResource.Erro_CadastroConcluido);
            return mensagens.Messages;
        }

        private static void ValidarCampo(string? valor, int limite, string campo, NotificationBag mensagens)
        {
            if (string.IsNullOrWhiteSpace(valor))
                mensagens.AdicionarErro(string.Format(EmpresaResource.Erro_CampoObrigatorio, campo));
            else if (valor.Length > limite)
                mensagens.AdicionarErro(string.Format(EmpresaResource.Erro_CampoLongo, campo, limite));
        }
    }
}
