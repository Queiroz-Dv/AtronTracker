using Shared.Application.Records;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace Shared.Domain
{
    public sealed class Regra<T, TProp>
    {
        public PropriedadeRecord<T, TProp> Propriedade { get; }

        public RegraValidacoesRecord<T, TProp> Validacoes { get; }

        public CondicaoRecord<T>? Condicao { get; internal set; }

        public string? MensagemErro { get; internal set; }

        internal Regra(Func<T, TProp> propriedade, string nomePropriedade)
        {
            Propriedade = new(propriedade, nomePropriedade);
            Validacoes = new();
        }

        internal NotificationMessage? Executar(T entidade)
        {
            if (Condicao is not null &&
                !Condicao.Valor(entidade))
            {
                return null;
            }

            TProp valor;

            try
            {
                valor = Propriedade.Valor(entidade);
            }
            catch
            {
                return CriarMensagem($"Não foi possível obter o valor do campo {Propriedade.NomePropriedade}.");
            }

            foreach (var validacao in Validacoes.PorValor)
            {
                try
                {
                    if (!validacao(valor))
                        return CriarMensagem();
                }
                catch
                {
                    return CriarMensagem($"Não foi possível validar o campo {Propriedade.NomePropriedade}.");
                }
            }

            foreach (var validacao in Validacoes.ComEntidade)
            {
                try
                {
                    if (!validacao(entidade, valor))
                        return CriarMensagem();
                }
                catch
                {
                    return CriarMensagem($"Não foi possível validar o campo {Propriedade.NomePropriedade}.");
                }
            }

            return null;
        }

        private NotificationMessage CriarMensagem(string? mensagem = null)
            => new()
            {
                Descricao =
                    mensagem
                    ?? MensagemErro
                    ?? $"O campo {Propriedade.NomePropriedade} é inválido.",

                Nivel = ENotificationType.Error
            };       
    }
}