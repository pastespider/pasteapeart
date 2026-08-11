using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasteApeart.Handler;
using System.Collections;
using System.Linq.Dynamic.Core;

namespace PasteApeart.usermodels
{
    /// <summary>
    ///
    ///</summary>
    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
    public class GradeRoleAppService : PasteApeartAppService
    {

        /// <summary>
        ///
        ///</summary>
        public GradeRoleAppService() : base()
        {

        }

        ///// <summary>
        ///// 读取AddDto的数据模型
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        //public VoloModelInfo ReadAddModel()
        //{
        //    var _model = BuilderHelper.DynamicReadModelProperty<GradeRoleAddDto>(new GradeRoleAddDto());
        //    return _model;
        //}

        ///// <summary>
        ///// 读取UpdateDto的数据模型
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        //public async Task<VoloModelInfo> ReadUpdateModel(int id)
        //{

        //    var _query = from a in _dbContext.GradeRole
        //                 join b11 in _dbContext.GradeInfo on a.GradeId equals b11.Id into b12
        //                 from b13 in b12.DefaultIfEmpty()
        //                 join b21 in _dbContext.RoleInfo on a.RoleId equals b21.Id into b22
        //                 from b23 in b22.DefaultIfEmpty()

        //                 select new GradeRoleUpdateDto
        //                 {
        //                     Id = a.Id,
        //                     GradeId = a.GradeId,
        //                     ExtendGrade = b13 != null ? b13.ToShortGrade() : null,
        //                     RoleId = a.RoleId,
        //                     ExtendRole = b23 != null ? b23.ToShortRole() : null,
        //                 };

        //    var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        //    if (_info == null || _info == default)
        //    {
        //        throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
        //    }
        //    var _dataModel = BuilderHelper.DynamicReadModelProperty<GradeRoleUpdateDto>(_info);
        //    return _dataModel;
        //}

        ///// <summary>
        ///// 读取Dto的数据模型 用于查看详情
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        //public async Task<VoloModelInfo> ReadDetailModel(int id)
        //{
        //    var _info = await _dbContext.GradeRole.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        //    if (_info == null || _info == default)
        //    {
        //        throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
        //    }
        //    var dto = ObjectMapper.Map<GradeRole, GradeRoleDto>(_info);
        //    if (dto.GradeId.PasteHasValue())
        //    {
        //        dto.ExtendGrade = await _dbContext.GradeInfo
        //        .Where(x => x.Id == dto.GradeId)
        //        .Select(x => x.ToShortGrade())
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();
        //    }
        //    if (dto.RoleId.PasteHasValue())
        //    {
        //        dto.ExtendRole = await _dbContext.RoleInfo
        //        .Where(x => x.Id == dto.RoleId)
        //        .Select(x => x.ToShortRole())
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();
        //    }
        //    var _dataModel = BuilderHelper.DynamicReadModelProperty<GradeRoleDto>(dto);
        //    return _dataModel;
        //}

        ///// <summary>
        ///// 读取ListDto的数据模型
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public VoloModelInfo ReadListModel()
        //{
        //    var _model = BuilderHelper.DynamicReadModelProperty<GradeRoleListDto>(new GradeRoleListDto());
        //    var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryGradeRole());
        //    if (_query_model != null)
        //    {
        //        _model.QueryProperties = _query_model.Properties;
        //    }
        //    return _model;
        //}

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<GradeRoleListDto>> Page([FromQuery] InputQueryGradeRole input)
        {
            var _query = from a in _dbContext.GradeRole
                         join b11 in _dbContext.GradeInfo on a.GradeId equals b11.Id into b12
                         from b13 in b12.DefaultIfEmpty()
                         join b21 in _dbContext.RoleInfo on a.RoleId equals b21.Id into b22
                         from b23 in b22.DefaultIfEmpty()
                         select new GradeRoleListDto
                         {
                             Id = a.Id,
                             GradeId = a.GradeId,
                             ExtendGrade = b13.ToGradeShort(),
                             RoleId = a.RoleId,
                             ExtendRole = b23.ToRoleShort(),
                         };

            // 保持您原始的代码结构和风格
            _query = _query
                .WhereIf(input.GradeId != 0, x => x.GradeId == input.GradeId)
                .WhereIf(input.RoleId != 0, x => x.RoleId == input.RoleId);

            var _pagedto = new PagedResultDto<GradeRoleListDto>();
            if (input.page == 1)
            {
                _pagedto.TotalCount = await _query.CountAsync();
            }
            var dataList = await _query
                .OrderByDescending(x => x.Id)
                .Page(input.page, input.size)
                .AsNoTracking()
                .ToListAsync();
            if (dataList == null || dataList.Count == 0)
            {
                return null;
            }
            _pagedto.Items = dataList;
            return _pagedto;
        }


        private IEnumerable GetDynamicDbSet(string entityName)
        {
            var dbSetProperty = _dbContext.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name == entityName);

            if (dbSetProperty == null)
                throw new ArgumentException($"DbSet {entityName} not found in DbContext");

            return dbSetProperty.GetValue(_dbContext) as IEnumerable;
        }

        //  public static IQueryable<TResult> BuildLeftJoinQuery<TOuter, TInner1, TInner2, TResult>(
        //this IQueryable<TOuter> outer,
        //IQueryable<TInner1> inner1,
        //IQueryable<TInner2> inner2,
        //Expression<Func<TOuter, TInner1, TInner2, TResult>> resultSelector,
        //Expression<Func<TOuter, int>> outerKey1,
        //Expression<Func<TInner1, int>> innerKey1,
        //Expression<Func<TOuter, int>> outerKey2,
        //Expression<Func<TInner2, int>> innerKey2)
        //  {
        //      // 构建第一个GroupJoin
        //      var groupJoin1 = CallGroupJoin(outer, inner1, outerKey1, innerKey1);

        //      // 构建第一个SelectMany
        //      var selectMany1 = CallSelectMany(groupJoin1);

        //      // 构建第二个GroupJoin  
        //      var groupJoin2 = CallGroupJoin(selectMany1, inner2, outerKey2, innerKey2);

        //      // 构建第二个SelectMany
        //      var selectMany2 = CallSelectMany(groupJoin2);

        //      // 最终Select
        //      return selectMany2.Select(resultSelector);
        //  }

        //  private static IQueryable<T> CallGroupJoin<TOuter, TInner, T>(
        //      IQueryable<TOuter> outer,
        //      IEnumerable<TInner> inner,
        //      Expression<Func<TOuter, int>> outerKey,
        //      Expression<Func<TInner, int>> innerKey)
        //  {
        //      var method = typeof(Queryable).GetMethods()
        //          .Where(m => m.Name == "GroupJoin")
        //          .Select(m => m.MakeGenericMethod(
        //              typeof(TOuter),
        //              typeof(TInner),
        //              outerKey.Body.Type,
        //              typeof(IEnumerable<TInner>)))
        //          .First();

        //      return (IQueryable<T>)method.Invoke(null, new object[] {
        //      outer,
        //      inner,
        //      outerKey,
        //      innerKey,
        //      Expression.Lambda(
        //          typeof(Func<,>).MakeGenericType(
        //              typeof(TOuter),
        //              typeof(IEnumerable<TInner>)),
        //          Expression.Parameter(typeof(IEnumerable<TInner>), "group"),
        //          Expression.Parameter(typeof(TOuter), "outer"))
        //  });
        //  }

        //  private static IQueryable<T> CallSelectMany<T>(IQueryable<T> source)
        //  {
        //      var method = typeof(Queryable).GetMethods()
        //          .Where(m => m.Name == "SelectMany")
        //          .First()
        //          .MakeGenericMethod(typeof(T), typeof(IEnumerable<>).MakeGenericType(typeof(T).GetGenericArguments()[1]));

        //      return (IQueryable<T>)method.Invoke(null, new object[] {
        //      source,
        //      Expression.Lambda(
        //          typeof(Func<,>).MakeGenericType(
        //              typeof(T),
        //              typeof(IEnumerable<>).MakeGenericType(typeof(T).GetGenericArguments()[1])),
        //          Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeof(T).GetGenericArguments()[1]), "collection"),
        //          Expression.Parameter(typeof(T), "source"))
        //  });
        //  }

        ///// <summary>
        ///// 添加一个
        /////</summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        //public async Task<GradeRoleDto> CreateItemAsync(GradeRoleAddDto input)
        //{
        //    var _userid = base.ReadCurrentAdminId();
        //    var newu = ObjectMapper.Map<GradeRoleAddDto, GradeRole>(input);
        //    //添加自定义
        //    _dbContext.Add(newu);
        //    await _dbContext.SaveChangesAsync();
        //    var backinfo = ObjectMapper.Map<GradeRole, GradeRoleDto>(newu);
        //    return backinfo;
        //}

        ///// <summary>
        ///// 更新一个
        /////</summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        //public async Task<GradeRoleDto> UpdateItemAsync(GradeRoleUpdateDto input)
        //{
        //    var info = await _dbContext.GradeRole.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
        //    if (info == null || info == default)
        //    {
        //        throw new PasteCodeException("需要查询的信息不存在", 404);
        //    }
        //    ObjectMapper.Map<GradeRoleUpdateDto, GradeRole>(input, info);
        //    await _dbContext.SaveChangesAsync();
        //    var backinfo = ObjectMapper.Map<GradeRole, GradeRoleDto>(info);
        //    return backinfo;
        //}

        ///// <summary>
        ///// 更新状态，或者更新绑定
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        ///// <exception cref="PasteCodeException"></exception>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "state" })]
        //public async Task<string> UpdateState(InputQueryUpdateState input)
        //{
        //    if (!int.TryParse(input.id, out var _id))
        //    {
        //        throw new PasteCodeException("提供的参数id错误，无法继续执行");
        //    }

        //    //因为只有一个isenable，直接写
        //    var find = await _dbContext.GradeRole.Where(x => x.Id == _id).FirstOrDefaultAsync();
        //    if (find == null || find == default)
        //    {
        //        throw new PasteCodeException("没有找到对应的操作对象，无法继续执行");
        //    }

        //    switch (input.name)
        //    {
        //    }

        //    //需要自行实现
        //    return "提交成功";
        //}

        ///// <summary>
        ///// 删除数据，不需要删除的话注意注释这个 ListDto中附带disable标注忽略删除
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
        //public async Task<string> Remove(int id)
        //{
        //    var info = await _dbContext.GradeRole.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (info == null || info == default)
        //    {
        //        throw new PasteCodeException("需要查询的信息不存在", 404);
        //    }
        //    _dbContext.Remove(info);
        //    await _dbContext.SaveChangesAsync();
        //    return "删除成功";
        //}
    }
}
