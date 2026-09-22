using ExampleForm.EntityFrameworkCore;
using ExampleForm.testmodels;
using ExampleForm.toolmodels;
using ExampleForm.usermodels;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExampleFormDbContextModelCreatingExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        public static void ConfigureExampleForm(
            this ModelBuilder builder,
            Action<ExampleFormModelBuilderConfigurationOptions> optionsAction = null)
        {


            Check.NotNull(builder, nameof(builder));

            var options = new ExampleFormModelBuilderConfigurationOptions(
                ExampleFormDbProperties.DbTablePrefix,
                ExampleFormDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            /* Configure all entities here. Example:

            builder.Entity<Question>(b =>
            {
                //Configure table & schema name
                b.ToTable(options.TablePrefix + "Questions", options.Schema);
            
                b.ConfigureByConvention();
            
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
                b.ConfigureByConvention();
            });

            // *RoleInfo*
            builder.Entity<RoleInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "RoleInfo", options.Schema);
                b.ConfigureByConvention();
            });

            // *GradeInfo*
            builder.Entity<GradeInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeInfo", options.Schema);
                b.ConfigureByConvention();
            });

            // *GradeRole*
            builder.Entity<GradeRole>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeRole", options.Schema);
                b.ConfigureByConvention();
            });

            //**UserInfo**
            builder.Entity<UserInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "UserInfo", options.Schema);
                b.ConfigureByConvention();
            });

            //**UserGrade**
            builder.Entity<UserGrade>(b =>
            {
                b.ToTable(options.TablePrefix + "UserGrade", options.Schema);
                b.ConfigureByConvention();
            });

            //**RoleInfo**
            builder.Entity<RoleInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "RoleInfo", options.Schema);
                b.ConfigureByConvention();
            });

            //**GradeRole**
            builder.Entity<GradeRole>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeRole", options.Schema);
                b.ConfigureByConvention();
            });

            //**GradeInfo**
            builder.Entity<GradeInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "GradeInfo", options.Schema);
                b.ConfigureByConvention();
            });

            //----------------------------------以下是测试，可以删除--------------------------------------------------

            //**TestTable**
            builder.Entity<TestTable>(b =>
            {
                b.ToTable(options.TablePrefix + "TestTable", options.Schema);
                b.ConfigureByConvention();
            });


            //**IncludeTable**
            builder.Entity<IncludeTable>(b =>
            {
                b.ToTable(options.TablePrefix + "IncludeTable", options.Schema);
                b.ConfigureByConvention();
            });


            //**MyTable**
            builder.Entity<MyTable>(b =>
            {
                b.ToTable(options.TablePrefix + "MyTable", options.Schema);
                b.ConfigureByConvention();
            });

            //**GuidTest1**
            builder.Entity<GuidTest1>(b =>
            {
                b.ToTable(options.TablePrefix + "GuidTest1", options.Schema);
                b.Property(x => x.Id).HasValueGenerator<SequentialGuidValueGenerator>().ValueGeneratedOnAdd();
                b.ConfigureByConvention();
            });

            //**UintTest1**
            builder.Entity<UintTest1>(b =>
            {
                b.ToTable(options.TablePrefix + "UintTest1", options.Schema);
                b.ConfigureByConvention();
            });

            //**ParentTable**
            builder.Entity<ParentTable>(b =>
            {
                b.ToTable(options.TablePrefix + "ParentTable", options.Schema);
                b.ConfigureByConvention();
            });

            //**UploadImageInfo**
            builder.Entity<UploadImageInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "UploadImageInfo", options.Schema);
                b.ConfigureByConvention();
            });

            //**TestLinkInclude**
            builder.Entity<TestLinkInclude>(b =>
            {
                b.ToTable(options.TablePrefix + "TestLinkInclude", options.Schema);
                b.ConfigureByConvention();
            });

        }
    }
}