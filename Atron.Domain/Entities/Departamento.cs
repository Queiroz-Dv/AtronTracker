using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public sealed class Departamento : EntityBase
    {
        public string CodigoDepartamento { get; private set; }
        public string DescricaoDepartamento { get; private set; }

        //public void AtualizarDescricao(string novaDescricao)
        //{
        //    DescricaoDepartamento = novaDescricao;
        //}

        // Passar para outra classe depois 
        public static IList<Departamento> ObterDepartamentosFake()
        {
            var departamentos = new List<Departamento>() {
                        new Departamento() { Id = 1234, CodigoDepartamento = "QRZ", DescricaoDepartamento = "Descricao QRZ" },
                        new Departamento() { Id = 1235, CodigoDepartamento = "ABC", DescricaoDepartamento = "Descricao ABC" },
                        new Departamento() { Id = 1236, CodigoDepartamento = "DEF", DescricaoDepartamento = "Descricao DEF" },
                        new Departamento() { Id = 1237, CodigoDepartamento = "GHI", DescricaoDepartamento = "Descricao GHI" },
                        new Departamento() { Id = 1238, CodigoDepartamento = "JKL", DescricaoDepartamento = "Descricao JKL" },
                        new Departamento() { Id = 1239, CodigoDepartamento = "MNO", DescricaoDepartamento = "Descricao MNO" },
                        new Departamento() { Id = 1240, CodigoDepartamento = "PQR", DescricaoDepartamento = "Descricao PQR" },
                        new Departamento() { Id = 1241, CodigoDepartamento = "STU", DescricaoDepartamento = "Descricao STU" },
                        new Departamento() { Id = 1242, CodigoDepartamento = "VWX", DescricaoDepartamento = "Descricao VWX" },
                        new Departamento() { Id = 1243, CodigoDepartamento = "YZA", DescricaoDepartamento = "Descricao YZA" }
                    };

            return departamentos;
        }
    }
}