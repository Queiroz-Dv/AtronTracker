<<<<<<< HEAD
﻿using Atron.Domain.Entities;
=======
﻿using System.Collections.Generic;
>>>>>>> 9c5d71f27b0cbf2d0952b4c244329a3ccae73954

namespace Atron.Domain.Componentes
{
    public class PropriedadesDeFluxo
    {
        public int Id { get; set; }

<<<<<<< HEAD
        public string Codigo { get; set; }       
=======
        public string Codigo { get; set; }

        public ICollection<PropriedadeDeFluxoModulo> PropriedadesDeFluxoModulo { get; set; }
>>>>>>> 9c5d71f27b0cbf2d0952b4c244329a3ccae73954
    }
}