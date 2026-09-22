//using Mapster;
//using Microsoft.EntityFrameworkCore;
//using ExampleForm.Application.Contracts;
//using ExampleForm.usermodels;
//using Volo.Abp.ObjectMapping;
//using Z.EntityFramework.Plus;
//using PasteApeart.Domain.Entity;

//namespace ExampleForm.Handler
//{
//    /// <summary>
//    /// 多层级 非递归多层级新增和更新
//    /// </summary>
//    public class LevelHandler
//    {
//        /// <summary>
//        /// 这是一个新增案例
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public static async Task<RoleInfoDto> RoleAdd(IExampleFormDbContext _dbContext, IObjectMapper ObjectMapper, RoleInfoAddDto input)
//        {
//            if (input.Sort == 0)
//            {
//                var _max = await _dbContext.RoleInfo.Where(x => x.FatherId == input.FatherId)
//                    .OrderByDescending(x => x.Sort)
//                    .Select(x => x.Sort)
//                    .FirstOrDefaultAsync();
//                input.Sort = _max + 10;
//            }

//            if (input.RoleType == 0)
//            {
//                if (String.IsNullOrEmpty(input.Model) || String.IsNullOrEmpty(input.Role))
//                {
//                    throw new PasteCodeException("新增权限的时候，模块和权限必填，请重新填写！");
//                }
//                var find = await _dbContext.RoleInfo.Where(x => x.RoleType == 0 && x.Model == input.Model && x.Role == input.Role).Select(x => x.Id).FirstOrDefaultAsync();
//                if (find > 0)
//                {
//                    throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
//                }
//            }
//            if (input.RoleType == EnumRoleType.Menu)
//            {
//                if (!string.IsNullOrEmpty(input.Path))
//                {
//                    if (input.FatherId == 0)
//                    {
//                        throw new PasteCodeException("非一级菜单，父级必选，请重新选择！");
//                    }
//                    var find = await _dbContext.RoleInfo.Where(x => x.RoleType == EnumRoleType.Menu && x.Path == input.Path).Select(x => x.Id).FirstOrDefaultAsync();
//                    if (find > 0)
//                    {
//                        throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
//                    }
//                }
//            }

//            //这里最好用事务包裹下
//            //var info = ObjectMapper.Map<RoleInfoAddDto, RoleInfo>(input);
//            var info = input.Adapt<RoleInfo>();
//            info.IsEnable = true;//添加自定义
//            _dbContext.Add(info);
//            await _dbContext.SaveChangesAsync();
//            await AfterAdd(_dbContext, nameof(RoleInfo), info);
//            await _dbContext.SaveChangesAsync();

//            //var backinfo = ObjectMapper.Map<RoleInfo, RoleInfoDto>(info);
//            var backinfo = info.Adapt<RoleInfoDto>();
//            return backinfo;
//        }

//        /// <summary>
//        /// 这是一个更新案例
//        ///</summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public static async Task<RoleInfoDto> RoleUpdate(IExampleFormDbContext _dbContext, IObjectMapper ObjectMapper, RoleInfoUpdateDto input)
//        {
//            if (input.Sort == 0)
//            {
//                var _max = await _dbContext.RoleInfo.Where(x => x.FatherId == input.FatherId)
//                    .OrderByDescending(x => x.Sort)
//                    .Select(x => x.Sort)
//                    .FirstOrDefaultAsync();
//                input.Sort = _max + 10;
//            }

//            if (input.RoleType == 0)
//            {
//                if (String.IsNullOrEmpty(input.Model) || String.IsNullOrEmpty(input.Role))
//                {
//                    throw new PasteCodeException("新增权限的时候，模块和权限必填，请重新填写！");
//                }
//                var find = await _dbContext.RoleInfo.Where(x => x.RoleType == 0 && x.Model == input.Model && x.Role == input.Role && x.Id != input.Id).Select(x => x.Id).FirstOrDefaultAsync();
//                if (find > 0)
//                {
//                    throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
//                }
//            }
//            if (input.RoleType == EnumRoleType.Menu)
//            {
//                if (!string.IsNullOrEmpty(input.Path))
//                {
//                    if (input.FatherId == 0)
//                    {
//                        throw new PasteCodeException("非一级菜单，父级必选，请重新选择！");
//                    }
//                    var find = await _dbContext.RoleInfo.Where(x => x.RoleType == EnumRoleType.Menu && x.Path == input.Path && x.Id != input.Id).Select(x => x.Id).FirstOrDefaultAsync();
//                    if (find > 0)
//                    {
//                        throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
//                    }
//                }
//            }

//            var info = await _dbContext.RoleInfo.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
//            if (info == null || info == default)
//            {
//                throw new PasteCodeException("需要查询的信息不存在");
//            }
//            //ObjectMapper.Map<RoleInfoUpdateDto, RoleInfo>(input, info);
//            input.Adapt(info);
//            await AfterUpdate(_dbContext, nameof(RoleInfo), info);
//            //var backinfo = ObjectMapper.Map<RoleInfo, RoleInfoDto>(info);?
//            var backinfo = info.Adapt<RoleInfoDto>();
//            await _dbContext.SaveChangesAsync();
//            return backinfo;
//        }

//        /// <summary>
//        /// 新增后 注意外部要保存下
//        /// </summary>
//        /// <param name="_dbContext"></param>
//        /// <param name="TableName">应该是PropertyName</param>
//        /// <param name="info"></param>
//        public static async Task AfterAdd(IExampleFormDbContext _dbContext, string TableName, ILevelEntity info)
//        {
//            info.SortStr = info.Sort.ToString().PadLeft(_persize, '0') + "0".PadLeft((_maxlevel - 1) * _persize, '0');
//            if (info.FatherId <= 0)
//            {
//                info.FatherStr = $"{info.Id},";
//                info.RootId = info.Id;
//                return;
//            }

//            var dbType = _dbContext.GetType();
//            var dbProperty = dbType.GetProperty(TableName);
//            if (dbProperty != null)
//            {
//                var dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity>;
//                if (dbSet != null)
//                {

//                    var father = await dbSet.Where(x => x.Id == info.FatherId).AsNoTracking().FirstOrDefaultAsync();
//                    if (father != null && father != default)
//                    {
//                        info.FatherStr = $"{father.FatherStr}{info.Id},";
//                        info.Level = (father.Level + 1);
//                        info.FatherId = father.Id;
//                        info.RootId = father.RootId != 0 ? father.RootId : father.Id;
//                        BuildSortStr(info, father);
//                    }
//                    else
//                    {
//                        info.FatherId = 0;
//                    }

//                }
//            }
//        }

//        /// <summary>
//        /// 更新后 注意外部要保存下
//        /// </summary>
//        /// <param name="_dbContext"></param>
//        /// <param name="TableName">应该是PropertyName</param>
//        /// <param name="info"></param>
//        /// <returns></returns>
//        public static async Task AfterUpdate(IExampleFormDbContext _dbContext, string TableName, ILevelEntity info)
//        {
//            IQueryable<ILevelEntity> dbSet = null;
//            var dbType = _dbContext.GetType();
//            var dbProperty = dbType.GetProperty(TableName);
//            if (dbProperty != null)
//            {
//                dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity>;
//            }
//            if (dbSet == null)
//            {
//                return;
//            }
//            var old_father_str = info.FatherStr;
//            var _chat_level = info.Level;

//            if (info.FatherId != 0)
//            {
//                var father = await dbSet.Where(x => x.Id == info.FatherId).AsNoTracking().FirstOrDefaultAsync();
//                if (father != null && father != default)
//                {
//                    info.Level = father.Level + 1;
//                    info.FatherStr = $"{father.FatherStr}{info.Id},";
//                    info.RootId = father.RootId;
//                    BuildSortStr(info, father);
//                }
//                else
//                {
//                    info.FatherId = 0;
//                    info.FatherStr = $"{info.Id},";
//                    info.Level = 0;
//                    info.RootId = info.Id;
//                    BuildSortStr(info, null);
//                }
//            }
//            else
//            {
//                info.FatherId = 0;
//                info.FatherStr = $"{info.Id},";
//                info.Level = 0;
//                info.RootId = info.Id;
//                BuildSortStr(info, null);
//            }
//            _chat_level = _chat_level - (info.Level);
//            if (!String.IsNullOrEmpty(old_father_str))
//            {
//                var suns = await dbSet.Where(x => x.Id != info.Id && x.FatherStr.StartsWith(old_father_str)).ToListAsync();
//                if (suns != null && suns.Count > 0)
//                {
//                    foreach (var item in suns)
//                    {
//                        item.FatherStr = item.FatherStr.Replace(old_father_str, info.FatherStr);
//                        item.Level = item.Level + _chat_level;
//                        item.RootId = info.RootId;
//                        BuildFootSortStr(item, info);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 自动排序 基于同层级排序到后面，只有当当前是0的时候生效
//        /// </summary>
//        /// <param name="_dbContext"></param>
//        /// <param name="TableName">应该是PropertyName</param>
//        /// <param name="input"></param>
//        /// <param name="step"></param>
//        /// <returns></returns>
//        public static async Task AutoSort(IExampleFormDbContext _dbContext, string TableName, ILevelEntity input,int step=10)
//        {
//            if (input.Sort == 0)
//            {
//                var dbType = _dbContext.GetType();
//                var dbProperty = dbType.GetProperty(TableName);
//                if (dbProperty != null)
//                {
//                    var dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity>;
//                    if (dbSet != null)
//                    {

//                        var _max = await dbSet.Where(x => x.FatherId == input.FatherId)
//                            .OrderByDescending(x => x.Sort)
//                            .Select(x => x.Sort)
//                            .FirstOrDefaultAsync();
//                        input.Sort = _max + 10;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 每层最大支持的排序位数 5表示00000
//        /// </summary>
//        private const int _persize = 5;
//        /// <summary>
//        /// 最大层级个数，从0开始 比如4就是 0 1 2 3
//        /// </summary>
//        private const int _maxlevel = 4;

//        /// <summary>
//        /// 基于父级和当前自己的排序信息，构建对应的排序字符串
//        /// </summary>
//        /// <param name="_info"></param>
//        /// <param name="_father"></param>
//        private static void BuildSortStr(ILevelEntity _info, ILevelEntity _father)
//        {
//            if (_info.Level < _maxlevel)
//            {
//                if (_father == null)
//                {
//                    _info.SortStr = _info.Sort.ToString().PadLeft(_persize, '0') + "0".PadLeft(_persize * (_maxlevel - 1), '0');
//                }
//                else
//                {
//                    if (!string.IsNullOrEmpty(_father.SortStr))
//                    {
//                        _info.SortStr = _father.SortStr.Substring(0, (_father.Level + 1) * _persize) + _info.Sort.ToString().PadLeft(_persize, '0');
//                        //补充尾巴
//                        if (_info.Level < (_maxlevel - 1))
//                        {
//                            _info.SortStr += "0".PadLeft(_persize * (_maxlevel - 1 - _info.Level), '0');
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 上级或者祖籍变更的替换，自己之后的不变,中间层级也不变 
//        /// </summary>
//        /// <param name="_info"></param>
//        /// <param name="_father">是自己的上级或者祖级</param>
//        private static void BuildFootSortStr(ILevelEntity _info, ILevelEntity _father)
//        {
//            var _old_sort_str = _info.SortStr;
//            _info.SortStr = _father.SortStr.Substring(0, (_father.Level + 1) * _persize);
//            _info.SortStr += _old_sort_str.Substring((_father.Level + 1) * _persize);
//        }

//    }
//}
