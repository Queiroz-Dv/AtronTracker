using AtronStock.Domain.Enums;

namespace AtronStock.Application.DTO.Request
{
    public class CategoriaRequest
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public EStatus Status { get; set; }

        public CategoriaRequest()
        {
            Status = EStatus.Ativo;
        }
    }
}