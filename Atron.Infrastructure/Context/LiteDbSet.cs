using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Context
{
    /// <summary>
    /// Interface para definir operações básicas de acesso a dados em um conjunto de dados genérico.
    /// </summary>
    /// <typeparam name="T"> Entidade que representa o tipo de dados no conjunto. Deve ser uma classe. </typeparam>
    public interface IDataSet<T>
    {
        /// <summary>
        /// Método para verificar se existe algum registro que atenda a um determinado critério.
        /// </summary>
        /// <param name="predicate"> Predicado que define a condição a ser verificada.</param>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, 
        /// contendo um valor booleano que indica se existe
        /// algum registro que atenda ao critério especificado.
        /// </returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Método para encontrar um único registro que atenda ao critério especificado.
        /// </summary>
        /// <param name="predicate">Predicado que define a condição de busca.</param>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, contendo a entidade encontrada ou null se não existir.
        /// </returns>
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);

        Task<IList<T>> FindAllAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Método para buscar um registro pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador único do registro.</param>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, contendo a entidade encontrada ou null se não existir.
        /// </returns>
        Task<T> FindByIdAsync(BsonValue id);
        /// <summary>
        /// Método para buscar todos os registros do conjunto.
        /// </summary>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, contendo uma coleção enumerável de todas as entidades.
        /// </returns>
        Task<IEnumerable<T>> FindAllAsync();

        /// <summary>
        /// Método para inserir uma nova entidade no conjunto.
        /// </summary>
        /// <param name="entity">Entidade a ser inserida.</param>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, contendo o identificador gerado para a entidade inserida.
        /// </returns>
        Task<BsonValue> InsertAsync(T entity);
        /// <summary>
        /// Método para atualizar uma entidade existente no conjunto.
        /// </summary>
        /// <param name="entity">Entidade com os dados atualizados.</param>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, contendo um valor booleano que indica se a atualização foi bem-sucedida.
        /// </returns>
        Task<bool> UpdateAsync(T entity);
        /// <summary>
        /// Método para excluir uma entidade do conjunto pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador único da entidade a ser excluída.</param>
        /// <returns>
        /// Retorna uma tarefa que representa a operação assíncrona, contendo um valor booleano que indica se a exclusão foi bem-sucedida.
        /// </returns>
        Task<bool> DeleteAsync(BsonValue id);
    }

    /// <summary>
    /// Conjunto de dados genérico para operações de acesso a dados usando LiteDB.
    /// </summary>
    /// <typeparam name="T"> Entidade que representa o tipo de dados no conjunto. Deve ser uma classe. </typeparam>
    public class LiteDbSet<T> : IDataSet<T>
    {
        private readonly ILiteCollection<T> _collection;

        public LiteDbSet(ILiteDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            var result = _collection.Find(predicate).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var exists = _collection.Exists(predicate);
            return Task.FromResult(exists);
        }
        
        public Task<IList<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            var results = _collection.Find(predicate).ToList();
            return Task.FromResult((IList<T>)results);
        }

        public Task<T> FindByIdAsync(BsonValue id) => Task.FromResult(_collection.FindById(id));
        public Task<IEnumerable<T>> FindAllAsync() => Task.FromResult(_collection.FindAll().AsEnumerable());
        public Task<BsonValue> InsertAsync(T entity) => Task.FromResult(_collection.Insert(entity));
        public Task<bool> UpdateAsync(T entity) => Task.FromResult(_collection.Update(entity));
        public Task<bool> DeleteAsync(BsonValue id) => Task.FromResult(_collection.Delete(id));

    }
}