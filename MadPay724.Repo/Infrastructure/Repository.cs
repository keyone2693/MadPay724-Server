using MadPay724.Data.Dtos.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.Utilities.Pagination;
using MadPay724.Common.Helpers.Utilities.Extensions;

namespace MadPay724.Repo.Infrastructure
{
    public abstract class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
    {
        #region ctor
        private readonly DbContext _db;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }
        #endregion


        #region normal 
        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }
        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("there is no entity");
            _dbSet.Update(entity);
        }
        public void Delete(object id)
        {
            var entity = GetById(id);
            if (entity == null)
                throw new ArgumentException("there is no entity");
            _dbSet.Remove(entity);
        }
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objs = _dbSet.Where(where).AsEnumerable();
            foreach (TEntity item in objs)
            {
                _dbSet.Remove(item);
            }
        }
        public TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsQueryable();
        }
        public PagedList<TEntity> GetAllPagedList(PaginationDto paginationDto,
            Expression<Func<TEntity, bool>> filter = null,
            string orderBy = "",
            string includeEntity = "")
        {
            IQueryable<TEntity> query;
            //filter
            if (filter != null)
                query = _dbSet.Where(filter);
            else
                query = _dbSet.AsQueryable();
            //include
            foreach (var includeentity in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeentity);
            }
            //orderby
            if (string.IsNullOrEmpty(orderBy) || string.IsNullOrWhiteSpace(orderBy))
            {
                if (orderBy.Split(',')[1] == "asc")
                {
                    query = query.OrderBy(orderBy.Split(',')[0]);
                }
                else if (orderBy.Split(',')[1] == "desc")
                {
                    query = query.OrderByDescending(orderBy.Split(',')[0]);
                }
                else
                {
                    query = query.OrderBy(orderBy.Split(',')[0]);
                }

            }
            return PagedList<TEntity>.Create(query, paginationDto.PageNumber, paginationDto.PageSize);
        }
        public IEnumerable<TEntity> GetMany(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeEntity = "", int count = 0)
        {
            //return _dbSet.Where(where).FirstOrDefault();

            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeentity in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeentity);
            }

            count = Math.Abs(count);
            if (count > 0)
                query = query.Take(count);

            if (orderBy != null)
            {
                return  orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }

        }
        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).FirstOrDefault();
        }

       public bool IsAny(Expression<Func<TEntity, bool>> filter)
        {
            if (_dbSet.Any(filter))
                return true;
            return false;

        }
        public async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> filter)
        {
            if (await _dbSet.AnyAsync(filter))
                return true;
            return false;
        }
        #endregion

        #region async
        public async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<PagedList<TEntity>> GetAllPagedListAsync(PaginationDto paginationDto,
            Expression<Func<TEntity, bool>> filter = null,
            string orderBy = "",
            string includeEntity = "")
        {
            IQueryable<TEntity> query;
            //filter
            if (filter != null)
                query = _dbSet.Where(filter);
            else
                query = _dbSet.AsQueryable();
            //include
            foreach (var includeentity in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeentity);
            }
            //orderby
            if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrWhiteSpace(orderBy))
            {
                if (orderBy.Split(',')[1] == "asc")
                {
                    query = query.OrderBy(orderBy.Split(',')[0]);
                }
                else if (orderBy.Split(',')[1] == "desc")
                {
                    query = query.OrderByDescending(orderBy.Split(',')[0]);
                }
                else
                {
                    query = query.OrderBy(orderBy.Split(',')[0]);
                }

            }
            //
            return await PagedList<TEntity>.CreateAsync(query,
                paginationDto.PageNumber, paginationDto.PageSize);
        }
        public async Task<IEnumerable<TEntity>> GetManyAsync(
      Expression<Func<TEntity, bool>> filter = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
      string includeEntity = "", int count = 0)
        {
            //return _dbSet.Where(where).FirstOrDefault();

            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeentity in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeentity);
            }

            count = Math.Abs(count);
            if (count > 0)
                query = query.Take(count);

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }

        }
        public async Task<long> GetCountAsync(
      Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();


        }
        public async Task<long> GetSumAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, long>> select = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (select == null)
            {
                return 0;
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            return await query.SumAsync(select);


        }
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.Where(where).FirstOrDefaultAsync();
        }

        #endregion

        #region dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<TEntity>> GetManyAsyncPaging(Expression<Func<TEntity,
            bool>> filter, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy,
            string includeEntity, int count, int firstCount, int page)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeentity in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeentity);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.Skip(firstCount).Skip(count * page).Take(count).ToListAsync();



        }

        ~Repository()
        {
            Dispose(false);
        }

        #endregion

    }
}
