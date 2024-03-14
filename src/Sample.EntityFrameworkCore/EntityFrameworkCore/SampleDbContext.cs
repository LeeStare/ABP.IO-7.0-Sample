using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Sample.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class SampleDbContext :
    AbpDbContext<SampleDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    // 避免多台主機連接第三方登入發生Exception
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // ========= 內建 Identity =========
    /// <summary>
    /// 使用者(帳號)
    /// </summary>
    public DbSet<IdentityUser> Users { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public DbSet<IdentityRole> Roles { get; set; }

    /// <summary>
    /// 使用者角色關聯表
    /// </summary>
    public DbSet<IdentityUserRole> UserRole { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }

    /// <summary>
    /// 組織
    /// </summary>
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    /// <summary>
    /// 登入紀錄表
    /// </summary>
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }

    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // ========= Tenant Management =========
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion Entities from the modules

    // ========= 自訂資料表 Custom Tables =========

    #region 範例

    /// <summary>
    /// 範例實體
    /// </summary>
    //public DbSet<SampleEntity> SampleEntity { get; set; }

    #endregion 範例


    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {

    }

    /// <summary>
    /// Migration 建表用
    /// </summary>
    /// <param name=
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(SampleConsts.DbTablePrefix + "YourEntities", SampleConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        // ========= 自訂資料表 Custom Tables =========


        #region 範例

        // === 範例 ===
        //builder.Entity<SampleEntity>(b =>
        //{
        //    b.ToTable("SampleEntity");

        //    b.ConfigureByConvention();

        //    b.Property(x => x.Id)
        //    .IsRequired();
        //});

        #endregion 範例
    }
}
