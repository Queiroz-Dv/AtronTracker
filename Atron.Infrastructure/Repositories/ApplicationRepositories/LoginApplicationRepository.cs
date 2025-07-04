using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Models.ApplicationModels;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginApplicationRepository : ILoginApplicationRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private AtronDbContext _context;

        public LoginApplicationRepository(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            AtronDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }       

        public async Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity)
        {
            var usuario = await _userManager.FindByNameAsync(usuarioIdentity.Codigo);
            if (usuario != null)
            {                
                usuario.RefreshToken = usuarioIdentity.RefreshToken;
                usuario.RefreshTokenExpireTime = usuarioIdentity.RefreshTokenExpireTime;

                IdentityResult result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded && !usuarioIdentity.Senha.IsNullOrEmpty())
                {
                    var autenticado = await _signInManager.PasswordSignInAsync(usuarioIdentity.Codigo, usuarioIdentity.Senha, true, false);
                    return autenticado.Succeeded;
                }

                return result.Succeeded;
            }

            return false;
        }

        public async Task<bool> AuthenticateUserLoginAsync(ApiLogin login)
        {
            var authenticated = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.Remember, false);

            return authenticated.Succeeded;
        }

        public async Task<bool> ConfigPasswordAsync(string codigoDoUsuario, ApiLogin model)
        {
            var usr = await _userManager.FindByNameAsync(codigoDoUsuario);
            if (usr != null)
            {
                var result = await _userManager.RemovePasswordAsync(usr);

                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(usr, model.Password);
                    return result.Succeeded;
                }
            }

            return false;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ObterRefreshTokenDoUsuario(string refreshToken)
        {
            return await _context.Users.AnyAsync(rf => rf.RefreshToken == refreshToken);
        }

        public void RemoverInfoTokenDeUsuario(string codigoUsuario)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.UserName == codigoUsuario);
            if (usuario != null)
            {
                usuario.RefreshToken = null;
                usuario.RefreshTokenExpireTime = default;
                _context.Users.Update(usuario);
                _context.SaveChanges();
            }
        }
    }
}