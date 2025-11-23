namespace Shared.Application.DTOS.Common
{
    public class ModuloDTO
    {
        public string Codigo { get; set; } // "DPT", "CRG"...
        public string Nome { get; set; }   // Nome do módulo, ex: "Departamentos"

        // Campos visuais (apenas se quiser deixá-los dinâmicos via banco)
        public string Icone { get; set; }  // Ex: "business"
        public string Rota { get; set; }   // Ex: "/atron/departamentos"
        public int Colunas { get; set; } = 1;
        public int Linhas { get; set; } = 1;
    }
}