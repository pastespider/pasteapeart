using ExampleApeart.EntityFrameworkCore;
using ExampleApeart.usermodels;
using System.Linq.Expressions;

namespace ExampleApeart.Handler
{
    public static class LinqExtend
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static GradeShort ToGradeShort(this GradeInfo input)
        {
            if (input != null)
            {
                return new GradeShort
                {
                    Id = input.Id,
                    Name = input.Name,
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserShort ToUserShort(this UserInfo user)
        {
            if (user == null)
            {
                return null;
            }
            return new UserShort
            {
                Id = user.Id,
                UserHead = user.UserHead,
                UserName = user.UserName,
            };
        }


        /// <summary>
        /// 测试扩展读取
        /// </summary>
        /// <param name="a"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public static RoleInfoUpdateDto ToRoleUpdateDto(this RoleInfo a, RoleInfo rol)
        {

            return new RoleInfoUpdateDto
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<UserShort> ToQueryUserShort(this IExampleApeartDbContext dbContext, Expression<Func<UserInfo, bool>> filter = null)
        {
            var query = dbContext.UserInfo.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Select(x => new UserShort
            {
                Id = x.Id,
                UserHead = x.UserHead,
                UserName = x.UserName
            });
        }


        /// <summary>
        /// 权限列表 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<RoleShort> ToQueryRoleShort(this IExampleApeartDbContext dbContext, Expression<Func<RoleInfo, bool>> filter = null)
        {
            var query = dbContext.RoleInfo.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Select(x => new RoleShort
            {
                Id = x.Id,
                Name = x.Name,
            });
        }

        /// <summary>
        /// 权限列表
        /// <param name="input"></param>
        /// </summary>
        public static RoleShort ToRoleShort(this RoleInfo input)
        {
            if (input == null) { return null; }
            return new RoleShort
            {
                Id = input.Id,
                Name = input.Name
            };
        }

    }
}
