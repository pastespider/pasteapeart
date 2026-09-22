using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Volo.Abp.Guids;

namespace ExampleForm.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class SequentialGuidValueGenerator : ValueGenerator<Guid>
    {
        /// <summary>
        /// 初始化的时候记得赋值
        /// </summary>
        public static IGuidGenerator _guidGenerator;

        /// <summary>
        /// 
        /// </summary>
        public SequentialGuidValueGenerator()
        {
            //_guidGenerator = guidGenerator;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public override Guid Next(EntityEntry entry)
            => _guidGenerator.Create(); // 生成有序 GUID
        /// <summary>
        /// 
        /// </summary>
        public override bool GeneratesTemporaryValues => false;
    }
}
