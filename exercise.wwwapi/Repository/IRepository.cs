using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace exercise.wwwapi.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get();
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions);

        IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetWithIncludes(Func<IQueryable<T>, IQueryable<T>> includeQuery);
        T? GetById(int id, Func<IQueryable<T>, IQueryable<T>> includeQuery);
        T GetById(object id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
        void Delete(params object[] ids);
        void Save();
        DbSet<T> Table { get; }

    }
}
