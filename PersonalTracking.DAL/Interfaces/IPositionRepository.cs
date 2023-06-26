using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.Generics;

namespace PersonalTracking.DAL.Interfaces
{
    public interface IPositionRepository<T> : IGenericRepository<T> where T : POSITION
    {
    }
}
