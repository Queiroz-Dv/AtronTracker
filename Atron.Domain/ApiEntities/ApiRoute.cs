using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atron.Domain.ApiEntities
{
    [Serializable]
    public class ApiRoute
    {
        public int Id { get; set; }
        
        [DisplayName("Módulo")]
        public string Modulo { get; set; } // Exemplo: Departamento

        [DisplayName("URL")]
        [NotMapped]
        public string Url { get; set; } // Essa URL é fixa e será obtida do Json então não precisa gravar no banco

        [DisplayName("Método")]
        public ApiRouteAction Acao { get; set; } // São os verbos comumente utilizados na API

        [DisplayName("Ativa")]
        public bool Ativo { get; set; } // Utilizado pra saber se o endpoint está ativo ou não

        public string NomeDaRotaDeAcesso { get; set; } // Nome de acesso descrito na rota
    }

    [Serializable]
    public enum ApiRouteAction
    {
        [Description("Obter")]
        Get,

        [Description("Criar")]
        Post,

        [Description("Atualizar")]
        Put,

        [Description("Excluir")]
        Delete
    }
}