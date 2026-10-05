using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions.RegraExtensions;
using System;

namespace Application.Validacoes
{
    public class UsuarioValidador : Validador<Usuario>
    {
        private readonly IAccessorService _accessorService;

        public UsuarioValidador(IAccessorService accessorService)
        {
            _accessorService = accessorService;

            RegraPara(x => x.Codigo)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroCodigoNulo);
            RegraPara(x => x.Codigo)
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(UsuarioResource.ErroCodigoLongo);
            RegraPara(x => x.Codigo)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroCodigoPequeno);

            RegraPara(x => x.Nome)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroNomeUsuarioNulo);
            RegraPara(x => x.Nome)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroNomePequeno);
            RegraPara(x => x.Nome)
                .TamanhoMenorOuIgualA(25)
                .ComMensagem(UsuarioResource.ErroNomeLongo);

            RegraPara(x => x.Sobrenome)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroSobrenomeObrigatorio);
            RegraPara(x => x.Sobrenome)
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(UsuarioResource.ErroSobrenomePequeno);
            RegraPara(x => x.Sobrenome)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(UsuarioResource.ErroSobrenomeLongo);

            RegraPara(x => x.DataNascimento)
                .DeveSer(data => data != DateTime.Now)
                .ComMensagem(UsuarioResource.ErroDataDeNascimento);

            RegraPara(x => x.Email)
                .NaoVazio()
                .ComMensagem(UsuarioResource.ErroEmailNulo);

            RegraPara(x => x.Email)
                .DeveSer(email => 
                {
                    if (string.IsNullOrEmpty(email)) return true;
                    var repo = _accessorService.ObterService<IUsuarioRepository>();
                    return !repo.VerificarEmailExistenteAsync(email).Result;
                })
                .ComMensagem(EmailResource.ErroEmailUtilizado);
        }
    }
}
