using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace $ext_safeprojectname$
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthTagDescriptions : IDocumentFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag{ Name="UserInfo",Description="账号列表"},
                new OpenApiTag{ Name="GradeInfo",Description="角色列表"},
                new OpenApiTag{ Name="RoleInfo",Description="权限列表"},
                new OpenApiTag{ Name="GradeRole",Description="角色权限"}
            };
        }
    }
}
