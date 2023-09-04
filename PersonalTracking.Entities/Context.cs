namespace PersonalTracking.Entities
{
    public class Context : QRZStaticDataContext
    {
        public Context() => db = new QRZDatabaseDataContext();

        public QRZDatabaseDataContext GetContext() => db;
    }
}