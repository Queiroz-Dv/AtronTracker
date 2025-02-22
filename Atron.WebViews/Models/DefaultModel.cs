using Shared.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public abstract class DefaultModel<DTO>
    {
        public List<DTO> Entities { get; set; }
        public PageInfoDTO PageInfo { get; set; }
    }
}