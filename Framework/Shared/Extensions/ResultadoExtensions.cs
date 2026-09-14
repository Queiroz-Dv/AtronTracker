using Shared.Domain.ValueObjects;

namespace Shared.Extensions
{
    public static class ResultadoExtensions
    {
        public static Resultado CommMensagemRegistroSalvo(this Resultado resultado, string registro)
        {
            resultado.MensagemRegistroSalvo(registro);
            return resultado;
        }

        public static Resultado ComMensagemFalhaNaCriacao(this Resultado resultado, string registro)
        {
            resultado.MesagemFalhaNaGravacao(registro);
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

        public static Resultado ComMensagemRegistroExistente(this Resultado resultado, string codigoRegistro)
        {
            resultado.MensagemRegistroExistente(codigoRegistro);
            return resultado;
        }
    }
}