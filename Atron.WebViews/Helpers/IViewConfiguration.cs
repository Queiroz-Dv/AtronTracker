namespace Atron.WebViews.Helpers
{
    public interface IViewConfiguration
    {
        public abstract void ConfigureViewBagCurrentController();
        public abstract void ConfigureViewDataFilter();
        public abstract void CreateTempDataMessages();
        public abstract void ConfigureDataTitleForView(string title);
        public abstract void ConfigureCurrentPageAction();
    }
}