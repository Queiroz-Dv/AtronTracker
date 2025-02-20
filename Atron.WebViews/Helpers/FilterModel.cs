namespace Atron.WebViews.Helpers
{
    public class FilterModel
    {
        // Filtrar por algum campo específico
        public string FilterBy { get; set; } = "";

        // Valor do filtro
        public string FilterValue { get; set; } = "";

        public FilterModel() { }

        public FilterModel(string filterBy, string filterValue)
        {
            FilterBy = filterBy;
            FilterValue = filterValue;
        }

        public int ItemPage { get; set; } = 1;
    }
}