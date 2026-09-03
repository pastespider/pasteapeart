using PasteApeart.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xunit;

namespace PasteApeart.Test
{
    public class AttributeTest
    {

        /// <summary>
        /// 测试继承的顺序
        /// </summary>
        [Fact]
        public void TestBasePropertySort()
        {
            var read = BuilderHelper.AttributeReadModelProperty(new TestDto());
            Console.WriteLine(JsonSerializer.Serialize(read));
            Assert.NotNull(read);
        }

    }
}
