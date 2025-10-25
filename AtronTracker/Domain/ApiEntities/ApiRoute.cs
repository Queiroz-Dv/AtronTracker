using System;

namespace Domain.ApiEntities
{
    [Serializable]
    public class ApiRoute
    {
        public string Url { get; set; } // Essa URL é fixa e será obtida do Json então não precisa gravar no banco        
    }
}