using Atron.Application.DTO;
using Shared.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class CargoModel
    {
        public CargoDTO Cargo { get; set; }

        public IEnumerable<CargoDTO> Cargos { get; set; }

        public PageInfoDTO PageInfo { get; set; }
    }
}