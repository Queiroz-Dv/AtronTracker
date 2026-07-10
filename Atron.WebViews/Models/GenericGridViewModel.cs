using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class GenericGridViewModel
    {
        public string LegendTitle { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool HasChildren { get; set; }
        public bool IsChildrenMultiSelect { get; set; }
        public List<string> EntityColumns { get; set; }
        public List<string> ChildrenColumns { get; set; }
        public List<GridItem> Entities { get; set; }
    }

    public class GridItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Dictionary<string, string> Values { get; set; } = new();
        public List<GridItem> Children { get; set; } = new();
    }

}
