using Application.Interfaces.Services;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.EntitiesServices.PerfisDeAcesso
{
    public class PerfilDeAcessoCacheInvalidator : IPerfilDeAcessoCacheInvalidator
    {
        private readonly ICacheUsuarioService _cacheUsuarioService;

        public PerfilDeAcessoCacheInvalidator(ICacheUsuarioService cacheUsuarioService)
        {
            _cacheUsuarioService = cacheUsuarioService;
        }

        public void InvalidarUsuariosDoPerfil(PerfilDeAcesso perfil)
        {
            InvalidarUsuarios(perfil?.PerfisDeAcessoUsuario?
                .Select(relacionamento => relacionamento.UsuarioCodigo ?? relacionamento.Usuario?.Codigo) ?? []);
        }

        public void InvalidarUsuarios(IEnumerable<string> codigosUsuarios)
        {
            foreach (var codigoUsuario in codigosUsuarios
                         .Where(codigo => !string.IsNullOrEmpty(codigo))
                         .Distinct())
            {
                _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(codigoUsuario);
            }
        }
    }
}
