using Movie_WebApi.Services;
using Movie_WebApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Movie_WebApi.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext dbContext;
        protected readonly DbSet<TEntity> dbSet;
        protected bool IsCacheEnable;
        private readonly IResourceService _resourceService;
        private readonly ICommonService _commonService;
        public TEntity originalEntity { get; set; }
        protected readonly string _EntityName;
        protected readonly DateTime _currentSQLServerDate;
        public Repository(DbContext Context, IResourceService resourceService, ICommonService commonService)
        {
            dbContext = Context ?? throw new ArgumentNullException(nameof(Context));
            dbSet = dbContext.Set<TEntity>();
            _resourceService = resourceService;
            _commonService = commonService;
            _EntityName = typeof(TEntity).Name;
            _currentSQLServerDate = DateTime.Now; //_commonService.GetSQLServerDate();
        }


        #region Read

        public async Task<List<TEntity>> GetAllAsync()
        {
            string key = this._EntityName;
            var getAll = await dbContext.Set<TEntity>().ToListAsync();
            try
            {
                var getAllFilter = getAll.Where(o => (bool)o.GetType().GetProperty("IsDeleted").GetValue(o) == false).ToList();
                return getAllFilter;
            }
            catch (Exception exx)
            { }
            return getAll;
        }
        public IEnumerable<TEntity> GetAll()
        {
            string key = this._EntityName;
            var getAll = dbContext.Set<TEntity>().AsEnumerable();
            try
            {
                var getAllFilter = getAll.Where(o => (bool)o.GetType().GetProperty("IsDeleted").GetValue(o) == false).ToList();
                return getAllFilter;
            }
            catch (Exception exx)
            { }
            return getAll;
        }

        public IQueryable<TEntity> GetAllQuery()
        {
            string key = this._EntityName;
            return dbContext.Set<TEntity>().AsQueryable();

        }

        public TEntity Get(int id)
        {
            string key = this._EntityName + id.ToString();
            TEntity o = dbContext.Set<TEntity>().Find(id);
            if (o == null)
            {
                throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
            }
            try
            {
                var isDeleted = o.GetType().GetProperty("IsDeleted").GetValue(o);
                if ((bool)isDeleted)
                {
                    o = null;
                    throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
                }
            }
            catch (Exception exx)
            { }
            return o;
        }

        public async Task<TEntity> GetAsync(int id)
        {
            string key = this._EntityName + id.ToString();
            TEntity o = await dbContext.Set<TEntity>().FindAsync(id);

            if (o == null)
            {
                throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
            }
            try
            {
                var isDeleted = o.GetType().GetProperty("IsDeleted").GetValue(o);
                if ((bool)isDeleted)
                {
                    o = null;
                    throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
                }
            }
            catch (Exception exx)
            { }
            return o;
        }

        public async Task<TEntity> GetNoTrackingAsync(int id)
        {
            string key = this._EntityName + id.ToString();
            TEntity o = await dbContext.Set<TEntity>().FindAsync(id);
            if (o == null)
            {
                throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
            }
            try
            {
                var isDeleted = o.GetType().GetProperty("IsDeleted").GetValue(o);
                if ((bool)isDeleted)
                {
                    o = null;
                    throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
                }
            }
            catch (Exception exx)
            { }
            dbContext.Entry(o).State = EntityState.Detached;
            return o;
        }

        public TEntity GetNoTracking(int id)
        {
            string key = this._EntityName + id.ToString();
            TEntity o = dbContext.Set<TEntity>().Find(id);
            if (o == null)
            {
                throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
            }
            try
            {
                var isDeleted = o.GetType().GetProperty("IsDeleted").GetValue(o);
                if ((bool)isDeleted)
                {
                    o = null;
                    throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
                }
            }
            catch (Exception exx)
            { }
            dbContext.Entry(o).State = EntityState.Detached;
            return o;
        }
        public TEntity GetNoTracking(long id)
        {
            string key = this._EntityName + id.ToString();
            TEntity o = dbContext.Set<TEntity>().Find(id);
            if (o == null)
            {
                throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
            }
            try
            {
                var isDeleted = o.GetType().GetProperty("IsDeleted").GetValue(o);
                if ((bool)isDeleted)
                {
                    o = null;
                    throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
                }
            }
            catch (Exception exx)
            { }
            dbContext.Entry(o).State = EntityState.Detached;
            return o;
        }

        #region Read With Child Entity


        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            string key = this._EntityName + GetPropertyName<TEntity>(includes);

            IQueryable<TEntity> set = dbContext.Set<TEntity>();

            return await includes.Aggregate(set, (current, include) => current.Include(include)).ToListAsync();

            // return (await includes.Aggregate(set, (current, include) => current.Include(include)).ToListAsync() ?? default(IEnumerable<TEntity>));
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            string key = this._EntityName + GetPropertyName<TEntity>(includes);

            IQueryable<TEntity> set = dbContext.Set<TEntity>().Where(predicate);

            return (await includes.Aggregate(set, (current, include) => current.Include(include)).FirstOrDefaultAsync() ?? default(TEntity));
        }
        public TEntity Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            string key = this._EntityName + GetPropertyName<TEntity>(includes);

            IQueryable<TEntity> set = dbContext.Set<TEntity>().Where(predicate);

            return (includes.Aggregate(set, (current, include) => current.Include(include)).FirstOrDefault() ?? default(TEntity));
        }

        #endregion

        #region Read With Predicate

        public Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            var findAll = dbContext.Set<TEntity>().Where(predicate).ToList();

            try
            {
                var getAllFilter = findAll.Where(o => (bool)o.GetType().GetProperty("IsDeleted").GetValue(o) == false).ToList();
                return getAllFilter;
            }
            catch (Exception exx)
            { }
            return findAll;
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var findAll = await dbContext.Set<TEntity>().Where(predicate).ToListAsync();
            try
            {
                var getAllFilter = findAll.Where(o => (bool)o.GetType().GetProperty("IsDeleted").GetValue(o) == false).ToList();
                return getAllFilter;
            }
            catch (Exception exx)
            { }
            return findAll;
        }

        public Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }


        public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                              Expression<Func<TEntity, bool>> predicate = null,
                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                              Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                              bool disableTracking = true)
        {
            IQueryable<TEntity> query = dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                return await query.Select(selector).FirstOrDefaultAsync();
            }
        }


        #endregion

        #endregion

        #region Create Update and Delete

        public void AddAsync(TEntity entity)
        {
            try
            {
                dbContext.Entry(entity).Property("CreatedDate").CurrentValue = _currentSQLServerDate;
            }
            catch (Exception ex) { }
            try
            {
                if (string.IsNullOrEmpty((string)dbContext.Entry(entity).Property("IPAddress").CurrentValue.ToString()))
                {
                    dbContext.Entry(entity).Property("IPAddress").CurrentValue = Helper.GetIPAddress(Helper.httpRequest);
                }
            }
            catch (Exception ex1) { }
            try
            {
                if (string.IsNullOrEmpty((string)dbContext.Entry(entity).Property("Ipaddress").CurrentValue))
                {
                    dbContext.Entry(entity).Property("Ipaddress").CurrentValue = Helper.GetIPAddress(Helper.httpRequest);
                }
            }
            catch { }
            //Global Common Fields Update
            //dbContext.Entry(entity).Property("CreateDate").CurrentValue = _currentSQLServerDate;
            //dbContext.Entry(entity).Property("LastModifiedDate").CurrentValue = dbContext.Entry(entity).Property("CreateDate").CurrentValue;
            //dbContext.Entry(entity).Property("LastModifiedBy").CurrentValue = dbContext.Entry(entity).Property("CreatedBy").CurrentValue;
            //SetCommmonPropertiesWhileAdd(entity);
            dbContext.Set<TEntity>().AddAsync(entity);

        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync(TEntity entity)
        {
            //Find Original Entity and Perform Audit
            TEntity originalEntity = GetOrignalEntity(entity);

            ////Global Common Fields Update
            //dbContext.Entry(entity).Property("Timestamp").CurrentValue = originalEntity.GetType().GetProperty("Timestamp").GetValue(originalEntity);
            //dbContext.Entry(entity).Property("CreateDate").CurrentValue = originalEntity.GetType().GetProperty("CreateDate").GetValue(originalEntity);
            //dbContext.Entry(entity).Property("CreatedBy").CurrentValue = originalEntity.GetType().GetProperty("CreatedBy").GetValue(originalEntity);
            //dbContext.Entry(entity).Property("LastModifiedDate").CurrentValue = _currentSQLServerDate;

            ////Marking Entity to Update & Ignore few fields
            //dbContext.Entry(entity).State = EntityState.Modified;
            //dbContext.Entry(entity).Property("CreateDate").IsModified = false;
            //dbContext.Entry(entity).Property("CreatedBy").IsModified = false;

            SetCommonPropertiesWhileUpdate(entity, originalEntity);
        }
        public TEntity GetEntity(Expression<Func<TEntity, bool>> predicate)
        {
            var oEntity = dbContext.Set<TEntity>().Where(predicate).SingleOrDefault();
            try
            {
                var isDeleted = oEntity.GetType().GetProperty("IsDeleted").GetValue(oEntity);
                if ((bool)isDeleted)
                {
                    oEntity = null;
                }
            }
            catch (Exception exx)
            { }
            return oEntity;
        }

        #region Paging, Searching and Sorting

        public async Task<int> GetRecordsTotalAsync()
        {
            return await dbContext.Set<TEntity>().CountAsync();
        }

        public virtual string GetSearchPropertyName()
        {
            return null; //"FirstName";
        }

        public async Task<IEnumerable<TEntity>> GetPagedSortedFilteredListAsync(int start, int length, string orderColumnName, ListSortDirection order, string searchValue)
        {
            var getAllResult = await CreateQueryWithWhereAndOrderBy(searchValue, orderColumnName, order)
                 .Skip(start)
                 .Take(length)
                 .ToListAsync();
            try
            {
                var getAllFilter = getAllResult.ToList().Where(m => ((dynamic)m).IsDeleted == false).ToList(); //getAllResult.Where(x=> x.GetType().GetProperty("IsDeleted").GetValue(x).ToString()=="true").ToList();
                return getAllFilter;
            }
            catch (Exception exx)
            { }
            return getAllResult;
        }

        protected virtual IQueryable<TEntity> CreateQueryWithWhereAndOrderBy(string searchValue, string orderColumnName, ListSortDirection order)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();

            query = GetWhereQueryForSearchValue(query, searchValue);

            query = AddOrderByToQuery(query, orderColumnName, order);

            return query;
        }

        public async Task<int> GetRecordsFilteredAsync(string searchValue)
        {
            return await GetWhereQueryForSearchValue(dbContext.Set<TEntity>(), searchValue).CountAsync();
        }


        /// <summary>
        /// This generic implementation relies on there being a SearchProperty on the Entity which 
        /// contains a concatenation of the data being displayed, e.g. "Adam Ant 21/02/1990 12345".
        /// Override for specific Datatables to handle logic such as searching on formated dates
        /// and concatenations.
        /// </summary>
        /// <param name="queryable">Entity framework query object</param>
        /// <param name="searchValue">text string to search on all displayed columns</param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> GetWhereQueryForSearchValue(IQueryable<TEntity> queryable, string searchValue)
        {

            string searchPropertyName = GetSearchPropertyName();
            PropertyInfo[] arrayPropertyInfos = typeof(TEntity).GetProperties();
            if (searchValue == "string")
                searchValue = "";
            //Now we will loop in all properties one by one to get value
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                searchPropertyName = property.Name;
                string PropType = property.PropertyType.ToString();

                if (!string.IsNullOrWhiteSpace(searchValue) && !string.IsNullOrWhiteSpace(searchPropertyName) && !PropType.Contains("System.Int32"))
                {
                    var searchValues = Regex.Split(searchValue, "\\s+");

                    foreach (string value in searchValues)
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            queryable = queryable.Where(GetExpressionForPropertyContains(searchPropertyName, value));
                        }
                    }

                    if (queryable.Count() > 0)
                        break;

                    return queryable;
                }



            }
            return queryable;

        }

        /// <summary>
        /// This generic implementation of adding the OrderBy clauses to the query will not
        /// handle any properties which do not exist on the DB table, such as concatenations
        /// or formatted dates.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderColumnName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> AddOrderByToQuery(IQueryable<TEntity> query, string orderColumnName, ListSortDirection order)
        {
            var orderDirectionMethod = order == ListSortDirection.Ascending
                    ? "OrderBy"
                    : "OrderByDescending";

            var type = typeof(TEntity);
            var property = type.GetProperty(orderColumnName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var filteredAndOrderedQuery = Expression.Call(typeof(Queryable), orderDirectionMethod, new Type[] { type, property.PropertyType }, query.Expression, Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<TEntity>(filteredAndOrderedQuery);
        }

        protected Expression<Func<TEntity, bool>> GetExpressionForPropertyContains(string propertyName, string value)
        {
            var parent = Expression.Parameter(typeof(TEntity));
            MethodInfo method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            var expressionBody = Expression.Call(Expression.Property(parent, propertyName), method, Expression.Constant(value));
            return Expression.Lambda<Func<TEntity, bool>>(expressionBody, parent);
        }

        #endregion

        #region Update with child Entity

        /// <summary>
        /// Method to Update Parent and all child entities depending on the state of object.
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateGraph(TEntity entity)
        {
            dbContext.ChangeTracker.TrackGraph(entity, node =>
            {
                var entry = node.Entry;
                var childEntity = entry.Entity;

                EntityState entityState = (EntityState)childEntity.GetType().GetProperty("EntityState").GetValue(childEntity);

                switch (entityState)
                {
                    case EntityState.Added:
                        New(childEntity);
                        break;

                    case EntityState.Modified:
                        Modify(childEntity);
                        break;
                    case EntityState.Deleted:
                        Delete(childEntity);
                        break;
                    default:
                        dbContext.Entry(childEntity).State = entityState;
                        break;
                }

            });


        }
        private void SetCommmonPropertiesWhileAdd(object entity)
        {
            dbContext.Entry(entity).Property("CreatedDate").CurrentValue = _currentSQLServerDate;
            dbContext.Entry(entity).Property("UpdatedDate").CurrentValue = _currentSQLServerDate;
            dbContext.Entry(entity).Property("UpdatedBy").CurrentValue = dbContext.Entry(entity).Property("CreatedBy").CurrentValue;
            //dbContext.Entry(entity).Property("ModifiedDate").CurrentValue = _currentSQLServerDate;
            //dbContext.Entry(entity).Property("ModifiedBy").CurrentValue = dbContext.Entry(entity).Property("CreatedBy").CurrentValue;

        }
        private void SetCommonPropertiesWhileUpdate(object entity, object oEntity)
        {
            //Global Common Fields Update
            //dbContext.Entry(entity).Property("Timestamp").CurrentValue = oEntity.GetType().GetProperty("Timestamp").GetValue(oEntity);
            try
            {
                dbContext.Entry(entity).Property("CreatedDate").CurrentValue = oEntity.GetType().GetProperty("CreatedDate").GetValue(oEntity);
                dbContext.Entry(entity).Property("UpdatedDate").CurrentValue = _currentSQLServerDate;
            }
            catch { }
            ///Added try catch to bypass exception in case createdby column not found in table entity
            ///as error was getting in TUVBooking -- Sumit Pathak 13_04_2020
            try
            {
                dbContext.Entry(entity).Property("CreatedBy").CurrentValue = oEntity.GetType().GetProperty("CreatedBy").GetValue(oEntity);
                if ((bool)dbContext.Entry(entity).Property("IsDeleted").CurrentValue == true)
                {
                    dbContext.Entry(entity).Property("DeletedDate").CurrentValue = _currentSQLServerDate;
                }
            }
            catch { }

            //Marking Entity to Update & Ignore few fields
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                dbContext.Entry(entity).Property("CreatedDate").IsModified = false;
            }
            catch { }
            try
            {
                dbContext.Entry(entity).Property("CreatedBy").IsModified = false;
            }
            catch (Exception ex) { }
        }
        private void New(object childEntity)
        {
            SetCommmonPropertiesWhileAdd(childEntity);
            dbContext.Entry(childEntity).State = EntityState.Added;
        }
        private void Modify(object childEntity)
        {
            Type type = childEntity.GetType();
            var keyName = dbContext.Model.FindEntityType(type).FindPrimaryKey().Properties.Select(x => x.Name).Single();
            var pkey = (int)childEntity.GetType().GetProperty(keyName).GetValue(childEntity, null);

            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var originalEntity = dbContext.Find(childEntity.GetType(), pkey);
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            if (originalEntity != null)
            {
                dbContext.Entry(originalEntity).State = EntityState.Detached;

                //////Global Common Fields Update
                //dbContext.Entry(childEntity).Property("LastModifiedDate").CurrentValue = _currentSQLServerDate;
                //dbContext.Entry(childEntity).Property("LastModifiedBy").CurrentValue = originalEntity.GetType().GetProperty("LastModifiedBy").GetValue(originalEntity);
                //dbContext.Entry(childEntity).Property("Timestamp").CurrentValue = originalEntity.GetType().GetProperty("Timestamp").GetValue(originalEntity);
                //dbContext.Entry(childEntity).Property("CreateDate").CurrentValue = originalEntity.GetType().GetProperty("CreateDate").GetValue(originalEntity);
                //dbContext.Entry(childEntity).Property("CreatedBy").CurrentValue = originalEntity.GetType().GetProperty("CreatedBy").GetValue(originalEntity);
                SetCommonPropertiesWhileUpdate(childEntity, originalEntity);

                ////Marking Entity to Update & Ignore few fields
                //dbContext.Entry(childEntity).State = EntityState.Modified;
                //dbContext.Entry(childEntity).Property("CreateDate").IsModified = false;
                //dbContext.Entry(childEntity).Property("CreatedBy").IsModified = false;

                dbContext.Entry(childEntity).State = EntityState.Modified;

            }

        }
        private void Delete(object childEntity)
        {
            Type type = childEntity.GetType();
            var keyName = dbContext.Model.FindEntityType(type).FindPrimaryKey().Properties.Select(x => x.Name).Single();
            var pkey = (int)childEntity.GetType().GetProperty(keyName).GetValue(childEntity, null);

            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var originalEntity = dbContext.Find(childEntity.GetType(), pkey);
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            if (originalEntity != null)
            {
                dbContext.Entry(originalEntity).State = EntityState.Detached;

                //dbContext.Entry(childEntity).Property("Timestamp").CurrentValue = originalEntity.GetType().GetProperty("Timestamp").GetValue(originalEntity);
                dbContext.Entry(childEntity).State = EntityState.Deleted;

            }

        }

        #endregion

        public void Remove(TEntity entity)
        {
            //Find Original Entity
            TEntity originalEntity = GetOrignalEntity(entity);
            dbContext.Entry(originalEntity).State = EntityState.Deleted;
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Addtional Mathod

        public IList<T> Replace<T>(IList<T> source, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var keyName = dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
               .Select(x => x.Name).Single();
            object pkey = GetKey<T>(newValue);


            var oValue = source.Select(s => s.GetType().GetProperty(keyName).GetValue(s)).ToList();
            var index = oValue.IndexOf(pkey);

            if (index != -1)
                source[index] = newValue;
            else
                source[source.Count() + 1] = newValue;

            return source;
        }

        private string GetPropertyName<T>(Expression<Func<T, object>>[] includes)
        {
            string key = string.Empty;
            foreach (var include in includes)
            {
                MemberExpression memberExpr = include.Body as MemberExpression;
                if (memberExpr == null)
                    throw new ArgumentException("Expression body must be a member expression");

                key += "_" + memberExpr.Member.Name;
            }
            return key;
        }


        public virtual dynamic GetKey<T>(T entity)
        {
            var keyName = dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                .Select(x => x.Name).Single();

            return (dynamic)entity.GetType().GetProperty(keyName).GetValue(entity, null);
        }
        public TEntity GetOrignalEntity(TEntity entity)
        {
            //Get Primary Key
            var pkey = GetKey<TEntity>(entity);

            //Get Original Entity from Database
            TEntity originalEntity = GetNoTracking(pkey);
            this.originalEntity = originalEntity;

            return originalEntity;
        }

        public bool Exists(Func<TEntity, bool> conditions)
        {
            return dbSet.Any(conditions);
        }

        public async Task<List<DyanmicModel>> GetSPDataByEntity(string SPName, string parameter)
        {
            var result = await dbContext.Set<DyanmicModel>().FromSql(SPName).ToListAsync();
            return result;
        }

        public DataTable GetDataBySP(string SPName, List<string> ParamName = null, List<string> ParamValue = null)
        {
            DataTable oDTList = new DataTable();
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = SPName;
                command.CommandType = CommandType.StoredProcedure;
                if (ParamName != null && ParamName.Count > 0)
                {
                    for (int i = 0; i < ParamName.Count; i++)
                    {
                        command.Parameters.Add(new SqlParameter(ParamName[i].ToString(), ParamValue[i]));
                    }
                }
                dbContext.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        oDTList.Columns.Add(name);
                    }
                    while (reader.Read())
                    {
                        DataRow dr = oDTList.NewRow();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            if (oDTList.Columns[i].ToString() == reader.GetName(i).ToString())
                            {
                                dr[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[i];
                            }
                        }
                        oDTList.Rows.Add(dr);
                    }
                }
                dbContext.Database.CloseConnection();
            }
            return oDTList;
        }

        public async Task<DataSet> GetDataSetBySP(string SPName, Dictionary<string, Object> parameters = null)
        {
            // creates resulting dataset
            var result = new DataSet();

            // creates a data access context (DbContext descendant)
            using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            {
                // creates a Command                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;

                // adds all parameters
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var pr in parameters)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = pr.Key;
                        p.Value = pr.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                try
                {
                    // executes
                    dbContext.Database.OpenConnection();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // loop through all resultsets (considering that it's possible to have more than one)
                        do
                        {
                            // loads the DataTable (schema will be fetch automatically)
                            var tb = new DataTable();
                            tb.Load(reader);
                            result.Tables.Add(tb);

                        } while (!reader.IsClosed);
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    // closes the connection
                    dbContext.Database.CloseConnection();
                }
            }

            // returns the DataSet
            return result;
        }
        public async Task<DataTable> GetDataBySPAsync(string SPName, List<string> ParamName = null, List<object> ParamValue = null)
        {
            DataTable oDTList = new DataTable();
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = SPName;
                command.CommandType = CommandType.StoredProcedure;
                if (ParamName != null && ParamName.Count > 0)
                {
                    for (int i = 0; i < ParamName.Count; i++)
                    {
                        command.Parameters.Add(new SqlParameter(ParamName[i].ToString(), ParamValue[i]));
                    }
                }
                dbContext.Database.OpenConnection();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        oDTList.Columns.Add(name);
                    }
                    while (reader.Read())
                    {
                        DataRow dr = oDTList.NewRow();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            if (oDTList.Columns[i].ToString() == reader.GetName(i).ToString())
                            {
                                dr[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[i];
                            }
                        }
                        oDTList.Rows.Add(dr);
                    }
                }
                dbContext.Database.CloseConnection();
            }
            return oDTList;
        }

        public async Task<TEntity> GetAsync(long id)
        {
            string key = this._EntityName + id.ToString();
            TEntity o = await dbContext.Set<TEntity>().FindAsync(id);

            if (o == null)
            {
                throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
            }
            try
            {
                var isDeleted = o.GetType().GetProperty("IsDeleted").GetValue(o);
                if ((bool)isDeleted)
                {
                    o = null;
                    throw new Exception("Record Not Found", new KeyNotFoundException("Record Not Found"));
                }
            }
            catch (Exception exx)
            { }
            return o;
        }

        #endregion
    }
}
