using System;

namespace Application.DTO
{
    public class NotificacaoInternaDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Mensagem { get; set; }

        public string Modulo { get; set; }

        public string TipoEvento { get; set; }

        public string UrlDestino { get; set; }

        public int? TarefaId { get; set; }

        public bool Lida { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataLeitura { get; set; }
    }
}
