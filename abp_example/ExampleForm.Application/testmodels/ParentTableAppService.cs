//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ExampleForm.Application.Contracts;
//using PasteApeart.Helper;
//using System.Linq.Dynamic.Core;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Domain.Repositories;

//namespace ExampleForm.testmodels
//{
//    /// <summary>
//    ///测试表
//    ///</summary>
//    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
//    public class ParentTableAppService : ExampleFormAppService
//    {

//        /// <summary>
//        ///测试表
//        ///</summary>
//        public ParentTableAppService() : base()
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
//            var _model = BuilderHelper.DynamicReadModelProperty<ParentTableAddDto>(new ParentTableAddDto());
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

//            var info = await _dbContext.ParentTable.Where(x => x.Id == id).Include(x => x.Front).AsNoTracking().FirstOrDefaultAsync();

//            var _query = from a in _dbContext.ParentTable
//                         select new ParentTableUpdateDto
//                         {
//                             Id = a.Id,
//                             Name = a.Name,
//                             Age = a.Age,
//                             Front = a.Front,
//                             IsEnable = a.IsEnable,
//                         };

//            var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
//            if (_info == null || _info == default)
//            {
//                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
//            }
//            var _dataModel = BuilderHelper.DynamicReadModelProperty<ParentTableUpdateDto>(_info);
//            return _dataModel;
//        }

//        /// <summary>
//        /// 读取详细
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<VoloModelInfo> ReadDetailModel(int id)
//        {
//            var _query = from a in _dbContext.ParentTable

//                         select new ParentTableDto
//                         {
//                             Id = a.Id,
//                             Name = a.Name,
//                             CreateDate = a.CreateDate,
//                             Age = a.Age,
//                             Front = a.Front,
//                             IsEnable = a.IsEnable,
//                         };

//            var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
//            if (_info == null || _info == default)
//            {
//                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
//            }

//            var _dataModel = BuilderHelper.DynamicReadModelProperty<ParentTableDto>(_info);
//            return _dataModel;
//        }

//        /// <summary>
//        /// 读取ListDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        public VoloModelInfo ReadListModel()
//        {
//            var _model = BuilderHelper.DynamicReadModelProperty<ParentTableListDto>(new ParentTableListDto());
//            var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryParentTable());
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
//        public async Task<PagedResultDto<ParentTableListDto>> Page([FromQuery] InputQueryParentTable input)
//        {
//            var _query = from a in _dbContext.ParentTable

//                         select new ParentTableListDto
//                         {
//                             Id = a.Id,
//                             Name = a.Name,
//                             CreateDate = a.CreateDate,
//                             Age = a.Age,
//                             Front = a.Front,
//                             IsEnable = a.IsEnable,
//                         };

//            _query = _query
//                .WhereIf(!String.IsNullOrEmpty(input.word), x =>
//                                 x.Name.Contains(input.word)
//                )
//            ;


//            var _pagedto = new PagedResultDto<ParentTableListDto>();
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
//                return null;// throw new PasteCodeException("没有查询到数据",204); //由前端处理这个204 由前端处理
//            }
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
//        public async Task<ParentTableDto> CreateItemAsync(ParentTableAddDto input)
//        {
//            //var _userid = base.ReadCurrentUserId();
//            var info = ObjectMapper.Map<ParentTableAddDto, ParentTable>(input);
//            //添加自定义
//            _dbContext.Add(info);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<ParentTable, ParentTableDto>(info);
//            return backinfo;
//        }

//        /// <summary>
//        /// 更新一个
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<ParentTableDto> UpdateItemAsync(ParentTableUpdateDto input)
//        {
//            var info = await _dbContext.ParentTable.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("需要查询的信息不存在", 404);
//            }
//            ObjectMapper.Map<ParentTableUpdateDto, ParentTable>(input, info);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<ParentTable, ParentTableDto>(info);
//            return backinfo;
//        }

//        /// <summary>
//        /// 更新状态，或者更新绑定
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        /// <exception cref="PasteCodeException"></exception>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "state" })]
//        public async Task<string> UpdateState(InputQueryUpdateState input)
//        {
//            if (!int.TryParse(input.id, out var _id))
//            {
//                throw new PasteCodeException("提供的参数id错误，无法继续执行");
//            }

//            //因为只有一个isenable，直接写
//            var find = await _dbContext.ParentTable.Where(x => x.Id == _id).FirstOrDefaultAsync();
//            if (find == null || find == default)
//            {
//                throw new PasteCodeException("没有找到对应的操作对象，无法继续执行");
//            }

//            switch (input.name)
//            {
//                case "isEnable":
//                    {
//                        find.IsEnable = input.state;
//                    }
//                    break;
//            }
//            await _dbContext.SaveChangesAsync();
//            return "提交成功";
//        }

//        /// <summary>
//        /// 删除数据，不需要删除的话注意注释这个 ListDto中附带disable标注忽略删除
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
//        public async Task<string> Remove(int id)
//        {
//            var info = await _dbContext.ParentTable.Where(x => x.Id == id).FirstOrDefaultAsync();
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
