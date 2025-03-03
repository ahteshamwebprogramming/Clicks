using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IGenericRepository<T> where T : class
    {      
            void AddAsync(T entity);
            void AddRangeAsync(T entity);
            IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
            IEnumerable<T> GetAllAsync();
            T GetByIdAsync(int id);
            void Remove(T entity);
            void RemoveRange(IEnumerable<T> entities);

    }
}
