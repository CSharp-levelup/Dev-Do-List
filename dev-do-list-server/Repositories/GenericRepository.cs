﻿using DevDoListServer.Data;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Repositories
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        protected AppDbContext context;

        public GenericRepository(AppDbContext context)
        {
            this.context = context;
        }

        public virtual async Task<T> Create(T entity)
        {
            var addedEntity = context.Add(entity).Entity;
            await saveChanges();
            return addedEntity;
        }

        public virtual async Task<int> Delete(T entity)
        {
            context.Remove(entity);
            return await saveChanges();
        }

        public virtual async Task<List<T>> GetAll()
        {
            var list = await context.Set<T>().ToListAsync();
            return list;
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await context.FindAsync<T>(id);
        }

        public virtual async Task<T> Update(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await saveChanges();
            return entity;
        }

        protected async Task<int> saveChanges()
        {
            return await context.SaveChangesAsync();
        }


    }
}
