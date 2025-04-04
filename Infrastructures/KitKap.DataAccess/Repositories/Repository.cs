using Kitkap.Entity.Repositories;
using KitKap.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly KitKapDbContext _context;
        private DbSet<T> _dbSet;

        public Repository(KitKapDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        
        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter=null)
        {
            IQueryable<T> query = _dbSet;
            if(filter != null)
            {
                query = _dbSet.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            if(filter != null)
                query = query.Where(filter);
            if(orderby != null)
                query = orderby(query);
            foreach (var table in includes)
            {
                query = query.Include(table);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

		public async Task<T> GetByIdAsync(long id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

       
    }
}
