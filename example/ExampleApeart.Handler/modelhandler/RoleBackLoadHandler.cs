using ExampleApeart.EntityFrameworkCore;
using ExampleApeart.usermodels;
using Mapster;
using Microsoft.EntityFrameworkCore;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using System.Text.Json;

namespace ExampleApeart.Handler
{
    public class RoleBackLoadHandler
    {


        /// <summary>
        /// 备份数据
        /// </summary>
        /// <param name="save"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public static async Task<dynamic> BackupAsync(IExampleApeartDbContext _dbContext, bool save = true)
        {
            if (!System.IO.Directory.Exists("backup"))
            {
                System.IO.Directory.CreateDirectory("backup");
            }
            var _target = Path.Combine("backup", "roles.json");
            var list = new List<RoleInfoAddDto>();
            var datas = new List<RoleInfoAddDto>();
            //设定最高5级
            for (var l = 0; l < 5; l++)
            {
                //用户可能更改level从0还是从1开始
                var roles = await _dbContext.RoleInfo.Where(x => x.Level == l).OrderBy(x => x.SortStr).ThenBy(x => x.Id).AsNoTracking().ToListAsync();
                if (roles != null && roles.Count > 0)
                {
                    foreach (var item in roles)
                    {
                        //var _tag = ObjectMapper.Map<RoleInfo, RoleInfoAddDto>(item);
                        var _tag = item.Adapt<RoleInfoAddDto>();
                        datas.Add(_tag);
                        if (item.FatherId != 0)
                        {
                            var find = datas.Where(x => x.Id == item.FatherId).FirstOrDefault();
                            if (find != null && find != default)
                            {
                                if (find.Items == null)
                                {
                                    find.Items = new List<RoleInfoAddDto>();
                                }
                                find.Items.Add(_tag);
                            }
                        }

                    }
                }
                else
                {
                    if (l > 1)
                    {
                        break;
                    }
                }
            }
            if (datas != null && datas.Count > 0)
            {
                var tags = datas.Where(x => x.FatherId == 0).OrderBy(x => x.Sort).ToList();
                if (tags != null && tags.Count > 0)
                {
                    if (save)
                    {
                        var _jsonString = JsonSerializer.Serialize(tags, JsonConfig.Options);
                        if (System.IO.File.Exists(_target))
                        {
                            System.IO.File.Delete(_target);
                        }
                        System.IO.File.WriteAllText(_target, _jsonString);
                    }
                    return tags;
                }
                else
                {
                    throw new PasteCodeException("没有获取到数据");
                }
            }
            else
            {
                throw new PasteCodeException("没有获取到数据");
            }
        }

        /// <summary>
        /// 载入数据
        /// </summary>
        /// <returns></returns>
        public static async Task<string> LoadAsync(IExampleApeartDbContext _dbContext, bool tran = true)
        {
            var _target = Path.Combine("backup", "roles.json");
            if (!System.IO.File.Exists(_target))
            {
                _target = Path.Combine("config", "roles.json");
            }
            if (System.IO.File.Exists(_target))
            {
                var _read = System.IO.File.ReadAllText(_target);
                if (String.IsNullOrEmpty(_read))
                {
                    throw new PasteCodeException("备份文件不存在");
                }
                //var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoleInfoAddDto>>(_read);
                var list = System.Text.Json.JsonSerializer.Deserialize<List<RoleInfoAddDto>>(_read, JsonConfig.Options);
                if (list != null && list.Count > 0)
                {
                    try
                    {
                        if (tran)
                        {
                            await _dbContext.Database.BeginTransactionAsync();
                        }
                        await _dbContext.GradeRole.Where(x => true).ExecuteDeleteAsync();
                        await _dbContext.RoleInfo.Where(x => true).ExecuteDeleteAsync();
                        foreach (var item in list)
                        {
                            await IncludeInsertAsync(_dbContext, item);
                        }
                        if (tran)
                        {
                            await _dbContext.Database.CommitTransactionAsync();
                        }
                    }
                    catch (Exception exl)
                    {
                        if (tran)
                        {
                            await _dbContext.Database.RollbackTransactionAsync();
                        }
                        throw new PasteCodeException(exl.Message);
                    }
                }
                else
                {
                    return "备份文件不存在";
                }
            }
            else
            {
                return "备份文件不存在";
            }
            return "";
        }

        /// <summary>
        /// 递归写入
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static async Task IncludeInsertAsync(IExampleApeartDbContext _dbContext, RoleInfoAddDto item)
        {
            //var _add = await LevelHandler.RoleAdd(_dbContext, item);
            var _add = item.Adapt<RoleInfo>();
            _dbContext.Add(_add);
            await _dbContext.SaveChangesAsync();
            if (item.Items != null)
            {
                foreach (var ii in item.Items)
                {
                    ii.FatherId = _add.Id;
                    //var _new = await _userHandler.RoleAdd(_dbContext,ii);
                    await IncludeInsertAsync(_dbContext, ii);
                }
            }
        }

    }

}
