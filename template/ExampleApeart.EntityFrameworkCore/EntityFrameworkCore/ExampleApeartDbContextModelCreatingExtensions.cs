using $ext_safeprojectname$.usermodels;
using Microsoft.EntityFrameworkCore;

namespace $ext_safeprojectname$
{
    /// <summary>
    /// 
    /// </summary>
    public static class $ext_safeprojectname$DbContextModelCreatingExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        public static void Configure$ext_safeprojectname$(
            this ModelBuilder builder,
            Action<$ext_safeprojectname$ModelBuilderConfigurationOptions> optionsAction = null)
        {


            //Check.NotNull(builder, nameof(builder));

            var options = new $ext_safeprojectname$ModelBuilderConfigurationOptions(
                $ext_safeprojectname$DbProperties.DbTablePrefix,
                $ext_safeprojectname$DbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            /* Configure all entities here. Example:

            builder.Entity<Question>(b =>
            {
                //Configure table & schema name
                b.ToTable(options.TablePrefix + "Questions", options.Schema);
            
                //b.ConfigureByConvention();
            
                //Properties
                b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);
                
                //Relations
                b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

                //Indexes
                b.HasIndex(q => q.CreationTime);
            });
            */
            // *UserInfo*
            builder.Entity<UserInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "UserInfo", options.Schema);
                //b.ConfigureByConvention();
            });

            // *RoleInfo*
            builder.Entity<RoleInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "RoleInfo", options.Schema);
                //b.ConfigureByConvention();
            });

            // *GradeInfo*
            builder.Entity<GradeInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeInfo", options.Schema);
                //b.ConfigureByConvention();
            });

            // *GradeRole*
            builder.Entity<GradeRole>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeRole", options.Schema);
                //b.ConfigureByConvention();
            });

            //**UserInfo**
            builder.Entity<UserInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "UserInfo", options.Schema);
                //b.ConfigureByConvention();
            });

            //**UserGrade**
            builder.Entity<UserGrade>(b =>
            {
                b.ToTable(options.TablePrefix + "UserGrade", options.Schema);
                //b.ConfigureByConvention();
            });

            //**RoleInfo**
            builder.Entity<RoleInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "RoleInfo", options.Schema);
                //b.ConfigureByConvention();
            });

            //**GradeRole**
            builder.Entity<GradeRole>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeRole", options.Schema);
                //b.ConfigureByConvention();
            });

            //**GradeInfo**
            builder.Entity<GradeInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeInfo", options.Schema);
                //b.ConfigureByConvention();
            });

            //----------------------------------以下是测试，可以删除--------------------------------------------------


        }
    }
}