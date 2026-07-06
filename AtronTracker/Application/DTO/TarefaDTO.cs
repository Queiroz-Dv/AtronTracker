using System;

namespace Application.DTO
{
    public class TarefaDTO
    {
        public int Id { get; set; }

        public int? Identificador { get; set; }

        public int DestinoInicial { get; set; }

        public bool ExigeAprovacaoParaObter { get; set; }

        public string UsuarioCodigo { get; set; }

        public string DepartamentoCodigo { get; set; }

        public string CargoCodigo { get; set; }

        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public UsuarioDTO Usuario { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public CargoDTO Cargo { get; set; }

        public TarefaEstadoDTO EstadoDaTarefa { get; set; }
    }
}
