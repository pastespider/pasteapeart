using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasteApeart;
using PasteApeart.Application.Contracts;
using PasteApeart.EntityFrameworkCore;
using PasteApeart.Helper;
using System.Linq.Dynamic.Core;

namespace $ext_safeprojectname$.usermodels
{
    /// <summary>
    ///角色信息 一个账号可以绑定多个角色,角色绑定权限
    ///</summary>
    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
    public class GradeInfoAppService : $ext_safeprojectname$AppService
    {

        /// <summary>
        ///角色信息 一个账号可以绑定多个角色,角色绑定权限
        ///</summary>
        public GradeInfoAppService() : base()
        {

        }

        /// <summary>
        /// 读取AddDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public VoloModelInfo ReadAddModel()
        {
            var _model = BuilderHelper.DynamicReadModelProperty<GradeInfoAddDto>(new GradeInfoAddDto());
            return _model;
        }

        /// <summary>
        /// 读取UpdateDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        public async Task<VoloModelInfo> ReadUpdateModel(int id)
        {
            var info = await _dbContext.GradeInfo.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (info == null || info == default)
            {
                throw new PasteCodeException("需要查询的信息不存在", 404);
            }
            //var _info = ObjectMapper.Map<GradeInfo, GradeInfoUpdateDto>(info);
            var _info = info.Adapt<GradeInfoUpdateDto>();
            var _dataModel = BuilderHelper.DynamicReadModelProperty<GradeInfoUpdateDto>(_info);
            return _dataModel;
        }

        /// <summary>
        /// 读取详细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        public async Task<VoloModelInfo> ReadDetailModel(int id)
        {
            var info = await _dbContext.GradeInfo.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (info == null || info == default)
            {
                throw new PasteCodeException("需要查询的信息不存在", 404);
            }
            //var _info = ObjectMapper.Map<GradeInfo, GradeInfoDto>(info);
            var _info = info.Adapt<GradeInfoDto>();
            var _dataModel = BuilderHelper.DynamicReadModelProperty<GradeInfoDto>(_info);
            return _dataModel;
        }

        /// <summary>
        /// 读取ListDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public VoloModelInfo ReadListModel()
        {
            var _model = BuilderHelper.DynamicReadModelProperty<GradeInfoListDto>(new GradeInfoListDto());
            var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryGradeInfo());
            if (_query_model != null)
            {
                _model.QueryProperties = _query_model.Properties;
            }
            return _model;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [UseReadOnlyConnectionAttribute]
        public async Task<PasteResultDto<GradeInfoListDto>> Page([FromQuery] InputQueryGradeInfo input)
        {
            var _query = from a in _dbContext.GradeInfo

                         select new GradeInfoListDto
                         {
                             Id = a.Id,
                             Name = a.Name,
                             Code = a.Code,
                             Desc = a.Desc,
                             Mark = a.Mark,
                             IsEnable = a.IsEnable,
                         };

            _query = _query
                .WhereIf(!String.IsNullOrEmpty(input.word), x =>
                                 x.Name.Contains(input.word)
                                 || x.Code.Contains(input.word)
                )
            ;


            var _pagedto = new PasteResultDto<GradeInfoListDto>();
            if (input.page == 1)
            {
                _pagedto.TotalCount = await _query.CountAsync();
                if (input.request == "one")
                {
                    _pagedto.BuildModel(new GradeInfoListDto(), new InputQueryGradeInfo());
                }
            }
            var dataList = await _query
                .OrderByDescending(x => x.Id)
                .Page(input.page, input.size)
                .AsNoTracking()
                .ToListAsync();
            if (dataList == null || dataList.Count == 0)
            {
                return null;// throw new PasteCodeException("没有查询到数据",204); //由前端处理这个204 由前端处理
            }
            _pagedto.Items = dataList;
            if (input.user_id > 0)
            {
                var binds = await _dbContext.UserGrade.Where(x => x.UserId == input.user_id).Select(x => x.GradeId).ToArrayAsync();
                if (binds?.Length > 0)
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
            return _pagedto;
        }

        ///// <summary>
        ///// 添加一个
        /////</summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        //public async Task<GradeInfoDto> CreateItemAsync(GradeInfoAddDto input)
        //{
        //    //var _userid = base.ReadCurrentUserId();
        //    var info = ObjectMapper.Map<GradeInfoAddDto, GradeInfo>(input);
        //    //添加自定义
        //    _dbContext.Add(info);
        //    await _dbContext.SaveChangesAsync();
        //    var backinfo = ObjectMapper.Map<GradeInfo, GradeInfoDto>(info);
        //    return backinfo;
        //}

        ///// <summary>
        ///// 更新一个
        /////</summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        //public async Task<GradeInfoDto> UpdateItemAsync(GradeInfoUpdateDto input)
        //{
        //    var info = await _dbContext.GradeInfo.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
        //    if (info == null || info == default)
        //    {
        //        throw new PasteCodeException("需要查询的信息不存在", 404);
        //    }
        //    ObjectMapper.Map<GradeInfoUpdateDto, GradeInfo>(input, info);
        //    await _dbContext.SaveChangesAsync();
        //    var backinfo = ObjectMapper.Map<GradeInfo, GradeInfoDto>(info);
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
        //    var find = await _dbContext.GradeInfo.Where(x => x.Id == _id).FirstOrDefaultAsync();
        //    if (find == null || find == default)
        //    {
        //        throw new PasteCodeException("没有找到对应的操作对象，无法继续执行");
        //    }

        //    switch (input.name)
        //    {
        //        case "isEnable":
        //            {
        //                find.IsEnable = input.state;
        //            }
        //            break;
        //    }
        //    await _dbContext.SaveChangesAsync();
        //    return "提交成功";
        //}

        /// <summary>
        /// 删除数据，不需要删除的话注意注释这个 ListDto中附带disable标注忽略删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
        public async Task<string> Remove(int id)
        {
            var info = await _dbContext.GradeInfo.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (info == null || info == default)
            {
                throw new PasteCodeException("需要查询的信息不存在", 404);
            }
            _dbContext.Remove(info);
            await _dbContext.SaveChangesAsync();
            return "删除成功";
        }
    }
}
