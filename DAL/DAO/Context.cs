using System;

namespace DAL.DAO
{
    public class Context : QRZStaticDataContext, IDisposable
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

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}