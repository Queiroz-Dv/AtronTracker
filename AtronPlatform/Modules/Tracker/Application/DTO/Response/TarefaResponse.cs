using System;

using Application.Records.Tarefa;

namespace Application.DTO.Response
{
    public class TarefaResponse
    {
        public int Id { get; set; }
        public bool ExigeAprovacaoParaObter { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public TarefaEstadoDTO EstadoDaTarefa { get; set; }
        public UsuarioRecord Usuario { get; set; }
    }
}
