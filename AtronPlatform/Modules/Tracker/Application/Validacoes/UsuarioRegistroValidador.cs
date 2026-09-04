using Application.DTO.Request;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions;
using Shared.Extensions.RegraExtensions;
using System;
using System.Text.RegularExpressions;

namespace Application.Validacoes
{
    public class UsuarioRegistroValidacoes : Validador<UsuarioRegistroRequest>
    {
        public UsuarioRegistroValidacoes()
        {
            RegrasParaCodigo();
            RegrasParaNome();
            RegrasParaSobrenome();
            RegrasParaDataNascimento();
            RegrasParaEmail();
            RegrasParaSenha();
        }

        private void RegrasParaCodigo()
        {
            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroCodigoNulo);

            RegraPara(x => x.Codigo)
                .TamanhoEntre(3, 10)
                .ComMensagem(UsuarioResource.Erro_TamanhoCodigo);
        }

        private void RegrasParaNome()
        {
            RegraPara(x => x.Nome)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroNomeUsuarioNulo);

            RegraPara(x => x.Nome)
                .TamanhoEntre(3, 25)
                .ComMensagem(UsuarioResource.Erro_TamanhoNome);
        }

        private void RegrasParaSobrenome()
        {
            RegraPara(x => x.Sobrenome)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroSobrenomeObrigatorio);

            RegraPara(x => x.Sobrenome)
                .TamanhoEntre(3, 50)
                .ComMensagem(UsuarioResource.Erro_TamanhoSobrenome);
        }

        private void RegrasParaDataNascimento()
        {
            RegraPara(x => x.DataNascimento)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroDataDeNascimento);

            RegraPara(x => x.DataNascimento)
                .Quando(x => x.DataNascimento >= DateOnly.FromDateTime(DateTime.Now))
                .ComMensagem(UsuarioResource.ErroDataDeNascimento);

            RegraPara(x => x.DataNascimento)
                .Quando(x => x.DataNascimento <= DateOnly.FromDateTime(DateTime.Now.AddYears(-18))) // Menor de idade
                .ComMensagem(UsuarioResource.ErroDataDeNascimento);
        }

        private void RegrasParaEmail()
        {
            RegraPara(x => x.Email)
                .NaoVazio()
                .EmailValido()
                .ComMensagem(UsuarioResource.ErroEmailNulo);

            RegraPara(x => x.Email)
                .TamanhoEntre(5, 100)
                .ComMensagem(UsuarioResource.Erro_TamanhoEmail);
        }

        private void RegrasParaSenha()
        {
            var regexComposicaoSenha = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{9,}$");
            var regexCaractereEspecial = new Regex(@"[!@#$%^&*(),.?""{}|<>]");

            RegraPara(x => x.Senha)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroSenhaNula);

            RegraPara(x => x.Senha)
                .TamanhoMenorQue(9)
                .ComMensagem(UsuarioResource.ErroSenhaTamanhoMinimo);

            RegraPara(x => x.Senha)
                .Quando(x => !regexComposicaoSenha.IsMatch(x.Senha))
                .ComMensagem(UsuarioResource.ErroSenhaComposicao);

            RegraPara(x => x.Senha)
                .Quando(x => !regexCaractereEspecial.IsMatch(x.Senha))
                .ComMensagem(UsuarioResource.ErroSenhaCaractereEspecial);

            RegraPara(x => x.ConfirmaSenha)
                .Quando(x => !x.Senha.IsEquals(x.ConfirmaSenha))
                .ComMensagem(UsuarioResource.ErroSenhasDiferentes);
        }
    }
}