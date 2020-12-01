using Movie_WebApi.Model;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Movie_WebApi.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity originalEntity { get; set; }
        TEntity Get(int id);
        Task<TEntity> GetAsync(int id);
        Task<TEntity> GetNoTrackingAsync(int id);
        TEntity GetNoTracking(int id);
        Task<List<TEntity>> GetAllAsync();

        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Without async will get object of entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity GetEntity(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAllQuery();

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        void AddAsync(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void UpdateAsync(TEntity entity);
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                              Expression<Func<TEntity, bool>> predicate = null,
                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                              Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                              bool disableTracking = true);
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        void UpdateGraph(TEntity entity);


        Task<IEnumerable<TEntity>> GetPagedSortedFilteredListAsync(int start, int length, string orderColumnName, ListSortDirection order, string searchValue);
        Task<int> GetRecordsTotalAsync();
        Task<int> GetRecordsFilteredAsync(string searchValue);
        string GetSearchPropertyName();

        bool Exists(Func<TEntity, bool> conditions);
        Task<List<DyanmicModel>> GetSPDataByEntity(string SPName, string parameter);

        DataTable GetDataBySP(string SPName, List<string> ParamName = null, List<string> ParamValue = null);
        Task<DataTable> GetDataBySPAsync(string SPName, List<string> ParamName = null, List<object> ParamValue = null);

        Task<DataSet> GetDataSetBySP(string SPName, Dictionary<string, Object> parameters = null);
        Task<TEntity> GetAsync(long id);
        TEntity Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
    }
}
