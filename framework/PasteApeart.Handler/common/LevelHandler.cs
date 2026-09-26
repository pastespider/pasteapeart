using Microsoft.EntityFrameworkCore;
using PasteApeart.Domain;
using PasteApeart.Domain.Entity;

namespace PasteApeart.Handler.Level
{
    /// <summary>
    /// 多层级 非递归多层级新增和更新
    /// </summary>
    public class LevelHandler : ILevelHandler
    {

        private readonly DbContext _dbContext;

        /// <summary>
        /// 每层最大支持的排序位数 5表示00000
        /// </summary>
        private const int _persize = 4;

        /// <summary>
        /// 最大层级个数，从0开始 比如4就是 0 1 2 3
        /// </summary>
        private const int _maxlevel = 6;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public LevelHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// 新增后调用，用于自动处理关系链
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task AfterAddAsync<T, Tkey>(ILevelEntity<Tkey> info) where T : class, ILevelEntity<Tkey>
        {
            info.SortStr = FullRootSortStr(info.Sort);//.ToString().PadLeft(_persize, '0') + "0".PadLeft((_maxlevel - 1) * _persize, '0');
            //if (EqualityComparer<Tkey>.Default.Equals(info.FatherId, default(Tkey)))
            //{
            //    info.FatherStr = $"{info.Id},";
            //    //info.RootId = info.Id;
            //    return;
            //}

            if (info.FatherId.Equals(default(Tkey)))
            {
                info.FatherStr = $"{info.Id},";
                return;
            }

            var dbSet = _dbContext.Set<T>();
            //var dbType = _dbContext.GetType();
            //var dbProperty = dbType.GetProperty(TableName);
            //if (dbProperty != null)
            //{
            //    var dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity>;
            if (dbSet != null)
            {
                var father = await dbSet
                    //.Where(x => EqualityComparer<Tkey>.Default.Equals(x.Id, info.FatherId))
                    .Where(x => x.Id.Equals(info.FatherId))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (father != null && father != default)
                {
                    info.FatherStr = $"{father.FatherStr}{info.Id},";
                    info.Level = (father.Level + 1);
                    info.FatherId = father.Id;
                    //info.RootId = father.RootId != 0 ? father.RootId : father.Id;
                    BuildSortStr(info, father);
                }
                else
                {
                    info.FatherId = default(Tkey);
                }
            }
            //}
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="oldFatherStr"></param>
        /// <param name="oldLevel"></param>
        /// <returns></returns>
        public async Task AfterUpdateAsync<T, Tkey>(ILevelEntity<Tkey> info, string oldFatherStr, int oldLevel) where T : class, ILevelEntity<Tkey>
        {
            //IQueryable<ILevelEntity> dbSet = null;
            var dbSet = _dbContext.Set<T>();
            //var dbType = _dbContext.GetType();
            //var dbProperty = dbType.GetProperty(TableName);
            //if (dbProperty != null)
            //{
            //    dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity>;
            //}
            if (dbSet == null)
            {
                return;
            }

            if (!default(Tkey).Equals(info.FatherId))
            {
                var father = await dbSet.Where(x => x.Id.Equals(info.FatherId)).AsNoTracking().FirstOrDefaultAsync();
                if (father != null && father != default)
                {
                    info.Level = father.Level + 1;
                    if (info.Level > _maxlevel)
                    {
                        throw new Exception($"当前业务层级超出最大设定MaxLevel={_maxlevel}，请调整业务逻辑后再试!");
                    }
                    info.FatherStr = $"{father.FatherStr}{info.Id},";
                    //info.RootId = father.RootId;
                    BuildSortStr(info, father);
                }
                else
                {
                    info.FatherId = default(Tkey);
                    info.FatherStr = $"{info.Id},";
                    info.Level = 0;
                    //info.RootId = info.Id;
                    BuildSortStr(info, null);
                }
            }
            else
            {
                info.FatherId = default(Tkey);
                info.FatherStr = $"{info.Id},";
                info.Level = 0;
                BuildSortStr(info, null);
            }
            oldLevel = oldLevel - (info.Level);
            if (!String.IsNullOrEmpty(oldFatherStr))
            {
                var suns = await dbSet.Where(x => !x.Id.Equals(info.Id) && x.FatherStr.StartsWith(oldFatherStr)).ToListAsync();
                if (suns != null && suns.Count > 0)
                {
                    foreach (var item in suns)
                    {
                        item.FatherStr = item.FatherStr.Replace(oldFatherStr, info.FatherStr);
                        item.Level = item.Level + oldLevel;
                        //item.RootId = info.RootId;
                        BuildFootSortStr(item, info);
                    }
                }
            }
            else
            {
                //需要执行全部，修正一下
            }
        }


        /// <summary>
        /// 基于FatherId和Sort重新计算Level SortStr FatherStr
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void AllResortByFatherIdAndSort<T, Tkey>(List<T> entities) where T : ILevelEntity<Tkey>
        {
            if (entities == null || entities.Count == 0) return;

            // 2. 找出顶级节点 (FatherId 为 0 或 null)
            //var roots = entities.Where(x => x.FatherId == 0).ToList();
            var roots = entities
                //.Where(x => EqualityComparer<Tkey>.Default.Equals(x.FatherId, default(Tkey)) || (x.FatherId?.ToString() == "0")  // 字符串"0"的情况
                .Where(x => default(Tkey).Equals(x.FatherId))
                    .ToList();
            // 3. 开启递归重算
            foreach (var root in roots)
            {
                if (root.Sort.ToString().Length > _persize)
                {
                    throw new Exception("排序值过大，请调整后再试");
                }
                root.Level = 0;
                root.FatherStr = $"{root.Id},";
                root.SortStr = BuildUnRootSortStr(0, root.Sort, "");
                RecalculateTree<T, Tkey>(root, entities);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="entities"></param>
        /// <exception cref="Exception"></exception>
        private static void RecalculateTree<T, Tkey>(T node, List<T> entities) where T : ILevelEntity<Tkey>
        {
            //寻找他的子集
            var childs = entities
                //.Where(x => EqualityComparer<Tkey>.Default.Equals(x.FatherId, node.Id))
                .Where(x => node.Id.Equals(x.FatherId))
                .ToList();
            if (childs?.Count > 0)
            {
                foreach (var child in childs)
                {
                    if (child.Sort.ToString().Length > _persize)
                    {
                        throw new Exception("排序值过大，请调整后再试");
                    }
                    child.Level = node.Level + 1;
                    child.FatherStr = $"{node.FatherStr}{child.Id},";
                    child.SortStr = BuildUnRootSortStr(child.Level, child.Sort, node.SortStr);

                    RecalculateTree<T, Tkey>(child, entities);
                }
            }
        }

        /// <summary>
        /// 基于排序获取默认串 这里指顶级的
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        private static string FullRootSortStr(int sort)
        {
            return sort.ToString().PadLeft(_persize, '0') + "0".PadLeft((_maxlevel - 1) * _persize, '0');
        }

        /// <summary>
        /// 构建当前层级的Sortstr
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="currentSort"></param>
        /// <param name="fatherSortstr">为空表示当前是顶级</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string BuildUnRootSortStr(int currentLevel, int currentSort, string fatherSortstr = "")
        {
            var fatherLevel = currentLevel - 1;
            if (currentLevel < _maxlevel)
            {
                if (string.IsNullOrEmpty(fatherSortstr))
                {
                    return FullRootSortStr(currentSort);//.ToString().PadLeft(_persize, '0') + "0".PadLeft(_persize * (_maxlevel - 1), '0');
                }
                else
                {
                    var str = fatherSortstr.Substring(0, (fatherLevel + 1) * _persize) + currentSort.ToString().PadLeft(_persize, '0');
                    if (currentLevel < (_maxlevel - 1))
                    {
                        str += "0".PadLeft(_persize * (_maxlevel - 1 - currentLevel), '0');
                    }
                    return str;
                }
            }
            else
            {
                throw new Exception($"当前超过设定的最大层级:{_maxlevel}，无法继续执行");
            }
        }

        /// <summary>
        /// 自动排序 基于同层级排序到后面，只有当当前是0的时候生效
        /// </summary>
        /// <param name="TableName">应该是PropertyName</param>
        /// <param name="input"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public async Task AutoSort<Tkey>(string TableName, ILevelEntity<Tkey> input, int step = 10)
        {
            if (input.Sort == 0)
            {
                var dbType = _dbContext.GetType();
                var dbProperty = dbType.GetProperty(TableName);
                if (dbProperty != null)
                {
                    var dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity<Tkey>>;
                    if (dbSet != null)
                    {

                        var _max = await dbSet
                            //.Where(x => EqualityComparer<Tkey>.Default.Equals(x.FatherId, input.FatherId))
                            .Where(x => x.FatherId.Equals(input.FatherId))
                            .OrderByDescending(x => x.Sort)
                            .Select(x => x.Sort)
                            .FirstOrDefaultAsync();
                        input.Sort = _max + 10;
                    }
                }
            }
        }


        /// <summary>
        /// 基于父级和当前自己的排序信息，构建对应的排序字符串
        /// </summary>
        /// <param name="currentEntity"></param>
        /// <param name="fatherEntity"></param>
        private static void BuildSortStr<Tkey>(ILevelEntity<Tkey> currentEntity, ILevelEntity<Tkey> fatherEntity)
        {
            if (currentEntity.Level < _maxlevel)
            {
                if (fatherEntity == null)
                {
                    currentEntity.SortStr = FullRootSortStr(currentEntity.Sort);//.ToString().PadLeft(_persize, '0') + "0".PadLeft(_persize * (_maxlevel - 1), '0');
                }
                else
                {
                    if (!string.IsNullOrEmpty(fatherEntity.SortStr))
                    {
                        currentEntity.SortStr = fatherEntity.SortStr.Substring(0, (fatherEntity.Level + 1) * _persize) + currentEntity.Sort.ToString().PadLeft(_persize, '0');
                        //补充尾巴
                        if (currentEntity.Level < (_maxlevel - 1))
                        {
                            currentEntity.SortStr += "0".PadLeft(_persize * (_maxlevel - 1 - currentEntity.Level), '0');
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 上级或者祖籍变更的替换，自己之后的不变,中间层级也不变 
        /// </summary>
        /// <param name="currentEntity"></param>
        /// <param name="fatherEntity">是自己的上级或者祖级</param>
        private static void BuildFootSortStr<Tkey>(ILevelEntity<Tkey> currentEntity, ILevelEntity<Tkey> fatherEntity)
        {
            var _old_sort_str = currentEntity.SortStr;
            currentEntity.SortStr = fatherEntity.SortStr.Substring(0, (fatherEntity.Level + 1) * _persize);
            currentEntity.SortStr += _old_sort_str.Substring((fatherEntity.Level + 1) * _persize);
        }

        #region 历史案例代码

        ///// <summary>
        ///// 这是一个新增案例
        /////</summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static async Task<RoleInfoDto> RoleAdd(IPasteClawDbContext _dbContext, RoleInfoAddDto input)
        //{
        //    if (input.Sort == 0)
        //    {
        //        var _max = await _dbContext.RoleInfo.Where(x => x.FatherId == input.FatherId)
        //            .OrderByDescending(x => x.Sort)
        //            .Select(x => x.Sort)
        //            .FirstOrDefaultAsync();
        //        input.Sort = _max + 10;
        //    }

        //    if (input.RoleType == 0)
        //    {
        //        if (String.IsNullOrEmpty(input.Model) || String.IsNullOrEmpty(input.Role))
        //        {
        //            throw new PasteCodeException("新增权限的时候，模块和权限必填，请重新填写！");
        //        }
        //        var find = await _dbContext.RoleInfo.Where(x => x.RoleType == 0 && x.Model == input.Model && x.Role == input.Role).Select(x => x.Id).FirstOrDefaultAsync();
        //        if (find > 0)
        //        {
        //            throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
        //        }
        //    }
        //    if (input.RoleType == EnumRoleType.Menu)
        //    {
        //        if (!string.IsNullOrEmpty(input.Path))
        //        {
        //            if (input.FatherId == 0)
        //            {
        //                throw new PasteCodeException("非一级菜单，父级必选，请重新选择！");
        //            }
        //            var find = await _dbContext.RoleInfo.Where(x => x.RoleType == EnumRoleType.Menu && x.Path == input.Path).Select(x => x.Id).FirstOrDefaultAsync();
        //            if (find > 0)
        //            {
        //                throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
        //            }
        //        }
        //    }

        //    //这里最好用事务包裹下
        //    //var info = ObjectMapper.Map<RoleInfoAddDto, RoleInfo>(input);
        //    var info = input.Adapt<RoleInfo>();
        //    info.IsEnable = true;//添加自定义
        //    _dbContext.Add(info);
        //    await _dbContext.SaveChangesAsync();
        //    //await AfterAdd(_dbContext, nameof(RoleInfo), info);
        //    await NewAfterAddAsync<RoleInfo, long>(_dbContext, info);
        //    await _dbContext.SaveChangesAsync();

        //    //var backinfo = ObjectMapper.Map<RoleInfo, RoleInfoDto>(info);
        //    var backinfo = info.Adapt<RoleInfoDto>();
        //    return backinfo;
        //}

        ///// <summary>
        ///// 这是一个更新案例
        /////</summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static async Task<RoleInfoDto> RoleUpdate(IPasteClawDbContext _dbContext, RoleInfoUpdateDto input)
        //{
        //    if (input.Sort == 0)
        //    {
        //        var _max = await _dbContext.RoleInfo.Where(x => x.FatherId == input.FatherId)
        //            .OrderByDescending(x => x.Sort)
        //            .Select(x => x.Sort)
        //            .FirstOrDefaultAsync();
        //        input.Sort = _max + 10;
        //    }

        //    if (input.RoleType == 0)
        //    {
        //        if (String.IsNullOrEmpty(input.Model) || String.IsNullOrEmpty(input.Role))
        //        {
        //            throw new PasteCodeException("新增权限的时候，模块和权限必填，请重新填写！");
        //        }
        //        var find = await _dbContext.RoleInfo.Where(x => x.RoleType == 0 && x.Model == input.Model && x.Role == input.Role && x.Id != input.Id).Select(x => x.Id).FirstOrDefaultAsync();
        //        if (find > 0)
        //        {
        //            throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
        //        }
        //    }
        //    if (input.RoleType == EnumRoleType.Menu)
        //    {
        //        if (!string.IsNullOrEmpty(input.Path))
        //        {
        //            if (input.FatherId == 0)
        //            {
        //                throw new PasteCodeException("非一级菜单，父级必选，请重新选择！");
        //            }
        //            var find = await _dbContext.RoleInfo.Where(x => x.RoleType == EnumRoleType.Menu && x.Path == input.Path && x.Id != input.Id).Select(x => x.Id).FirstOrDefaultAsync();
        //            if (find > 0)
        //            {
        //                throw new PasteCodeException("已经有这个菜单项了，请勿重复添加！");
        //            }
        //        }
        //    }

        //    var info = await _dbContext.RoleInfo.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
        //    if (info == null || info == default)
        //    {
        //        throw new PasteCodeException("需要查询的信息不存在");
        //    }
        //    var oldFather = info.FatherStr;
        //    var oldLevel = info.Level;
        //    //ObjectMapper.Map<RoleInfoUpdateDto, RoleInfo>(input, info);
        //    input.Adapt(info);
        //    //await AfterUpdate(_dbContext, nameof(RoleInfo), info);
        //    await NewAfterUpdateAsync<RoleInfo, long>(_dbContext, info, oldFather, oldLevel);
        //    //var backinfo = ObjectMapper.Map<RoleInfo, RoleInfoDto>(info);?
        //    var backinfo = info.Adapt<RoleInfoDto>();
        //    await _dbContext.SaveChangesAsync();
        //    return backinfo;
        //}

        ///// <summary>
        ///// 新增后 注意外部要保存下
        ///// </summary>
        ///// <param name="_dbContext"></param>
        ///// <param name="TableName">应该是PropertyName</param>
        ///// <param name="info"></param>
        //[Obsolete("Use NewAfterAddAsync")]
        //public static async Task AfterAdd<Tkey>(IPasteClawDbContext _dbContext, string TableName, ILevelEntity<Tkey> info)
        //{
        //    info.SortStr = info.Sort.ToString().PadLeft(_persize, '0') + "0".PadLeft((_maxlevel - 1) * _persize, '0');
        //    //if (EqualityComparer<Tkey>.Default.Equals(info.FatherId, default(Tkey)))
        //    if (default(Tkey).Equals(info.FatherId))
        //    {
        //        info.FatherStr = $"{info.Id},";
        //        //info.RootId = info.Id;
        //        return;
        //    }

        //    var dbType = _dbContext.GetType();
        //    var dbProperty = dbType.GetProperty(TableName);
        //    if (dbProperty != null)
        //    {
        //        var dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity<Tkey>>;
        //        if (dbSet != null)
        //        {

        //            var father = await dbSet
        //                //.Where(x => EqualityComparer<Tkey>.Default.Equals(x.Id, info.FatherId))
        //                .Where(x => x.Id.Equals(info.FatherId))
        //                .AsNoTracking()
        //                .FirstOrDefaultAsync();
        //            if (father != null && father != default)
        //            {
        //                info.FatherStr = $"{father.FatherStr}{info.Id},";
        //                info.Level = (father.Level + 1);
        //                info.FatherId = father.Id;
        //                //info.RootId = father.RootId != 0 ? father.RootId : father.Id;
        //                BuildSortStr(info, father);
        //            }
        //            else
        //            {
        //                info.FatherId = default(Tkey);
        //            }

        //        }
        //    }
        //}

        ///// <summary>
        ///// 更新后 注意外部要保存下 这里有问题 要基于之前的关系链查找
        ///// </summary>
        ///// <param name="_dbContext"></param>
        ///// <param name="TableName">应该是PropertyName</param>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //[Obsolete("use NewAfterUpdate")]
        //public static async Task AfterUpdate<Tkey>( string TableName, ILevelEntity<Tkey> info)
        //{
        //    IQueryable<ILevelEntity<Tkey>> dbSet = null;
        //    var dbType = _dbContext.GetType();
        //    var dbProperty = dbType.GetProperty(TableName);
        //    if (dbProperty != null)
        //    {
        //        dbSet = dbProperty.GetValue(_dbContext) as IQueryable<ILevelEntity<Tkey>>;
        //    }
        //    if (dbSet == null)
        //    {
        //        return;
        //    }
        //    var old_father_str = info.FatherStr;
        //    var _chat_level = info.Level;

        //    //if (!EqualityComparer<Tkey>.Default.Equals(info.FatherId, default(Tkey)))
        //    if (default(Tkey).Equals(info.FatherId))
        //    {
        //        var father = await dbSet
        //             //.Where(x => EqualityComparer<Tkey>.Default.Equals(x.Id,info.FatherId))
        //             .Where(x => x.Id.Equals(info.FatherId))
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync();
        //        if (father != null && father != default)
        //        {
        //            info.Level = father.Level + 1;
        //            info.FatherStr = $"{father.FatherStr}{info.Id},";
        //            //info.RootId = father.RootId;
        //            BuildSortStr(info, father);
        //        }
        //        else
        //        {
        //            info.FatherId = default(Tkey);
        //            info.FatherStr = $"{info.Id},";
        //            info.Level = 0;
        //            //info.RootId = info.Id;
        //            BuildSortStr(info, null);
        //        }
        //    }
        //    else
        //    {
        //        info.FatherId = default(Tkey);
        //        info.FatherStr = $"{info.Id},";
        //        info.Level = 0;
        //        //info.RootId = info.Id;
        //        BuildSortStr(info, null);
        //    }
        //    _chat_level = _chat_level - (info.Level);
        //    if (!String.IsNullOrEmpty(old_father_str))
        //    {
        //        var suns = await dbSet.Where(x => !x.Id.Equals(info.Id) && x.FatherStr.StartsWith(old_father_str)).ToListAsync();
        //        if (suns != null && suns.Count > 0)
        //        {
        //            foreach (var item in suns)
        //            {
        //                item.FatherStr = item.FatherStr.Replace(old_father_str, info.FatherStr);
        //                item.Level = item.Level + _chat_level;
        //                //item.RootId = info.RootId;
        //                BuildFootSortStr(item, info);
        //            }
        //        }
        //    }
        //}


        #endregion

    }
}
