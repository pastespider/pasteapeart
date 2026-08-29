using ExampleApeart.usermodels;
using Microsoft.EntityFrameworkCore;

namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExampleApeartDbContextModelCreatingExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        public static void ConfigureExampleApeart(
            this ModelBuilder builder,
            Action<ExampleApeartModelBuilderConfigurationOptions> optionsAction = null)
        {


            //Check.NotNull(builder, nameof(builder));

            var options = new ExampleApeartModelBuilderConfigurationOptions(
                ExampleApeartDbProperties.DbTablePrefix,
                ExampleApeartDbProperties.DbSchema
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