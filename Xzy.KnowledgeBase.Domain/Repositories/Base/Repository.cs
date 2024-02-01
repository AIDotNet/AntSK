using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xzy.KnowledgeBase.Domain.Map;
using Xzy.KnowledgeBase.Domain.Model;

namespace Xzy.KnowledgeBase.Domain.Repositories.Base
{
    public class Repository<T> : SimpleClient<T> where T : class, new()
    {
        public Repository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
        {
            if (context == null)
            {
            }
            //Sqlite.DbMaintenance.CreateDatabase();
            //Sqlite.CodeFirst.InitTables(typeof(CodeFirstTable1));
        }

        //注意：如果使用Client不能写成静态的，Scope并发更高
        public static SqlSugarScope Sqlite = SqlSugarHelper.Sqlite;

        public SimpleClient<T> CurrentDb
        { get { return new SimpleClient<T>(Sqlite); } }//用来处理T表的常用操作

        #region 通用方法

        public virtual SqlSugarScope GetDB()
        {
            return Sqlite;
        }

        /// <summary>
        /// 获取所有list
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }

        /// <summary>
        /// 获取所有list-异步
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync()
        {
            return await CurrentDb.GetListAsync();
        }

        /// <summary>
        /// 根据lambda查询
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.GetList(whereExpression);
        }

        /// <summary>
        /// 根据lambda查询-异步
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await CurrentDb.GetListAsync(whereExpression);
        }

        /// <summary>
        /// 根据lambda表达式获取数量
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.Count(whereExpression);
        }

        /// <summary>
        /// 根据lambda表达式获取数量-异步
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await CurrentDb.CountAsync(whereExpression);
        }

        /// <summary>
        /// 获取分页
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual PageList<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            PageList<T> list = new PageList<T>();
            list.List = CurrentDb.GetPageList(whereExpression, page);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual PageList<P> GetPageList<P>(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            var result = CurrentDb.GetPageList(whereExpression, page);
            var pageData = new PageList<P>
            {
                TotalCount = page.TotalCount,
                PageIndex = page.PageIndex,
                PageSize = page.PageSize,
                List = result.ToDTOList<P>()
            };
            return pageData;
        }

        /// <summary>
        /// 获取分页-异步
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual async Task<PageList<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            PageList<T> list = new PageList<T>();
            list.List = await CurrentDb.GetPageListAsync(whereExpression, page);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual async Task<PageList<P>> GetPageListAsync<P>(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            var result = await CurrentDb.GetPageListAsync(whereExpression, page);
            var pageData = new PageList<P>
            {
                TotalCount = page.TotalCount,
                PageIndex = page.PageIndex,
                PageSize = page.PageSize,
                List = result.ToDTOList<P>()
            };
            return pageData;
        }

        public virtual PageList<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            PageList<T> list = new PageList<T>();
            list.List = CurrentDb.GetPageList(whereExpression, page, orderByExpression, orderByType);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual async Task<PageList<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            PageList<T> list = new PageList<T>();
            list.List = await CurrentDb.GetPageListAsync(whereExpression, page, orderByExpression, orderByType);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual PageList<P> GetPageList<P>(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            var result = CurrentDb.GetPageList(whereExpression, page, orderByExpression, orderByType);
            var pageData = new PageList<P>
            {
                TotalCount = page.TotalCount,
                PageIndex = page.PageIndex,
                PageSize = page.PageSize,
                List = result.ToDTOList<P>()
            };
            return pageData;
        }

        public virtual async Task<PageList<P>> GetPageListAsync<P>(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            var result = await CurrentDb.GetPageListAsync(whereExpression, page, orderByExpression, orderByType);
            var pageData = new PageList<P>
            {
                TotalCount = page.TotalCount,
                PageIndex = page.PageIndex,
                PageSize = page.PageSize,
                List = result.ToDTOList<P>()
            };
            return pageData;
        }

        public virtual PageList<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page)
        {
            PageList<T> list = new PageList<T>();
            list.List = CurrentDb.GetPageList(conditionalList, page);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual async Task<PageList<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page)
        {
            PageList<T> list = new PageList<T>();
            list.List = await CurrentDb.GetPageListAsync(conditionalList, page);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual PageList<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            PageList<T> list = new PageList<T>();
            list.List = CurrentDb.GetPageList(conditionalList, page, orderByExpression, orderByType);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        public virtual async Task<PageList<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            PageList<T> list = new PageList<T>();
            list.List = await CurrentDb.GetPageListAsync(conditionalList, page, orderByExpression, orderByType);
            list.PageIndex = page.PageIndex;
            list.PageSize = page.PageSize;
            list.TotalCount = page.TotalCount;
            return list;
        }

        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(dynamic id)
        {
            return CurrentDb.GetById(id);
        }

        /// <summary>
        /// 根据id获取实体-异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(dynamic id)
        {
            return await CurrentDb.GetByIdAsync(id);
        }

        /// <summary>
        /// 根据lambda获取单个对象 （注意，需要确保唯一，如果获取到2个会报错，这种场景需要使用GetFirst）
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.GetSingle(whereExpression); //Db.Queryable<T>().First(whereExpression);
        }

        /// <summary>
        /// 根据lambda获取单个对象-异步  （注意，需要确保唯一，如果获取到2个会报错，这种场景需要使用GetFirst）
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await CurrentDb.GetSingleAsync(whereExpression); //await Db.Queryable<T>().FirstAsync(whereExpression);
        }

        /// <summary>
        /// 根据lambda获取单个对象
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual T GetFirst(Expression<Func<T, bool>> whereExpression)
        {
            return GetDB().Queryable<T>().First(whereExpression);
        }

        /// <summary>
        /// 根据lambda获取单个对象 --异步
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await GetDB().Queryable<T>().FirstAsync(whereExpression);
        }

        /// <summary>
        /// 实体插入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Insert(T obj)
        {
            return CurrentDb.Insert(obj);
        }

        /// <summary>
        /// 实体插入-异步
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(T obj)
        {
            return await CurrentDb.InsertAsync(obj);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public virtual bool InsertRange(List<T> objs)
        {
            return CurrentDb.InsertRange(objs);
        }

        /// <summary>
        /// 批量插入-异步
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertRangeAsync(List<T> objs)
        {
            return await CurrentDb.InsertRangeAsync(objs);
        }

        /// <summary>
        /// 插入返回自增列
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int InsertReturnIdentity(T obj)
        {
            return CurrentDb.InsertReturnIdentity(obj);
        }

        /// <summary>
        /// 插入返回自增列-异步
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertReturnIdentityAsync(T obj)
        {
            return await CurrentDb.InsertReturnIdentityAsync(obj);
        }

        /// <summary>
        /// 插入返回longid
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual long InsertReturnBigIdentity(T obj)
        {
            return CurrentDb.InsertReturnBigIdentity(obj);
        }

        /// <summary>
        /// 插入返回longid-异步
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<long> InsertReturnBigIdentityAsync(T obj)
        {
            return await CurrentDb.InsertReturnBigIdentityAsync(obj);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteByIds(dynamic[] ids)
        {
            return CurrentDb.DeleteByIds(ids);
        }

        /// <summary>
        /// 批量删除-异步
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            return await CurrentDb.DeleteByIdsAsync(ids);
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(dynamic id)
        {
            return CurrentDb.DeleteById(id);
        }

        /// <summary>
        /// 根据主键删除-异步
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(dynamic id)
        {
            return await CurrentDb.DeleteByIdAsync(id);
        }

        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Delete(T obj)
        {
            return CurrentDb.Delete(obj);
        }

        /// <summary>
        /// 根据实体删除-异步
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T obj)
        {
            return await CurrentDb.DeleteAsync(obj);
        }

        /// <summary>
        /// 根据表达式删除
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.Delete(whereExpression);
        }

        /// <summary>
        /// 根据表达式删除-异步
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await CurrentDb.DeleteAsync(whereExpression);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Update(T obj)
        {
            return CurrentDb.Update(obj);
        }

        /// <summary>
        /// 更新-异步
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T obj)
        {
            return await CurrentDb.UpdateAsync(obj);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public virtual bool UpdateRange(List<T> objs)
        {
            return CurrentDb.UpdateRange(objs);
        }

        /// <summary>
        /// 批量更新-异步
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateRangeAsync(List<T> objs)
        {
            return await CurrentDb.UpdateRangeAsync(objs);
        }

        /// <summary>
        /// 是否包含元素
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.IsAny(whereExpression);
        }

        /// <summary>
        /// 是否包含元素-异步
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await CurrentDb.IsAnyAsync(whereExpression);
        }

        #endregion 通用方法
    }
}
