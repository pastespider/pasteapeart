using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 
    /// </summary>
    public class IncludeTable:Entity<int>
    {

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(16)]
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(64)]
        public string Desc { get; set; }

    }
}
