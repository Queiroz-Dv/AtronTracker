﻿using Atron.Application.DTO;
using Shared.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class CargoModel
    {
        public CargoModel()
        {
            Cargos = new List<CargoDTO>();
            Departamentos = new List<DepartamentoDTO>();
        }

        public CargoDTO Cargo { get; set; }

        public List<CargoDTO> Cargos { get; set; }

        public List<DepartamentoDTO> Departamentos { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public PageInfoDTO PageInfo { get; set; }
    }
}