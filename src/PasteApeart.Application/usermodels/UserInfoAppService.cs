using PasteApeart.Application.Contracts;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasteForm.Helper;
using System.Linq.Dynamic.Core;

namespace PasteApeart.usermodels
{
    /// <summary>
    ///
    ///</summary>
    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
    public class UserInfoAppService : PasteApeartAppService
    {

        /// <summary>
        ///
        ///</summary>
        public UserInfoAppService() : base()
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
            var _model = BuilderHelper.AttributeReadModelProperty<UserInfoAddDto>(new UserInfoAddDto());
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
            var _info = await _dbContext.UserInfo.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (_info == null || _info == default)
            {
                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }
            //var dto = ObjectMapper.Map<UserInfo, UserInfoUpdateDto>(_info);
            var dto = _info.Adapt<UserInfoUpdateDto>();
            var _query_gradeid = _dbContext.UserGrade.Where(x => x.UserId == _info.Id).Select(x => x.GradeId);
            var grades = await _dbContext.GradeInfo.Where(x => _query_gradeid.Contains(x.Id)).AsNoTracking().ToListAsync();
            if (grades != null && grades.Count > 0)
            {
                //dto.Grades = ObjectMapper.Map<List<GradeInfo>, List<GradeInfoListDto>>(grades);
                dto.Grades = grades.Adapt<List<GradeInfoListDto>>();
            }

            dto.PassWord = "";//密码只有重置，没有修改
            var _dataModel = BuilderHelper.AttributeReadModelProperty<UserInfoUpdateDto>(dto);
            return _dataModel;
        }
        /// <summary>
        /// 读取UpdateDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<VoloModelInfo> ReadDetailModel(int id)
        {
            var _info = await _dbContext.UserInfo.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (_info == null || _info == default)
            {
                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }
            //var dto = ObjectMapper.Map<UserInfo, UserInfoDto>(_info);
            var dto = _info.Adapt<UserInfoDto>();
            var _dataModel = BuilderHelper.AttributeReadModelProperty<UserInfoDto>(dto);
            return _dataModel;

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<List<UserInfoListDto>> MapsterPageAsync()
        //{
        //    var reads = await _dbContext.UserInfo.Where(x => x.Id == 1).ProjectToType<UserInfoListDto>().ToListAsync();
        //    return reads;
        //}

        ///// <summary>
        ///// 读取ListDto的数据模型
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public VoloModelInfo ReadListModel()
        //{
        //    var _model = BuilderHelper.AttributeReadModelProperty<UserInfoListDto>(new UserInfoListDto());
        //    var _query_model = BuilderHelper.AttributeReadModelProperty(new InputQueryUserInfo());
        //    if (_query_model != null)
        //    {
        //        _model.QueryProperties = _query_model.Properties;
        //    }
        //    return _model;
        //}

        ///// <summary>
        ///// 获取
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<PagedResultDto<UserInfoListDto>> Page([FromQuery] InputQueryUserInfo input)
        //{
        //    var _query = _dbContext.UserInfo.Where(t => 1 == 1);
        //    var _pagedto = new PagedResultDto<UserInfoListDto>();
        //    if (input.page == 1)
        //    {
        //        _pagedto.TotalCount = await _query.CountAsync();
        //    }
        //    var dataList = await _query
        //        .WhereIf(!String.IsNullOrEmpty(input.word), x => x.UserName.Contains(input.word) || x.Desc.Contains(input.word))
        //        .WhereIf(input.sdate != null, x => input.sdate <= x.CreateDate && x.CreateDate < input.edate)
        //        .WhereIf(!string.IsNullOrEmpty(input.email),x=>x.Email == input.email)
        //        .OrderByDescending(x => x.Id)
        //        .Page(input.page, input.size)
        //        .AsNoTracking()
        //        .ToListAsync();
        //    if (dataList == null || dataList.Count == 0)
        //    {
        //        return null;
        //    }
        //    var temList = ObjectMapper.Map<List<UserInfo>, List<UserInfoListDto>>(dataList);
        //    _pagedto.Items = temList;
        //    return _pagedto;
        //}

        /// <summary>
        /// 添加一个
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public async Task<UserInfoListDto> CreateItemAsync(UserInfoAddDto input)
        {
            var _userid = base.ReadCurrentAdminId();
            try
            {
                _dbContext.Database.BeginTransaction();
                //var newu = ObjectMapper.Map<UserInfoAddDto, UserInfo>(input);
                var newu = input.Adapt<UserInfo>();
                //添加自定义
                newu.PassWord = newu.PassWord.ToMd5Lower();//和下面同理，两边要一致，登陆那边也要
                _dbContext.Add(newu);
                newu.CreateDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                if (input.GradeIds != null && input.GradeIds.Length > 0)
                {
                    var list = new List<UserGrade>();
                    //var ids = input.GradeIds.Distinct().ToList();
                    foreach (var gradeid in input.GradeIds.Distinct().ToList())
                    {
                        list.Add(new UserGrade { GradeId = gradeid, UserId = newu.Id });
                    }
                    _dbContext.AddRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("没有配置角色信息");
                }
                _dbContext.Database.CommitTransaction();
                var dto = newu.Adapt<UserInfoListDto>();

                return dto;
            }
            catch (Exception exl)
            {
                _dbContext.Database.RollbackTransaction();
                throw new PasteCodeException(exl.Message);
            }
            //var backinfo = ObjectMapper.Map<UserInfo, UserInfoDto>(newu);
            //return "创建成功";
        }
        /// <summary>
        /// 更新一个
        ///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        public async Task<UserInfoListDto> UpdateItemAsync(UserInfoUpdateDto input)
        {
            var info = await _dbContext.UserInfo.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
            if (info == null || info == default)
            {
                throw new PasteCodeException("需要查询的信息不存在", 404);
            }
            var _oldpass = info.PassWord;
            try
            {
                _dbContext.Database.BeginTransaction();
                //ObjectMapper.Map<UserInfoUpdateDto, UserInfo>(input, info);
                input.Adapt(info);
                if (!String.IsNullOrEmpty(input.PassWord))
                {
                    info.PassWord = input.PassWord.ToMd5Lower();//这里可以和自己的密钥混淆下，或者是创建时间啥的!
                }
                else
                {
                    info.PassWord = _oldpass;
                }
                await _dbContext.SaveChangesAsync();

                // if (input.GradeIds != null && input.GradeIds.Length > 0)
                // {
                //     var adds = new List<int>();
                //     var dels = new List<int>();
                //     var binds = await _dbContext.UserGrade.Where(x => x.UserId == info.Id).Select(x => x.GradeId).ToArrayAsync();
                //     if (binds != null && binds.Length > 0)
                //     {
                //         if (input.GradeIds == null || input.GradeIds.Length == 0)
                //         {
                //             await _dbContext.UserGrade.Where(x => x.UserId == info.Id).DeleteAsync();
                //         }
                //         else
                //         {
                //             dels = binds.Except(input.GradeIds).ToList();
                //             adds = input.GradeIds.Except(binds).ToList();
                //         }
                //     }
                //     else
                //     {
                //         adds = input.GradeIds.Distinct().ToList();
                //     }
                //     if (adds != null && adds.Count > 0)
                //     {
                //         var list = new List<UserGrade>();
                //         foreach (var gradeid in adds)
                //         {
                //             list.Add(new UserGrade { GradeId = gradeid, UserId = info.Id });
                //         }
                //         _dbContext.AddRange(list);
                //         await _dbContext.SaveChangesAsync();
                //     }
                //     if (dels != null && dels.Count > 0)
                //     {
                //         await _dbContext.UserGrade.Where(x => x.UserId == info.Id && dels.Contains(x.GradeId)).DeleteAsync();
                //     }
                // }
                // else
                // {
                //     await _dbContext.UserGrade.Where(x => x.UserId == info.Id).DeleteAsync();
                // }
                _dbContext.Database.CommitTransaction();
                return info.Adapt<UserInfoListDto>();
            }
            catch (Exception exl)
            {
                _dbContext.Database.RollbackTransaction();

                throw new PasteCodeException(exl.Message);
            }
            //var backinfo = ObjectMapper.Map<UserInfo, UserInfoDto>(info);
            //return "修改成功";
        }

        /// <summary>
        /// 更新状态，或者更新绑定
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "state" })]
        public async Task<string> UpdateState(InputQueryUpdateState input)
        {
            if (!int.TryParse(input.id, out var _id))
            {
                throw new PasteCodeException("提供的参数id错误，无法继续执行");
            }
            if (input.search.Contains("model=bind"))
            {
                var _gradeid = 0;// 需要写一个扩展，从参数中提取某一个 先假设下，后面补充
                var _querys = input.search.ToQuerys();
                if (_querys != null)
                {
                    if (_querys.ContainsKey("gradeId"))
                    {
                        int.TryParse(_querys["gradeId"], out _gradeid);
                    }
                }
                if (_gradeid == 0)
                {
                    throw new PasteCodeException("参数错误，没有获取到gradeId！");
                }
                var find = await _dbContext.UserGrade.Where(x => x.GradeId == _gradeid && x.UserId == _id).FirstOrDefaultAsync();
                if (find == null || find == default)
                {
                    if (input.state)
                    {
                        _dbContext.Add(new UserGrade { GradeId = _gradeid, UserId = _id });
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    if (!input.state)
                    {
                        _dbContext.Remove(find);
                        await _dbContext.SaveChangesAsync();
                    }
                }

            }
            else
            {
                //因为只有一个isenable，直接写
                var find = await _dbContext.UserInfo.Where(x => x.Id == _id).FirstOrDefaultAsync();
                if (find == null || find == default)
                {
                    throw new PasteCodeException("没有找到对应的操作对象，无法继续执行");
                }
                if (find.IsEnable != input.state)
                {
                    find.IsEnable = input.state;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return "操作成功";
        }

        /// <summary>
        /// 删除数据，不需要删除的话注意注释这个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
        public async Task<string> Remove(int id)
        {
            var info = await _dbContext.UserInfo.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (info == null || info == default)
            {
                throw new PasteCodeException("需要查询的信息不存在", 404);
            }
            //这里根据实际需求 比如做删除前的判断 或是软删除 禁用状态等
            _dbContext.Remove(info);
            await _dbContext.SaveChangesAsync();
            return "删除成功";
        }

    }
}
