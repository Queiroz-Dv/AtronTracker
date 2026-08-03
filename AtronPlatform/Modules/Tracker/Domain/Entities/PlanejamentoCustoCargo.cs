using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities
{
    public sealed class PlanejamentoCustoCargo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(PlanejamentoCustoId))]
        [NotNull]
        public int PlanejamentoCustoId { get; set; }

        [ForeignKey(nameof(PlanejamentoCustoCodigo))]
        [NotNull]
        [MaxLength(10)]
        public string PlanejamentoCustoCodigo { get; set; }

        public PlanejamentoCusto PlanejamentoCusto { get; set; }

        [ForeignKey(nameof(CargoId))]
        [NotNull]
        public int CargoId { get; set; }

        [ForeignKey(nameof(CargoCodigo))]
        [NotNull]
        [MaxLength(10)]
        public string CargoCodigo { get; set; }

        public Cargo Cargo { get; set; }

        public bool Detalhado { get; set; }

        public decimal? ValorMinimo { get; set; }

        public decimal? ValorTeto { get; set; }
    }
}
