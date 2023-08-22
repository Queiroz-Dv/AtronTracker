using System;

namespace DAL.DAO
{
    public class Context : QRZStaticDataContext
    {
        private QRZDatabaseDataContext _db;

        public Context()
        {
            _db = new QRZDatabaseDataContext();
        }

        public QRZDatabaseDataContext GetContext()
        {
            return _db;
        }
    }
}