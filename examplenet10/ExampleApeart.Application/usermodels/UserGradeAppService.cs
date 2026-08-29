//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using PasteApeart.Helper;
//using ExampleApeart.Application.Contracts;
//using ExampleApeart.Handler;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Domain.Repositories;

//namespace ExampleApeart.usermodels
//{
//    /// <summary>
//    ///
//    ///</summary>
//    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
//    public class UserGradeAppService : ExampleApeartAppService
//    {

//        /// <summary>
//        ///
//        ///</summary>
//        public UserGradeAppService() : base()
//        {

//        }

//        /// <summary>
//        /// 读取AddDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
//        public VoloModelInfo ReadAddModel()
//        {
//            var _model = BuilderHelper.AttributeReadModelProperty<UserGradeAddDto>(new UserGradeAddDto());
//            return _model;
//        }

//        /// <summary>
//        /// 读取UpdateDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<VoloModelInfo> ReadUpdateModel(int id)
//        {
//            var _query = from a in _dbContext.UserGrade
//                         join b in _dbContext.UserInfo on a.UserId equals b.Id into c
//                         from user in c.DefaultIfEmpty()
//                         join d in _dbContext.GradeInfo on a.GradeId equals d.Id into e
//                         from grade in e.DefaultIfEmpty()
//                         select new UserGradeUpdateDto
//                         {
//                             Id = a.Id,
//                             GradeId = a.GradeId,
//                             UserId = a.UserId,
//                             ExtendGrade = grade != null ? grade.ToShortGrade() : null,
//                             ExtendUser = user != null ? user.ToShortUser() : null,
//                         };
//            var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
//            if (_info != null && _info != default)
//            {
//                //var dto = ObjectMapper.Map<UserGrade, UserGradeUpdateDto>(_info);
//                var _dataModel = BuilderHelper.AttributeReadModelProperty<UserGradeUpdateDto>(_info);
//                return _dataModel;
//            }
//            var _model = BuilderHelper.AttributeReadModelProperty<UserGradeUpdateDto>(new UserGradeUpdateDto());
//            return _model;
//        }

//        /// <summary>
//        /// 读取ListDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        public VoloModelInfo ReadListModel()
//        {
//            var _model = BuilderHelper.AttributeReadModelProperty<UserGradeListDto>(new UserGradeListDto());
//            var _query_model = BuilderHelper.AttributeReadModelProperty(new InputQueryUserGrade());
//            if (_query_model != null)
//            {
//                _model.QueryProperties = _query_model.Properties;
//            }
//            return _model;
//        }

//        /// <summary>
//        /// 获取
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpGet]
//        public async Task<PagedResultDto<UserGradeListDto>> Page([FromQuery] InputSearchBase input)
//        {
//            var _query = from a in _dbContext.UserGrade
//                         join b in _dbContext.UserInfo on a.UserId equals b.Id into c
//                         from user in c.DefaultIfEmpty()
//                         join x in _dbContext.GradeInfo on a.GradeId equals x.Id into y
//                         from grade in y.DefaultIfEmpty()
//                         select new UserGradeListDto
//                         {
//                             Id = a.Id,
//                             GradeId = a.GradeId,
//                             UserId = a.UserId,
//                             ExtendGrade = grade != null ? grade.ToShortGrade() : null,
//                             ExtendUser = user != null ? user.ToShortUser() : null
//                         };

//            //var _query = _dbContext.UserGrade.Where(t => 1 == 1);
//            var _pagedto = new PagedResultDto<UserGradeListDto>();
//            if (input.page == 1)
//            {
//                _pagedto.TotalCount = await _query.CountAsync();
//            }
//            var dataList = await _query
//                .OrderByDescending(x => x.Id)
//                .Page(input.page, input.size)
//                .AsNoTracking()
//                .ToListAsync();
//            if (dataList == null || dataList.Count == 0)
//            {
//                return null;
//            }
//            //var temList = ObjectMapper.Map<List<UserGrade>, List<UserGradeListDto>>(dataList);
//            _pagedto.Items = dataList;
//            return _pagedto;
//        }

//        /// <summary>
//        /// 添加一个
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
//        public async Task<UserGradeDto> CreateItemAsync(UserGradeAddDto input)
//        {
//            var _userid = base.ReadCurrentAdminId();
//            var newu = ObjectMapper.Map<UserGradeAddDto, UserGrade>(input);
//            //添加自定义
//            _dbContext.Add(newu);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<UserGrade, UserGradeDto>(newu);
//            return backinfo;
//        }
//        /// <summary>
//        /// 更新一个
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<UserGradeDto> UpdateItemAsync(UserGradeUpdateDto input)
//        {
//            var info = await _dbContext.UserGrade.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("需要查询的信息不存在", 404);
//            }
//            ObjectMapper.Map<UserGradeUpdateDto, UserGrade>(input, info);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<UserGrade, UserGradeDto>(info);
//            return backinfo;
//        }

//        /// <summary>
//        /// 删除数据，不需要删除的话注意注释这个
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
//        public async Task<string> Remove(int id)
//        {
//            var info = await _dbContext.UserGrade.Where(x => x.Id == id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("需要查询的信息不存在", 404);
//            }
//            _dbContext.Remove(info);
//            await _dbContext.SaveChangesAsync();
//            return "删除成功";
//        }

//    }
//}
