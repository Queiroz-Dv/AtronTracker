using PersonalTracking.DAL.DataAcess;

namespace PersonalTracking.DAL.DAO
{
    public class Context
    {
        private const string connectionString = @"Data Source=MRJ01-DSK238\SQLEXPRESS;Initial Catalog=PERSONALTRACKING;Integrated Security=True";

        private static PersonalTrackingDBDataContext db;

        public static PersonalTrackingDBDataContext CreateContext()
        {
            if (db== null)
            {
                db = new PersonalTrackingDBDataContext(connectionString);
            }

            return db;
        }

        public static PersonalTrackingDBDataContext GetContext()
        {
            return CreateContext();
        }

        public static PersonalTrackingDBDataContext DB
        {
            get
            {
                return CreateContext();
            }
        }
    }
}