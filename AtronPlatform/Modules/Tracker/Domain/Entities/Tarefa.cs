using System;

using System.Collections.Generic;
using Domain.Enums;
using Domain.Extensions;

namespace Domain.Entities
{
    public class Tarefa : EntityBase
    {
        private const int EstadoPendenteAprovacaoId = 2;
        private const int EstadoIniciadaId = 5;

        public DestinoInicialTarefa DestinoInicial { get; set; }

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

        public ICollection<TarefaMovimentacao> Movimentacoes { get; set; }
        
        public void AprovarObtencao(int usuarioId, string usuarioCodigo)
        {
            UsuarioId = usuarioId;
            UsuarioCodigo = usuarioCodigo;
            DestinoInicial = DestinoInicialTarefa.Usuario;
            this.RemoverDepartamento();
            this.RemoverCargo();

            if (TarefaEstadoId != EstadoPendenteAprovacaoId)
            {
                return;
            }

            TarefaEstadoId = EstadoIniciadaId;
            ExigeAprovacaoParaObter = false;
        }
    }
}
