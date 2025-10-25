using Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class CargoModel : DefaultModel<CargoDTO>
    {
        public CargoModel()
        {
            Entities = new List<CargoDTO>();
        }

        public CargoDTO Cargo { get; set; }
    }
}