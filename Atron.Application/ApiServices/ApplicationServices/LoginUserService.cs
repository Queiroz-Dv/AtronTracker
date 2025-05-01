using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces;
using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IApplicationTokenService _tokenService;
        private readonly ILoginApplicationRepository _loginApplication;
        private readonly IUsuarioService _usuarioService;
        private readonly IPerfilDeAcessoService _perfilDeAcessoService;
        private readonly MessageModel _messageModel;

        public LoginUserService(
            ILoginApplicationRepository loginApplication,
            IApplicationTokenService tokenService,
            IUsuarioService usuarioService,
            IPerfilDeAcessoService perfilDeAcessoService,
            MessageModel messageModel)
        {
            _loginApplication = loginApplication;
            _tokenService = tokenService;
            _usuarioService = usuarioService;
            _perfilDeAcessoService = perfilDeAcessoService;
            _messageModel = messageModel;
        }

        public async Task<LoginDTO> Authenticate(LoginRequestDTO loginRequest)
        {
            var loginDTO = new LoginDTO()
            {
                Authenticated = false
            };

            var login = new ApiLogin()
            {
                UserName = loginRequest.CodigoDoUsuario,
                Password = loginRequest.Senha
            };

            var result = await _loginApplication.AuthenticateUserLoginAsync(login);

            loginDTO.Authenticated = result;

            if (loginDTO.Authenticated)
            {
                // Get user data
                var user = await _usuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);

                // Dados adicionais para as claims do usuário
                loginDTO.DadosDoUsuario = new DadosDoUsuario()
                {
                    NomeDoUsuario = user.Nome,
                    CodigoDoUsuario = user.Codigo,
                    Email = user.Email,
                    CodigoDoCargo = user.CargoCodigo,
                    CodigoDoDepartamento = user.DepartamentoCodigo,
                    Expiracao = DateTime.Now.AddHours(24),
                    CodigosPerfis = new List<string>(),
                    ModulosCodigo = new List<string>(),
                };

                var perfisAssociados = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(user.Codigo);

                foreach (var perf in perfisAssociados)
                {
                    loginDTO.DadosDoUsuario.CodigosPerfis.Add(perf.Codigo);

                    foreach (var mod in perf.Modulos)
                    {
                        // Check if the module code is not already in the ModulosCodigo collection
                        if (!loginDTO.DadosDoUsuario.ModulosCodigo.Contains(mod.Codigo))
                        {
                            loginDTO.DadosDoUsuario.ModulosCodigo.Add(mod.Codigo);
                        }
                    }
                }

                // Generate token
                var userToken = _tokenService.GenerateToken(loginDTO.DadosDoUsuario);

                // Set token
                loginDTO.UserToken = userToken;
            }
            else
            {
                _messageModel.AddError("Usuário ou senha inválidos");
            }

            return loginDTO;
        }

        public async Task Logout()
        {
            await _loginApplication.Logout();
        }
    }
}