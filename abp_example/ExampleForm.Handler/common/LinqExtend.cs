using ExampleForm.testmodels;
using ExampleForm.usermodels;

namespace ExampleForm.Handler
{
    public static class LinqExtend
    {


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[Obsolete("use ToGradeShort")]
        //public static GradeShort ToShortGrade(this GradeInfo input)
        //{
        //    if (input != null)
        //    {
        //        return new GradeShort
        //        {
        //            Id = input.Id,
        //            Name = input.Name,
        //        };
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

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
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete("use ToRoleShort")]
        public static RoleShort ToShortRole(this RoleInfo input)
        {
            if (input == null)
            {
                return null;
            }
            return new RoleShort
            {
                Id = input.Id,
                Name = input.Name,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static RoleShort ToRoleShort(this RoleInfo input)
        {
            if (input == null)
            {
                return null;
            }
            return new RoleShort
            {
                Id = input.Id,
                Name = input.Name,
            };
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static UserShortModel ToShortUser(this UserInfo input)
        //{
        //    if (input == null) { return null; }
        //    return new UserShortModel
        //    {
        //        Id = input.Id,
        //        UserHead = input.UserHead,
        //        UserName = input.UserName,
        //    };
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserShort ToUserShort(this UserInfo user)
        {
            return new UserShort
            {
                Id = user.Id,
                UserHead = user.UserHead,
                UserName = user.UserName,
            };
        }


        /// <summary>
        /// 测试表 用于测试CURD的表
        /// </summary>
        public static TestShort ToTestShort(this TestTable input)
        {
            if (input != null)
            {
                return new TestShort
                {
                    Id = input.Id,
                    Name = input.Name
                };
            }
            else
            {
                return null;
            }
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

        public static RoleInfoListDto ToRoleListDto(this RoleInfo a, RoleInfo role)
        {
            if (a == null)
            {
                return null;
            }
            return new RoleInfoListDto
            {
                Id = a.Id,
                Desc = a.Desc,
                FatherId = a.FatherId,
                Icon = a.Icon,
                IsEnable = a.IsEnable,
                Level = a.Level,
                Model = a.Model,
                Name = a.Name,
                Path = a.Path,
                Role = a.Role,
                RoleType = a.RoleType,
                SortStr = a.SortStr,
                Sort = a.Sort,
                ExtendFather = role != null ? role.ToRoleShort() : null
            };
        }

    }
}
