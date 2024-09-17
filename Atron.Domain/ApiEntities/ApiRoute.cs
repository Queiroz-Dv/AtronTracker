using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atron.Domain.ApiEntities
{
    [Serializable]
    public class ApiRoute
    {        
        [DisplayName("Módulo")]
        public string Modulo { get; set; } // Exemplo: Departamento

        [DisplayName("URL")]
        [NotMapped]
        public string Url { get; set; } // Essa URL é fixa e será obtida do Json então não precisa gravar no banco

        [DisplayName("Ação")]
        public ApiRouteAction Acao { get; set; } // São os verbos comumente utilizados na API

        public string AcaoDescricao { get; set; }
    }

    [Serializable]
    public enum ApiRouteAction
    {
        [Description("GET")]
        Get,

        [Description("POST")]
        Post,

        [Description("PUT")]
        Put,

        [Description("DELETE")]
        Delete
    }
}