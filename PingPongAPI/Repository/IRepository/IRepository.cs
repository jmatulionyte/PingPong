using System;
using System.Linq.Expressions;

namespace PingPongAPI.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
        //get specific T class by linq expression as param
        //linq expression(func) on the class 'T'
        //func need to be provided output result - bool (e.g. u=> u.Id = id - return bool)
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        //no tracking from EF
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null);

        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task SaveAsync();
    }
}

