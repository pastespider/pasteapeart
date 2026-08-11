using Microsoft.AspNetCore.Mvc.Filters;
using PasteApeart.EntityFrameworkCore;

namespace PasteApeart.Application
{
    /// <summary>
    /// 事务回滚
    /// </summary>
    public class PasteTransactionFilter : IAsyncActionFilter
    {

        /*
         使用方式
                [TypeFilter(typeof(PasteTransactionFilter))]

         */

        /// <summary>
        /// 注意这个要申明为Scop方式，不能用瞬时方式
        /// </summary>
        private IPasteApeartDbContext _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public PasteTransactionFilter(IPasteApeartDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 执行控制器方法（包含所有SaveChanges调用）
                var resultContext = await next();

                // 检查是否有未处理的异常
                if (resultContext.Exception == null || resultContext.ExceptionHandled)
                {
                    // 没有异常或异常已处理，提交事务
                    await transaction.CommitAsync();
                }
                else
                {
                    // 发生未处理异常，回滚事务
                    await transaction.RollbackAsync();
                }
            }
            catch
            {
                // 发生异常，确保回滚
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

}
