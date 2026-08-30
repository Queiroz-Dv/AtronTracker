using Application.Interfaces.Services;
using Domain.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.AuthServices
{
    public class ConfirmacaoEmailCodigoService : IConfirmacaoEmailCodigoService
    {
        private const int MenorCodigo = 0;
        private const int MaiorCodigoExclusivo = 1000000;

        private static string GerarIdentificador()
            => RandomNumberGenerator.GetInt32(MenorCodigo, MaiorCodigoExclusivo).ToString("D6");

        public string GerarHash(string valor)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(valor)));

        public (ConfirmacaoEmail ConfirmacaoEmail, string Identificador) CriarDadosConfirmacao(string usuarioCodigo, int validadeEmHoras)
        {
            var identificador = GerarIdentificador();
            return (CriarConfirmacao(usuarioCodigo, identificador, validadeEmHoras), identificador);
        }

        private ConfirmacaoEmail CriarConfirmacao(string usuarioCodigo, string identificador, int validadeEmHoras)
        {
            var agora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            return new ConfirmacaoEmail
            {
                UsuarioCodigo = usuarioCodigo,
                IdentificadorHash = GerarHash(identificador),
                CriadoEm = agora,
                ExpiraEm = agora.AddHours(validadeEmHoras)
            };
        }

        public bool ConfirmacaoValida(ConfirmacaoEmail confirmacaoEmail, string usuarioCodigo, string identificador)
        {
            if (confirmacaoEmail is null)
            {
                return false;
            }

            return confirmacaoEmail.UsuarioCodigo == usuarioCodigo
                && confirmacaoEmail.IdentificadorHash == GerarHash(identificador);
        }
    }
}
