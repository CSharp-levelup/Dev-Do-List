using System.Linq.Expressions;
using DevDoListServer.Data;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Repositories
{
    public abstract class GenericRepository<T>(AppDbContext context) : IRepository<T>
        where T : class
    {
        protected AppDbContext context = context;

        public virtual async Task<T> Create(T entity)
        {
            var addedEntity = context.Add(entity).Entity;
            await SaveChanges();
            return addedEntity;
        }

        public virtual async Task<int> Delete(T entity)
        {
            context.Remove(entity);
            return await SaveChanges();
        }

        public virtual async Task<List<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await context.FindAsync<T>(id);
        }

        public virtual async Task<T> Update(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await SaveChanges();
            return entity;
        }

        public virtual async Task<bool> Exists(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }

        public virtual async Task<T> FindSingle(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().SingleAsync(predicate);
        }

        public virtual async Task<ICollection<T>> FindAll(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().Where(predicate).ToListAsync();
        }

        protected async Task<int> SaveChanges()
        {
            return await context.SaveChangesAsync();
        }


    }
}
