﻿namespace Atron.Domain.Entities
{
    public class Mes : EntityBase
    {        
        public string Descricao { get; set; }
        public Salario Salario { get; set; }
    }
}