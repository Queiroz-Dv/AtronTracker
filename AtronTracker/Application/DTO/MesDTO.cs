using System.Collections.Generic;

namespace Application.DTO
{
    public class MesDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public static List<MesDTO> Meses()
        {
            return new List<MesDTO>
            {
                new() { Id = 1, Descricao = "Janeiro" },
                new() { Id = 2, Descricao = "Fevereiro" },
                new() { Id = 3, Descricao = "Março" },
                new() { Id = 4, Descricao = "Abril" },
                new() { Id = 5, Descricao = "Maio" },
                new() { Id = 6, Descricao = "Junho" },
                new() { Id = 7, Descricao = "Julho" },
                new() { Id = 8, Descricao = "Agosto" },
                new() { Id = 9, Descricao = "Setembro" },
                new() { Id = 10, Descricao = "Outubro" },
                new() { Id = 11, Descricao = "Novembro" },
                new() { Id = 12, Descricao = "Dezembro" }
            };
        }
    }
}