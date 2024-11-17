using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;

namespace MedSoft.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Fields
        private readonly ApplicationDbContext _db;
        internal DbSet<T> _dbSet;
        #endregion

        #region Constructor
        public Repository(ApplicationDbContext db)
        {
            _db = db;       
            _dbSet = _db.Set<T>();
        }
        #endregion

        #region Methods
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false)
        { 
            IQueryable<T> query = (tracked) ? _dbSet : _dbSet.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.
                    Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                } 
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var includeProperty in includeProperties.
                    Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        #endregion
    }
}
