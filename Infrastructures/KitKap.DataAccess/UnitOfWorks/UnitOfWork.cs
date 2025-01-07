using Kitkap.Entity.Repositories;
using Kitkap.Entity.UnitOfWorks;
using KitKap.DataAccess.Contexts;
using KitKap.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.DataAccess.UnitOfWorks
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly KitKapDbContext _context;
        private bool disposed = false;

        public UnitOfWork(KitKapDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            return new Repository<T>(_context);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);      //GC - Garbage Collector
        }
    }
}
