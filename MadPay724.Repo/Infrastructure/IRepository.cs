using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Repo.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity,bool>> where);


        TEntity GetById(object id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            string includeEntity);
        TEntity Get(Expression<Func<TEntity, bool>> where);

        //--------------------------------------------

        Task InsertAsync(TEntity entity);

        Task<TEntity> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetManyAsync(
            Expression<Func<TEntity, bool>> filter,

            Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderBy,

            string includeEntity
            );

        Task<IEnumerable<TEntity>> GetManyAsyncPaging(
            Expression<Func<TEntity, bool>> filter,

            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,

            string includeEntity,
            int count,
            int firstCount,
            int page
        );

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);
    }
}
