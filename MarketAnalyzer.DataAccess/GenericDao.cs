using MarketAnalyzer.Core;
using MarketAnalyzer.Data;
using MarketAnalyzer.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MarketAnalyzer.DataAccess
{
    public abstract class GenericDao
    {
        public ICache Cache { get; set; }

        public MarketAnalyzerDB2Context CreateContext()
        {
            return new MarketAnalyzerDB2Context();
        }
    }


    public class GenericDao<TEntity> : GenericDao
  
        where TEntity : class, IEntity
    {
        public GenericDao()
        {
            this.Cache = new DotNetCache();
        }

       

        protected string GetAllKey
        {
            get
            {
                return "BaseService_GetAll_" + typeof(TEntity).Name;
            }
        }

        public virtual TEntity Add(TEntity entity)
        {
            var context = CreateContext();
            if (entity.Id == Guid.Empty) throw new OperationCanceledException("All entities must have an id defined befored being stored in the database");
            context.Entry(entity).State = EntityState.Added;

            if (entity is ICreatedTime)
            {
                var createdTime = (ICreatedTime)entity;
                createdTime.DateCreated = DateTime.UtcNow;
            }
            if (entity is IUpdatedTime)
            {
                var updatedTime = (IUpdatedTime)entity;
                updatedTime.DateUpdated = DateTime.UtcNow;
            }

            return entity;
        }

        public virtual TEntity AddSync(TEntity entity)
        {
            using (var context = CreateContext())
            {
                if (entity.Id == Guid.Empty) throw new OperationCanceledException("All entities must have an id defined befored being stored in the database");
                if (entity.Id == Guid.Empty) throw new OperationCanceledException("All entities must have an id defined befored being stored in the database");
                context.Entry(entity).State = EntityState.Added;

                if (entity is ICreatedTime)
                {
                    var createdTime = (ICreatedTime)entity;
                    createdTime.DateCreated = DateTime.UtcNow;
                }
                if (entity is IUpdatedTime)
                {
                    var updatedTime = (IUpdatedTime)entity;
                    updatedTime.DateUpdated = DateTime.UtcNow;
                }
                context.SaveChanges();
                return entity;
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            using (var context = CreateContext())
            {
                if (entity.Id == Guid.Empty) throw new OperationCanceledException("All entities must have an id defined befored being stored in the database");
                context.Entry(entity).State = EntityState.Added;

                if (entity is ICreatedTime)
                {
                    var createdTime = (ICreatedTime)entity;
                    createdTime.DateCreated = DateTime.UtcNow;
                }
                if (entity is IUpdatedTime)
                {
                    var updatedTime = (IUpdatedTime)entity;
                    updatedTime.DateUpdated = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();
                return entity;
            }
        }

        public virtual void Delete(TEntity entity)
        {
            var context = CreateContext();
            context.Entry(entity).State = EntityState.Deleted;
        }

        public virtual void Delete(IList<TEntity> entity)
        {
            var context = CreateContext();
            foreach (var item in entity)
            {
                context.Entry(item).State = EntityState.Deleted;
            }
        }

        public virtual void DeleteSync(TEntity entity)
        {
            using (var context = CreateContext())
            {
                Delete(entity);
                context.SaveChanges();
            }
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            using (var context = CreateContext())
            {
                Delete(entity);
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteListOfEntitiesAsync(IList<TEntity> entity)
        {
            using (var context = CreateContext())
            {
                Delete(entity);
                await context.SaveChangesAsync();
            }
        }

        public virtual TEntity GetSync(Guid id)
        {
            using (var context = CreateContext())
            {
                return context.Set<TEntity>().SingleOrDefault(entity => (entity).Id == id);
            }
        }

        

        public virtual IList<TEntity> GetSync(IList<Guid> ids)
        {
            using (var context = CreateContext())
            {
                return context.Set<TEntity>().Where(e => ids.Contains(e.Id)).ToList();
            }
        }

        

        public virtual IList<TEntity> GetAllSync()
        {
            var context = CreateContext();
            return (from c in context.Set<TEntity>()
                    select c).Distinct().ToList();
        }

        public virtual IList<TEntity> GetAllSync(bool useCache = true)
        {

            IList<TEntity> values = null;
            var key = GetAllKey;

            if (useCache)
            {
                values = this.GetFromCache<IList<TEntity>>(key);
                if (values != null) return values;
            }

            using (var context = CreateContext())
            {
                values = this.GetAllSync();

                if (values != null)
                    this.SetInCache(key, values);

                return values;
            }
        }

       /* protected virtual IList<TEntity> GetAllSync(bool useCache = true)
        {
            IList<TEntity> values = null;
            var key = GetAllKey;

            if (useCache)
            {
                values = this.GetFromCache<IList<TEntity>>(key);
                if (values != null) return values;
            }

            using (var context = CreateContext())
            {
                values = this.GetAllSync(context);

                if (values != null)
                    this.SetInCache(key, values);

                return values;
            }
        }*/

        public async virtual Task<IList<TEntity>> GetAllAsync()
        {
            var context = CreateContext();
            return await (from c in context.Set<TEntity>()
                            select c).Distinct().ToListAsync();
            
        }

        /*    return query;
        //}
        //public virtual async Task<IList<TEntity>> GetAllAsync(bool useCache = true)
        public virtual async Task<IList<TEntity>> GetAllAsync()
        {
            var context = CreateContext();
            IList<TEntity> values = null;
            var key = GetAllKey;

            /* validar se existe cache, se não não pode ir buscar à cache
            if (this.Cache == null)
            {
                useCache = false;
            }

            if (useCache)
            {
                values = this.GetFromCache<IList<TEntity>>(key);
                if (values != null) return values;
            }*/

            /*using (context)
            {
                values = await this.GetAllAsync();

                //if (values != null)
                 //   this.SetInCache(key, values);

                return values;
            }
        }*/

        public virtual async Task<Company> gettest(Guid id)
        {
            using (var context = new MarketAnalyzerDB2Context())
            {
                return await context.Companies.FindAsync(id);
            }
        }

        public virtual async Task<TEntity> GetAsync(Guid id)
        {
            using (var context = CreateContext())
            {
                return await context.Set<TEntity>().SingleOrDefaultAsync(entity => (entity).Id == id);
            }
        }

      

        public virtual async Task<IList<TEntity>> GetAsync(IList<Guid> ids)
        {
            using (var context = CreateContext())
            {
                return await context.Set<TEntity>().Where(e => ids.Contains(e.Id)).ToListAsync();
            }
        }

    

        public virtual async Task<TEntity> GetFirstByAsync(Expression<Func<TEntity, bool>> whereClause)
        {
            using (var context = CreateContext())
            {
                return await context.Set<TEntity>().Where(whereClause).FirstOrDefaultAsync();
            }
        }

      

        public virtual IList<TEntity> GetListBySync(Expression<Func<TEntity, bool>> whereClause)
        {
            using (var context = CreateContext())
            {
                return context.Set<TEntity>().Where(whereClause).ToList();
            }
        }

      

        public virtual async Task<IList<TEntity>> GetListByAsync(Expression<Func<TEntity, bool>> whereClause)
        {
            using (var context = CreateContext())
            {
                try
                {
                    return await context.Set<TEntity>().Where(whereClause).ToListAsync();
                }
                catch (Exception) { return null; }
            }
        }

        

        public virtual TEntity GetSingleBySync(Expression<Func<TEntity, bool>> whereClause)
        {
            using (var context = CreateContext())
            {
                return context.Set<TEntity>().Where(whereClause).SingleOrDefault();
            }
        }

       

        public virtual async Task<TEntity> GetSingleByAsync(Expression<Func<TEntity, bool>> whereClause)
        {
            using (var context = CreateContext())
            {
                return await context.Set<TEntity>().Where(whereClause).SingleOrDefaultAsync();
            }
        }

        public void SetCache(ICache cache)
        {
            this.Cache = cache;
        }

        public virtual TEntity Update(TEntity entity)
        {
            var context = CreateContext();
            context.Entry(entity).State = EntityState.Modified;

            if (entity is IUpdatedTime)
            {
                var informationTime = (IUpdatedTime)entity;
                informationTime.DateUpdated = DateTime.UtcNow;
            }

            return entity;
        }

        public virtual TEntity UpdateSync(TEntity entity)
        {
            using (var context = CreateContext())
            {
                var updatedEntity = Update(entity);
                context.SaveChanges();
                return updatedEntity;
            }
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            using (var context = CreateContext())
            {
                context.Entry(entity).State = EntityState.Modified;

                if (entity is IUpdatedTime)
                {
                    var informationTime = (IUpdatedTime)entity;
                    informationTime.DateUpdated = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();
                return entity;
            }
        }

        public virtual async Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> listOfEntity)
        {
            using (var context = CreateContext())
            {
                context.UpdateRange(listOfEntity);
                await context.SaveChangesAsync();
                
                return listOfEntity;
            }
        }

        public virtual async Task<IList<TEntity>> AddRangeAsync(IList<TEntity> listOfEntity)
        {
            using (var context = CreateContext())
            {
                context.AddRange(listOfEntity);
                await context.SaveChangesAsync();

                return listOfEntity;
            }
        }

        public virtual async Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> listOfEntity, int batchSize)
        {
            using (var context = CreateContext())
            {
                for (int i = 0; i < listOfEntity.Count; i += batchSize)
                {
                    context.UpdateRange(listOfEntity.Skip(i).Take(batchSize));
                    await context.SaveChangesAsync();
                }

                return listOfEntity;
            }
        }

        public T GetFromCache<T>(string key)
        {
            if (this.Cache == null)
                throw new InvalidOperationException("No cache exists.");

            return this.Cache.Get<T>(key);
        }

        public void InvalidateInCache(string key)
        {
            if (this.Cache != null)
                this.Cache.Invalidate(key);
        }

        public void SetInCache(string key, object value)
        {
            if (Cache != null)
                this.Cache.Set(key, value);
        }
    }
}