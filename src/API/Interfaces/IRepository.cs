using API.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetById(string id, string collection);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate, string collection);

        Task Add(T entity, string collection);
        Task Update(T entity, string id, string collection);
        Task Remove(string id, string collection);

        Task<List<T>> GetAll(string collection);
        Task<List<T>> GetWhere(Expression<Func<T, bool>> predicate, string collection);

        Task<bool> Exists(string id, string collection);
    }
}
