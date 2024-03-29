using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.DataAcess.Repositories
{
    public class MainRepository<T> : IMainRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;
        public MainRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public T Add(T entity)
        {
            dbSet.Add(entity);
            Save();
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync(); 
            return entity;
        }

        public bool Exist(int id)
        {
            var item = dbSet.Find(id);
            return item != null;
        }

        public async Task<bool> ExistAsync(int id)
        {
            var item = await dbSet.FindAsync(id);
            return item != null;
        }

        public T Find(Expression<Func<T, bool>> critera, string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if(includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.FirstOrDefault(critera);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> critera, string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.Where(critera).ToList();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> critera, string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(critera).ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> critera, string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(critera);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public T GetById(int id)
        {
            return dbSet.Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);

        }

        public bool Remove(T entity)
        {
            dbSet.Remove(entity);
            return Save() > 0;
        }

        public T Update(T entity)
        {
            dbSet.Update(entity);
            Save();
            return entity;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
             dbSet.RemoveRange(entity);
            Save();
        }
    }
}
