using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace ExampleForm.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoWriteInterceptor : SaveChangesInterceptor
    {
        private ICurrentUser _currentUser;
        /// <summary>
        /// 
        /// </summary>
        public AutoWriteInterceptor(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetReportDatesForAddedEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetReportDatesForAddedEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void SetReportDatesForAddedEntities(DbContext context)
        {
            if (context == null) return;

            //if (_currentUser != null)
            //{
            //    Console.WriteLine(_currentUser?.UserName+_currentUser?.Name+_currentUser?.SurName);
            //}
            //else
            //{
            //    Console.WriteLine("AutoWrite CurrentUser is null !");
            //}

            //foreach (var entry in context.ChangeTracker.Entries<IKeyMonthProperty>())
            //{
            //    // 只处理新增实体且ReportDate为空的情况
            //    if (entry.State == EntityState.Added && entry.Entity.KeyMonth == 0)
            //    {
            //        entry.Entity.KeyMonth = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

            //        // 如果需要标记属性为已修改（某些情况下需要）
            //        entry.Property(x => x.KeyMonth).IsModified = true;
            //    }
            //}

            //foreach (var entry in context.ChangeTracker.Entries<IMemberKeyProperty>())
            //{
            //    // 只处理新增实体且ReportDate为空的情况
            //    if (entry.State == EntityState.Added && entry.Entity.MemberKey == 0)
            //    {
            //        entry.Entity.MemberKey = (int)(entry.Entity.MemberId % 100);
            //        // 如果需要标记属性为已修改（某些情况下需要） 新增的话，应该是不需要的
            //        entry.Property(x => x.MemberKey).IsModified = true;
            //    }
            //}

        }
    }
}
