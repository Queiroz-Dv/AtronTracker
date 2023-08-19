using PersonalTracking.Models;

namespace DAL.Interfaces.FactoryModules
{
    /// <summary>
    /// Interface que determina a conversão e criação dos objetos
    /// </summary>
    public interface IDepartmentFactory
    {
        /// <summary>
        /// Cria um objeto do tipo DAL.
        /// </summary>
        /// <param name="model">Um objeto model a ser convertido.</param>
        /// <returns>Um objeto DAL</returns>
        DEPARTMENT CreateModelToDalEntity(DepartmentModel model);

        /// <summary>
        /// Cria um objeto do tipo model.
        /// </summary>
        /// <param name="entity">O objeto DAL a ser convertido</param>
        /// <returns>Um objeto model.</returns>
        DepartmentModel CreateDalToModel(DEPARTMENT entity);

        /// <summary>
        /// Repassa os valores para uma nova model
        /// </summary>
        /// <param name="_entity">Model que é recebida</param>
        /// <returns>Um novo model verificado</returns>
        DepartmentModel SetDepartmentModelFactory(DepartmentModel _entity);

        DepartmentModel CreateDepartmentModelFactory();
    }
}