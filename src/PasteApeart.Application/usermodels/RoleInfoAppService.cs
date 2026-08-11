using PasteApeart.Application;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasteForm.Helper;
using System.Linq.Dynamic.Core;

namespace PasteApeart.usermodels
{
    /// <summary>
    /// 权限列表
    ///</summary>
    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
    public class RoleInfoAppService : PasteApeartAppService
    {

        //private IAppCache _appCache => LazyServiceProvider.LazyGetRequiredService<IAppCache>();

        ///// <summary>
        ///// 
        ///// </summary>
        //private UserHelper _userHelper => LazyServiceProvider.LazyGetRequiredService<UserHelper>();

        /// <summary>
        /// 
        /// </summary>
        //private UserHandler _userHandler => LazyServiceProvider.LazyGetRequiredService<UserHandler>();

        /// <summary>
        /// 读取AddDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public VoloModelInfo ReadAddModel()
        {
            //当前的路由为 /api/app/userInfo/readAddModel
            var _model = BuilderHelper.AttributeReadModelProperty<RoleInfoAddDto>(new RoleInfoAddDto());
            return _model;
        }

        /// <summary>
        /// 读取UpdateDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public async Task<VoloModelInfo> ReadUpdateModel(int id)
        {
            //当前的路由为 /api/app/userInfo/{id}/readUpdateModel
            var _query = from a in _dbContext.RoleInfo.Where(x => x.Id == id)
                         join b in _dbContext.RoleInfo on a.FatherId equals b.Id into c
                         from rol in c.DefaultIfEmpty()
                         select new RoleInfoUpdateDto
                         {
                             Id = a.Id,
                             Desc = a.Desc,
                             FatherId = a.FatherId,
                             Icon = a.Icon,
                             IsEnable = a.IsEnable,
                             Model = a.Model,
                             Name = a.Name,
                             Path = a.Path,
                             Role = a.Role,
                             RoleType = a.RoleType,
                             Sort = a.Sort,
                             ExtendRole = rol != null ? new RoleShortModel
                             {
                                 Id = rol.Id,
                                 Model = rol.Model,
                                 Name = rol.Name,
                                 Path = rol.Path,
                                 Role = rol.Role,
                                 RoleType = rol.RoleType
                             } : null
                         };

            var _info = await _query.AsNoTracking().FirstOrDefaultAsync();
            if (_info == null || _info == default)
            {
                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }
            var _dataModel = BuilderHelper.AttributeReadModelProperty<RoleInfoUpdateDto>(_info);
            return _dataModel;
        }

        /// <summary>
        /// 读取UpdateDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public async Task<VoloModelInfo> ReadDetailModel(int id)
        {

            var _query = from a in _dbContext.RoleInfo
                         join b in _dbContext.RoleInfo on a.FatherId equals b.Id into c
                         from rol in c.DefaultIfEmpty()
                         select new RoleInfoDto
                         {
                             Id = a.Id,
                             Desc = a.Desc,
                             FatherId = a.FatherId,
                             Icon = a.Icon,
                             IsEnable = a.IsEnable,
                             Model = a.Model,
                             Name = a.Name,
                             Path = a.Path,
                             Role = a.Role,
                             RoleType = a.RoleType,
                             SortStr = a.SortStr,
                             FatherStr = a.FatherStr,
                             Level = a.Level,
                             RootId = a.RootId,
                             Sort = a.Sort,
                             ExtendRole = rol != null ? new RoleShortModel
                             {
                                 Id = rol.Id,
                                 Model = rol.Model,
                                 Name = rol.Name,
                                 Path = rol.Path,
                                 Role = rol.Role,
                                 RoleType = rol.RoleType
                             } : null
                         };

            var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (_info == null || _info == default)
            {
                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }
            var _dataModel = BuilderHelper.AttributeReadModelProperty<RoleInfoDto>(_info);
            return _dataModel;
        }

        /// <summary>
        /// 读取ListDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        public async Task<VoloModelInfo> ReadListModel()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<RoleInfoListDto>(new RoleInfoListDto());
            var isRoot = await base.IsRootAsync();
            if (!isRoot)
            {
                if (_model.Properties != null)
                {
                    _model.Properties.RemoveAll(x => x.Name == "button");
                }
            }
            var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryRoleInfo());
            if (_query_model != null)
            {
                _model.QueryProperties = _query_model.Properties;
            }
            return _model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        //[UseReadOnlyConnectionAttribute]//启用只读
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        [DisableAuditing]
        public async Task<PagedResultDto<RoleInfoListDto>> Page([FromQuery] InputQueryRoleInfo input)
        {

            var query = from a in _dbContext.RoleInfo
                        .WhereIf(!string.IsNullOrEmpty(input.word), x => x.Name.Contains(input.word) || x.Desc.Contains(input.word))
                        .WhereIf(input.FatherId != 0, x => x.FatherId == input.FatherId)
                        .WhereIf(input.role_type != EnumRoleType.All, x => x.RoleType == input.role_type)
                        join b in _dbContext.RoleInfo on a.FatherId equals b.Id into c
                        from role in c.DefaultIfEmpty()
                        select new RoleInfoListDto
                        {
                            Id = a.Id,
                            Desc = a.Desc,
                            FatherId = a.FatherId,
                            Icon = a.Icon,
                            IsEnable = a.IsEnable,
                            Level = a.Level,
                            Model = a.Model,
                            Name = a.Name,
                            Path = a.Path,
                            Role = a.Role,
                            RoleType = a.RoleType,
                            SortStr = a.SortStr,
                            Sort = a.Sort,
                            ExtendFather = role.ToRoleShort()
                        };
            //var query = _query.WhereIf(!string.IsNullOrEmpty(input.word), x => x.Name.Contains(input.word) || x.Desc.Contains(input.word))
            //    .WhereIf(input.FatherId != 0, x => x.FatherId == input.FatherId)
            //    .WhereIf(input.role_type != EnumRoleType.All, x => x.RoleType == input.role_type);
            var _pagedto = new PagedResultDto<RoleInfoListDto>();
            _pagedto.TotalCount = await query.CountAsync();
            var dataList = await query.PasteOrderBy(input.orderby).Page(input.page, input.size).ToListAsync();
            if (input.GradeId > 0)
            {
                var binds = await _dbContext.GradeRole.Where(x => x.GradeId == input.GradeId).Select(x => x.RoleId).ToArrayAsync();
                if (binds != null && binds.Length > 0)
                {
                    foreach (var item in dataList)
                    {
                        if (binds.Contains(item.Id))
                        {
                            item.ExtendBind = true;
                        }
                    }
                }
            }
            _pagedto.Items = dataList;

            return _pagedto;
        }

        /// <summary>
        /// 返回当前登陆者的菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        public async Task<List<RoleMenuModel>> Menus()
        {
            var userId = base.ReadCurrentAdminId();
            //var _token = base.ReadCurrentUserToken();
            var isRoot = await base.IsRootAsync();
            //var _isroot = _userHelper.NewHasRole("root", "root", "", _token);
            var queryGradeId = _dbContext.UserGrade.Where(x => x.UserId == userId).Select(x => x.GradeId);
            var queryRoleId = _dbContext.GradeRole.Where(x => queryGradeId.Contains(x.GradeId)).Select(x => x.RoleId);
            var queryRole = await _dbContext.RoleInfo.Where(x => x.RoleType == EnumRoleType.Menu && x.IsEnable)
                .WhereIf(!isRoot, x => queryRoleId.Contains(x.Id))
                .OrderBy(x => x.SortStr)
                .Select(x => new RoleMenuModel
                {
                    Desc = x.Desc,
                    FatherId = x.FatherId,
                    Icon = x.Icon,
                    Id = x.Id,
                    Level = x.Level,
                    Name = x.Name,
                    Path = x.Path,
                    Sort = x.Sort
                }).ToListAsync();
            if (queryRole != null && queryRole.Count > 0)
            {
                return queryRole;
            }
            else
            {
                return null;
            }
        }

        ///// <summary>
        ///// 更新绑定
        ///// </summary>
        ///// <param name="groupid"></param>
        ///// <param name="roleid"></param>
        ///// <param name="state"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        //public async Task<int> UpdateBindRole(int groupid, int roleid, bool state)
        //{
        //    var grade = _dbContext.GradeInfo.Where(x => x.Id == groupid).AsNoTracking().FirstOrDefault();

        //    var boolupdated = false;
        //    if (grade != null && grade != default)
        //    {
        //        if (state)
        //        {
        //            var find = await _dbContext.GradeRole.Where(x => x.GradeId == groupid && x.RoleId == roleid).AsNoTracking().FirstOrDefaultAsync();
        //            if (find == null || find == default)
        //            {
        //                boolupdated = true;
        //                var one = new GradeRole();
        //                one.GradeId = groupid;
        //                one.RoleId = roleid;
        //                _dbContext.Add(one);
        //                await _dbContext.SaveChangesAsync();
        //            }
        //        }
        //        else
        //        {
        //            var find = await _dbContext.GradeRole.Where(x => x.GradeId == groupid && x.RoleId == roleid).FirstOrDefaultAsync();
        //            if (find != null && find != default)
        //            {
        //                _dbContext.Remove(find);
        //                await _dbContext.SaveChangesAsync();
        //                boolupdated = true;
        //            }
        //        }
        //    }
        //    if (boolupdated)
        //    {
        //        await _appCache.DeleteAsync(string.Format(PublicString.CacheGradeInfoHead, grade.Name));
        //    }
        //    return 1;
        //}

        /// <summary>
        /// 添加一个权限列表
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public async Task<RoleInfoDto> CreateItemAsync(RoleInfoAddDto input)
        {
            //return await _userHandler.RoleAdd(_dbContext, input);
            return await LevelHandler.RoleAdd(_dbContext, input);
        }

        /// <summary>
        /// 更新一个权限列表
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public async Task<RoleInfoDto> UpdateItemAsync(RoleInfoUpdateDto input)
        {
            //return await _userHandler.RoleUpdate(_dbContext, input);

            return await LevelHandler.RoleUpdate(_dbContext, input);
        }

        ///// <summary>
        ///// 更新状态，或者更新绑定
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        ///// <exception cref="PasteCodeException"></exception>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "auth" })]
        //public async Task<string> UpdateState(InputQueryUpdateState input)
        //{
        //    if (!int.TryParse(input.id, out var _id))
        //    {
        //        throw new PasteCodeException("提供的参数id错误，无法继续执行");
        //    }
        //    if (input.search.Contains("model=bind"))
        //    {
        //        if (input.name != "extendBind")
        //        {
        //            throw new PasteCodeException("无效的操作，不支持当前情况下执行此操作！");
        //        }
        //        var _gradeid = 0;// 需要写一个扩展，从参数中提取某一个 先假设下，后面补充
        //        var _querys = input.search.ToQuerys();
        //        if (_querys != null)
        //        {
        //            if (_querys.ContainsKey("grade_id"))
        //            {
        //                int.TryParse(_querys["grade_id"], out _gradeid);
        //            }
        //        }
        //        if (_gradeid == 0)
        //        {
        //            throw new PasteCodeException("参数错误，没有获取到grade_id！");
        //        }
        //        var find = await _dbContext.GradeRole.Where(x => x.GradeId == _gradeid && x.RoleId == _id).FirstOrDefaultAsync();
        //        if (find == null || find == default)
        //        {
        //            if (input.state)
        //            {
        //                _dbContext.Add(new GradeRole { GradeId = _gradeid, RoleId = _id });
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                return "状态一致，不需要变更!";
        //            }
        //        }
        //        else
        //        {
        //            if (!input.state)
        //            {
        //                _dbContext.Remove(find);
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                return "状态一致，不需要变更！";
        //            }
        //        }
        //        return $"更改{input.name}绑定状态成功";
        //    }
        //    else
        //    {
        //        //因为只有一个isenable，直接写
        //        var find = await _dbContext.RoleInfo.Where(x => x.Id == _id).FirstOrDefaultAsync();
        //        if (find == null || find == default)
        //        {
        //            throw new PasteCodeException("没有找到对应的操作对象，无法继续执行");
        //        }
        //        if (find.IsEnable != input.state)
        //        {
        //            find.IsEnable = input.state;
        //            await _dbContext.SaveChangesAsync();
        //        }
        //        return $"修改{input.name}状态成功";
        //    }
        //}

        /// <summary>
        /// 删除数据，不需要删除的话注意注释这个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PasteTransactionFilter))]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
        public async Task<string> Remove(int id)
        {
            //Console.WriteLine("RoleInfo.Remove DbContext HashCode " + _dbContext.GetHashCode());
            var info = await _dbContext.RoleInfo.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (info == null || info == default)
            {
                throw new PasteCodeException("需要查询的信息不存在", 404);
            }
            _dbContext.Remove(info);
            await _dbContext.SaveChangesAsync();
            return "删除成功";
        }

        ///// <summary>
        ///// 备份数据
        ///// </summary>
        ///// <param name="save"></param>
        ///// <returns></returns>
        ///// <exception cref="PasteCodeException"></exception>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        //public async Task<dynamic> Backup(bool save = true)
        //{
        //    return await _userHandler.BackupAsync(_dbContext, save);
        //}

        ///// <summary>
        ///// 载入数据
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        //public async Task<string> Load()
        //{
        //    return await _userHandler.LoadAsync(_dbContext);
        //}

    }
}
