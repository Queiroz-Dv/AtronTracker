using Shared.DTO;

namespace Atron.WebViews.Models
{
    public class DefaultModel<DTO>
    {
        public PageInfoDTO<DTO> PageInfo { get; set; }
    }
}