//using System.Collections.Generic;
//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Domain.Repositories;
//using PasteApeart.Helper;
//using ExampleForm.Application.Contracts;

//namespace ExampleForm.testmodels
//{
//    /// <summary>
//    ///测试链接表
//    ///</summary>
//    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
//    public class TestLinkIncludeAppService : ExampleFormAppService
//    {

//        /// <summary>
//        ///测试链接表
//        ///</summary>
//        public TestLinkIncludeAppService():base()
//        {

//        }

//        /// <summary>
//        /// 读取AddDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//		[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
//        public VoloModelInfo ReadAddModel()
//        {
//            var _model = BuilderHelper.DynamicReadModelProperty<TestLinkIncludeAddDto>(new TestLinkIncludeAddDto());
//            return _model;
//        }

//        /// <summary>
//        /// 读取UpdateDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//		[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<VoloModelInfo> ReadUpdateModel(int id)
//        {
//            var _query = from a in _dbContext.TestLinkInclude
//                            join b11 in _dbContext.UserInfo on a.UserId equals b11.Id into b12
//                            from b13 in b12.DefaultIfEmpty()
//                            join b21 in _dbContext.GradeInfo on a.GradeId equals b21.Id into b22
//                            from b23 in b22.DefaultIfEmpty()

//            select new TestLinkIncludeUpdateDto
//            {
//                Id = a.Id,
//                        MyTable = a.MyTable,
//                        UserId = a.UserId,
//                                    ExtendUser = b13 != null ? b13.ToUserShort() : null,
//                        GradeId = a.GradeId,
//                                    ExtendGrade = b23 != null ? b23.ToGradeShort() : null,
//            };

//			var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
//            if (_info == null || _info == default)
//            {
//                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
//            }
//            var _dataModel = BuilderHelper.DynamicReadModelProperty<TestLinkIncludeUpdateDto>(_info);
//            return _dataModel;
//        }

//        /// <summary>
//        /// 读取详细
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//		[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<VoloModelInfo> ReadDetailModel(int id)
//        {
//            var _query = from a in _dbContext.TestLinkInclude
//                            join b11 in _dbContext.UserInfo on a.UserId equals b11.Id into b12
//                            from b13 in b12.DefaultIfEmpty()
//                            join b21 in _dbContext.GradeInfo on a.GradeId equals b21.Id into b22
//                            from b23 in b22.DefaultIfEmpty()

//            select new TestLinkIncludeDto
//            {
//                Id = a.Id,
//                        MyTable = a.MyTable,
//                        UserId = a.UserId,
//                                    ExtendUser = b13 != null ? b13.ToUserShort() : null,
//                        GradeId = a.GradeId,
//                                    ExtendGrade = b23 != null ? b23.ToGradeShort() : null,
//            };

//			var _info = await _query.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
//            if (_info == null || _info == default)
//            {
//                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
//            }

//            var _dataModel = BuilderHelper.DynamicReadModelProperty<TestLinkIncludeDto>(_info);
//            return _dataModel;
//        }

//		/// <summary>
//        /// 读取ListDto的数据模型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        public VoloModelInfo ReadListModel()
//        {
//            var _model = BuilderHelper.DynamicReadModelProperty<TestLinkIncludeListDto>(new TestLinkIncludeListDto());
//			var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryTestLinkInclude());
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
//        public async Task<PasteResultDto<TestLinkIncludeListDto>> Page([FromQuery] InputQueryTestLinkInclude input)
//        {
//            var _query = from a in _dbContext.TestLinkInclude
//                            join b11 in _dbContext.UserInfo on a.UserId equals b11.Id into b12
//                            from b13 in b12.DefaultIfEmpty()
//                            join b21 in _dbContext.GradeInfo on a.GradeId equals b21.Id into b22
//                            from b23 in b22.DefaultIfEmpty()

//            select new TestLinkIncludeListDto
//            {
//                Id = a.Id,
//                        MyTable = a.MyTable,
//                        UserId = a.UserId,
//                                    ExtendUser = b13 != null ? b13.ToUserShort() : null,
//                        GradeId = a.GradeId,
//                                    ExtendGrade = b23 != null ? b23.ToGradeShort() : null,
//            };

//            _query = _query
//            ;


//            var _pagedto = new PasteResultDto<TestLinkIncludeListDto>();
//            if (input.page == 1)
//            {
//                _pagedto.TotalCount = await _query.CountAsync();
//               if(input.request=="one"){
//                  _pagedto.BuildModel(new TestLinkIncludeListDto(),new InputQueryTestLinkInclude()); 
//                }
//            }
//            var dataList = await _query
//                .OrderByDescending(x => x.Id)
//                .Page(input.page, input.size)
//                .AsNoTracking()
//                .ToListAsync();
//            if(dataList==null||dataList.Count == 0){
//               return null;// throw new PasteCodeException("没有查询到数据",204); //由前端处理这个204 由前端处理
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
//		[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
//        public async Task<TestLinkIncludeDto> CreateItemAsync(TestLinkIncludeAddDto input)
//        {
//            //var _userid = base.ReadCurrentUserId();
//            var info = ObjectMapper.Map<TestLinkIncludeAddDto, TestLinkInclude>(input);
//            //添加自定义
//            _dbContext.Add(info);
//                        info.UserId = base.ReadCurrentUserIdThrowException();
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<TestLinkInclude, TestLinkIncludeDto>(info);
//            return backinfo;
//        }
		
//        /// <summary>
//        /// 更新一个
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
//        public async Task<TestLinkIncludeDto> UpdateItemAsync(TestLinkIncludeUpdateDto input)
//        {
//            var info = await _dbContext.TestLinkInclude.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("需要查询的信息不存在", 404);
//            }
//            ObjectMapper.Map<TestLinkIncludeUpdateDto, TestLinkInclude>(input, info);
//            await _dbContext.SaveChangesAsync();
//            var backinfo = ObjectMapper.Map<TestLinkInclude, TestLinkIncludeDto>(info);
//            return backinfo;
//        }
		
		
//		/// <summary>
//        /// 删除数据，不需要删除的话注意注释这个 ListDto中附带disable标注忽略删除
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
//        public async Task<string> Remove(int id)
//        {
//            var info = await _dbContext.TestLinkInclude.Where(x => x.Id == id).FirstOrDefaultAsync();
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
