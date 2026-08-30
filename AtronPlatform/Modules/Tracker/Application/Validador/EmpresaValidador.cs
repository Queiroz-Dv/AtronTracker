using System;
using System.Collections.Generic;
using System.Net.Mail;
using Application.DTO;
using Domain.Enums;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace Application.Validador
{
    public sealed class EmpresaValidador : IValidador<EmpresaDTO>
    {
        public IEnumerable<NotificationMessage> Validar(EmpresaDTO? empresa)
        {
            var notificacoes = new NotificationBag();
            if (empresa is null)
            {
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return notificacoes.Messages;
            }

            ValidarCampo(empresa.Codigo, 3, 25, nameof(empresa.Codigo), notificacoes);
            ValidarCampo(empresa.NomeFantasia, 3, 150, nameof(empresa.NomeFantasia), notificacoes);
            ValidarCampo(empresa.Endereco, 3, 200, nameof(empresa.Endereco), notificacoes);
            ValidarCampo(empresa.Numero, 1, 20, nameof(empresa.Numero), notificacoes);
            ValidarCampo(empresa.Email, 3, 254, nameof(empresa.Email), notificacoes);

            var emailInformado = empresa.Email?.Trim();
            if (!string.IsNullOrWhiteSpace(emailInformado)
                && (!MailAddress.TryCreate(emailInformado, out var email)
                    || !string.Equals(email.Address, emailInformado, StringComparison.OrdinalIgnoreCase)))
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroCampoInvalido);

            if (!Enum.IsDefined(typeof(StatusEmpresa), empresa.Status))
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroCampoInvalido);

            return notificacoes.Messages;
        }

        private static void ValidarCampo(
            string? valor,
            int tamanhoMinimo,
            int tamanhoMaximo,
            string nomeCampo,
            NotificationBag notificacoes)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                notificacoes.AdicionarErro(string.Format(
                    NotificacoesPadronizadas.ErroCampoObrigatorio,
                    nomeCampo));
                return;
            }

            var tamanho = valor.Trim().Length;
            if (tamanho < tamanhoMinimo || tamanho > tamanhoMaximo)
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroCampoInvalido);
        }
    }
}
