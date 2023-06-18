using DAL.Generics;

namespace DAL.Interfaces
{
    public interface IPositionRepository<T> : IGenericRepository<T> where T : POSITION
    {
    }
}
