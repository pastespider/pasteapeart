//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ExampleForm.Application.Contracts;
//using ExampleForm.Handler;
//using PasteApeart.Helper;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Domain.Repositories;

//namespace ExampleForm.testmodels
//{
//    /// <summary>
//    ///
//    ///</summary>
//    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
//    public class TestTableAppService : ExampleFormAppService
//    {

//        /// <summary>
//        ///
//        ///</summary>
//        public TestTableAppService() : base()
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
//            var _model = BuilderHelper.DynamicReadModelProperty<TestTableAddDto>(new TestTableAddDto());
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

//            var _query = from a in _dbContext.TestTable
//                         join b11 in _dbContext.UserInfo on a.UserId equals b11.Id into b12
//                         from b13 in b12.DefaultIfEmpty()

//                         select new TestTableUpdateDto
//                         {
//                             Id = a.Id,
//                             Name = a.Name,
//                             Head = a.Head,
//                             Age = a.Age,
//                             DateStart = a.DateStart,
//                             DateEnd = a.DateEnd,
//                             Balance = a.Balance,
//                             UserId = a.UserId,
//                             ExtendUser = b13 != null ? b13.ToUserShort() : null,
//                             //More = a.More,
//                         };

//            var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
//            if (_info == null || _info == default)
//            {
//                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
//            }
//            var _dataModel = BuilderHelper.DynamicReadModelProperty<TestTableUpdateDto>(_info);
//            return _dataModel;
//        }

//        /// <summary>
//        /// 读取Dto的数据模型 用于查看详情
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<VoloModelInfo> ReadDetailModel(int id)
//        {
//            var _info = await _dbContext.TestTable.Where(x => x.Id == id)
//            .AsNoTracking().FirstOrDefaultAsync();
//            if (_info == null || _info == default)
//            {
//                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
//            }
//            var dto = ObjectMapper.Map<TestTable, TestTableDto>(_info);
//            if (dto.UserId.PasteHasValue())
//            {
//                dto.ExtendUser = await _dbContext.UserInfo
//                .Where(x => x.Id == dto.UserId)
//                .Select(x => x.ToUserShort())
//                .AsNoTracking()
//                .FirstOrDefaultAsync();
//            }
//            var _dataModel = BuilderHelper.DynamicReadModelProperty<TestTableDto>(dto);
//            return _dataModel;
//        }

//        /// <summary>
//        /// 读取ListDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        public VoloModelInfo ReadListModel()
//        {
//            var _model = BuilderHelper.DynamicReadModelProperty<TestTableListDto>(new TestTableListDto());
//            var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryTestTable());
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
//        public async Task<PagedResultDto<TestTableListDto>> Page([FromQuery] InputQueryTestTable input)
//        {
//            var _query = from a in _dbContext.TestTable
//                         join b11 in _dbContext.UserInfo on a.UserId equals b11.Id into b12
//                         from b13 in b12.DefaultIfEmpty()
//                         select new TestTableListDto
//                         {
//                             Name = a.Name,
//                             Head = a.Head,
//                             Age = a.Age,
//                             DateStart = a.DateStart,
//                             DateEnd = a.DateEnd,
//                             Balance = a.Balance,
//                             Id = a.Id,
//                             UserId = a.UserId,
//                             ExtendUser = b13 != null ? b13.ToUserShort() : null,
//                             //More = a.More,
//                         };

//            _query = _query.WhereIf(!String.IsNullOrEmpty(input.word), x => x.Name.Contains(input.word) || x.Head.Contains(input.word));

//            var _pagedto = new PagedResultDto<TestTableListDto>();
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
//        public async Task<TestTableDto> CreateItemAsync(TestTableAddDto input)
//        {
//            var _userid = base.ReadCurrentAdminId();
//            var info = ObjectMapper.Map<TestTableAddDto, TestTable>(input);
//            //添加自定义
//            _dbContext.Add(info);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<TestTable, TestTableDto>(info);
//            return backinfo;
//        }

//        /// <summary>
//        /// 更新一个
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<TestTableDto> UpdateItemAsync(TestTableUpdateDto input)
//        {
//            var info = await _dbContext.TestTable.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("需要查询的信息不存在", 404);
//            }
//            ObjectMapper.Map<TestTableUpdateDto, TestTable>(input, info);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<TestTable, TestTableDto>(info);
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
//            var info = await _dbContext.TestTable.Where(x => x.Id == _id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("没有找到对应的操作对象，无法继续执行");
//            }

//            switch (input.name)
//            {
//            }

//            //需要自行实现
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
//            var info = await _dbContext.TestTable.Where(x => x.Id == id).FirstOrDefaultAsync();
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
