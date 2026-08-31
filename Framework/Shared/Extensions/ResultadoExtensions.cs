using Shared.Domain.ValueObjects;

namespace Shared.Extensions
{
    public static class ResultadoExtensions
    {
        public static Resultado ComMensagemRegistroSalvo(this Resultado resultado, string mensagem)
        {
            resultado.MensagemRegistroSalvo(mensagem);
            return resultado;
        }

        public static Resultado ComMensagemRegistroAtualizado(this Resultado resultado, string codigoRegistro)
        {
            resultado.MensagemRegistroAtualizado(codigoRegistro);
            return resultado;
        }

        public static Resultado ComMensagemRegistroNaoEncontrado(this Resultado resultado, string codigoRegistro)
        {
            resultado.MensagemRegistroNaoEncontrado(codigoRegistro);
            return resultado;
        }
    }
}