using exercise.wwwapi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace exercise.wwwapi.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {


        private DataContext _db;
        private DbSet<T> _table = null;
       
        public Repository(DataContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions)
        {
            if (includeExpressions.Any())
            {
                var set = includeExpressions
                    .Aggregate<Expression<Func<T, object>>, IQueryable<T>>
                     (_table, (current, expression) => current.Include(expression));
            }
            return _table.ToList();
        }
        public async Task<IEnumerable<T>> Get()
        {
            return _table.ToList();
        }
        public IEnumerable<T> GetAll()
        {
            return _table.ToList();
        }
        public IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> filter)
        {
            return _table.Where(filter).ToList();
        }

        public IEnumerable<T> GetWithIncludes(Func<IQueryable<T>, IQueryable<T>> includeQuery)
        {
            IQueryable<T> query = includeQuery(_table);
            return query.ToList();
        }
        public T GetById(object id)
        {
            return _table.Find(id);
        }

        public T? GetById(int id, Func<IQueryable<T>, IQueryable<T>> includeQuery)
        {
            IQueryable<T> query = _table.Where(e => EF.Property<int>(e, "Id") == id);
            query = includeQuery(query);
            return query.FirstOrDefault();
        }

        public void Insert(T obj)
        {
            _table.Add(obj);
        }
        public void Update(T obj)
        {
            _table.Attach(obj);
            _db.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            T existing = _table.Find(id);
            _table.Remove(existing);
        }


        public void Save()
        {
            _db.SaveChanges();
        }
        public DbSet<T> Table { get { return _table; } }

    }
}
