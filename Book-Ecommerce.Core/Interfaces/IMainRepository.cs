using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Core.Interfaces
{
    public interface IMainRepository<T> where T : class
    {
        // Non Async
        T GetById(int id);
        IEnumerable<T> GetAll();
        T Add(T entity);
        T Update(T entity);
        bool Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        T Find (Expression<Func<T,bool>> critera,string[] includes=null);
        IEnumerable<T> FindAll (Expression<Func<T,bool>> critera,string[] includes=null);
        bool Exist(int id);
        // Async
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> FindAsync(Expression<Func<T, bool>> critera, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> critera, string[] includes = null);
        Task<bool> ExistAsync(int id);

    }
}
