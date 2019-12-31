using Masterpiece.Code;
using Masterpiece.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Masterpiece.Repository.RepositoryBase
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    public class RepositoryBase<TContext, TEntity> where TContext : DbContext, new() where TEntity : class, new()
    {
        protected TContext dbContext;
        protected string tranId;

        public string TranId { get; set; }

        protected RepositoryBase(TContext dbcontext)
        {
            this.dbContext = dbcontext;
        }

        protected virtual List<TEntity> AllEntities()
        {
            return dbContext.Set<TEntity>().ToList();
        }

        protected virtual int DeleteEntity(TEntity entity)
        {
            try
            {
                if (entity is ICaching<TEntity>)
                {
                    TEntity existingEntity = dbContext.Set<TEntity>().Local.FirstOrDefault(((ICaching<TEntity>)entity).CheckInLocal());
                    if (existingEntity != null)
                    {
                        dbContext.Entry(existingEntity).State = EntityState.Detached;
                    }
                }

                dbContext.Set<TEntity>().Attach(entity);
                dbContext.Entry<TEntity>(entity).State = EntityState.Deleted;
                return dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return 0;
            }
        }

        protected virtual int DeleteEntity(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = dbContext.Set<TEntity>().Where(predicate).ToList();
            entities.ForEach(m => dbContext.Entry<TEntity>(m).State = EntityState.Deleted);
            return dbContext.SaveChanges();
        }

        protected virtual TEntity FindEntity(int keyValue)
        {
            return dbContext.Set<TEntity>().Find(keyValue);
        }

        protected virtual TEntity FindEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        protected List<TEntity> FindList(string strSql)
        {
            return dbContext.Database.SqlQuery<TEntity>(strSql).ToList<TEntity>();
        }

        protected virtual List<TEntity> FindList(string strSql, DbParameter[] dbParameter)
        {
            return dbContext.Database.SqlQuery<TEntity>(strSql, dbParameter).ToList<TEntity>();
        }

        protected virtual int InsertEntity(TEntity entity)
        {
            dbContext.Entry<TEntity>(entity).State = EntityState.Added;
            return dbContext.SaveChanges();
        }

        protected virtual int InsertEntity(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                dbContext.Entry<TEntity>(entity).State = EntityState.Added;
            }
            return dbContext.SaveChanges();
        }

        protected virtual IQueryable<TEntity> IQueryable()
        {
            return dbContext.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Set<TEntity>().Where(predicate);
        }

        protected virtual List<T> Paging<T>(string fields,
            string where,
            string orderby, PagingObject paging, object param = null) where T : class
        {
            string pagingTemplate = @"WITH PagingSet AS
                                   (
                                      SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum,{1} from {2}
                                    )
                                    SELECT * FROM PagingSet WHERE RowNum BETWEEN {3} AND {4} ;";

            string totalCountTemplate = @"declare @total int;
                                          set @total = (select COUNT(1) from {0});
                                          select @total; ";

            List<SqlParameter> paramArray = new List<SqlParameter>();
            List<SqlParameter> paramArray1 = new List<SqlParameter>();
            if (param != null)
            {
                PropertyInfo[] ps = param.GetType().GetProperties();
                foreach (PropertyInfo property in ps)
                {
                    string name = property.Name;
                    object value = property.GetValue(param);
                    if (value != null)
                    {
                        SqlParameter sqlParam = new SqlParameter(name, value);
                        SqlParameter sqlParam1 = new SqlParameter(name, value);
                        paramArray.Add(sqlParam);
                        paramArray1.Add(sqlParam1);
                    }
                }
            }

            paging.TotalCount = dbContext.Database.SqlQuery<int>(string.Format(totalCountTemplate, where), paramArray.ToArray()).FirstOrDefault();

            var sql = string.Format(pagingTemplate, orderby, fields, where, (paging.PageIndex - 1) * paging.PageSize + 1, paging.PageIndex * paging.PageSize);
            return dbContext.Database.SqlQuery<T>(sql, paramArray1.ToArray()).ToList();
        }

        protected virtual int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        protected virtual int UpdateEntity(TEntity entity)
        {
            if (entity is ICaching<TEntity>)
            {
                TEntity existingEntity = dbContext.Set<TEntity>().Local.FirstOrDefault(((ICaching<TEntity>)entity).CheckInLocal());
                if (existingEntity != null)
                {
                    dbContext.Entry(existingEntity).State = EntityState.Detached;
                }
            }

            dbContext.Set<TEntity>().Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            if (entity is IChangeTrack && ((IChangeTrack)entity).IsEnableAudit())
            {
                List<string> changeFields = ((IChangeTrack)entity).ChangedFields();
                if (changeFields.Count > 0)
                {
                    props = props.Where(x => changeFields.Contains(x.Name)).ToArray();
                }
                else
                {
                    return 0;
                }
            }
            foreach (PropertyInfo prop in props)
            {
                dbContext.Entry(entity).Property(prop.Name).IsModified = true;
            }
            return dbContext.SaveChanges();
        }

        // protected virtual List<TEntity> FindList(Pagination pagination)
        // {
        //     bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
        //     string[] _order = pagination.sidx.Split(',');
        //     MethodCallExpression resultExp = null;
        //     var tempData = dbcontext.Set<TEntity>().AsQueryable();
        //     foreach (string item in _order)
        //     {
        //         string _orderPart = item;
        //         _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
        //         string[] _orderArry = _orderPart.Split(' ');
        //         string _orderField = _orderArry[0];
        //         bool sort = isAsc;
        //         if (_orderArry.Length == 2)
        //         {
        //             isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
        //         }
        //         var parameter = Expression.Parameter(typeof(TEntity), "t");
        //         var property = typeof(TEntity).GetProperty(_orderField);
        //         var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        //         var orderByExp = Expression.Lambda(propertyAccess, parameter);
        //         resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity),                        property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
        //    }
        //    tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
        //    pagination.records = tempData.Count();
        //    tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
        //    return tempData.ToList();
        //  }
        //  protected virtual List<TEntity> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        //  {
        //      bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
        //      string[] _order = pagination.sidx.Split(',');
        //      MethodCallExpression resultExp = null;
        //      var tempData = dbcontext.Set<TEntity>().Where(predicate);
        //      foreach (string item in _order)
        //      {
        //        string _orderPart = item;
        //        _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
        //        string[] _orderArry = _orderPart.Split(' ');
        //        string _orderField = _orderArry[0];
        //        bool sort = isAsc;
        //        if (_orderArry.Length == 2)
        //        {
        //            isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
        //        }
        //        var parameter = Expression.Parameter(typeof(TEntity), "t");
        //        var property = typeof(TEntity).GetProperty(_orderField);
        //        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        //        var orderByExp = Expression.Lambda(propertyAccess, parameter);
        //        resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity),                 property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
        //    }
        //    tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);

        //    string sql = tempData.ToString();

        //    pagination.records = tempData.Count();

        //    // 数据库分页
        //    if(!pagination.isClientPaging)
        //    {
        //        tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
        //    }

        //    string sql1 = tempData.ToString();

        //    return tempData.ToList();
        // }
    }
}
