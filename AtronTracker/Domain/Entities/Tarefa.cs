using System;

using System.Collections.Generic;

namespace Domain.Entities
{
    public class Tarefa : EntityBase
    {
        public int? Identificador { get; set; }

        public int DestinoInicial { get; set; }

        public bool ExigeAprovacaoParaObter { get; set; }

        public int? UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }

        public int? DepartamentoId { get; set; }

        public string DepartamentoCodigo { get; set; }

        public int? CargoId { get; set; }

        public string CargoCodigo { get; set; }

        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int TarefaEstadoId { get; set; }

        public TarefaEstado EstadoDaTarefa { get; set; }

        public Usuario Usuario { get; set; }

        public Departamento Departamento { get; set; }

        public Cargo Cargo { get; set; }

        public ICollection<SolicitacaoObtencaoTarefa> SolicitacoesObtencao { get; set; }
    }
}
