using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.EntityFrameworkCore;
using Sample.Localization;
using Sample.MultiTenancy;
using Sample.Web.Menus;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using System;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Sample.Enum;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;
using Quartz;
using Sample.Web.Quartz;

namespace Sample.Web;

[DependsOn(
    typeof(SampleHttpApiModule),
    typeof(SampleApplicationModule),
    typeof(SampleEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpBackgroundWorkersQuartzModule),
    typeof(AbpBackgroundJobsQuartzModule)
    )]
public class SampleWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(SampleResource),
                typeof(SampleDomainModule).Assembly,
                typeof(SampleDomainSharedModule).Assembly,
                typeof(SampleApplicationModule).Assembly,
                typeof(SampleApplicationContractsModule).Assembly,
                typeof(SampleWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Sample");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        // 設定Cors政策
        context.Services.AddCors(cors => cors.AddPolicy("default", policy =>
        {
            if (hostingEnvironment.IsDevelopment() || hostingEnvironment.EnvironmentName.Equals("HamaDevelpment"))
            {
                policy.
                   AllowAnyOrigin().
                   AllowAnyHeader().
                   AllowAnyMethod();
            }
            else
            {
                var setUrl = configuration.GetSection("App").GetValue<string>("SelfUrl") ?? string.Empty;
                policy.
                   WithOrigins(setUrl,
                     "http://localhost:4200", "https://localhost:44391", (configuration["App:RedirectUrl"] ?? "https://sample.hamastar.com.tw/")).
                   AllowAnyHeader().
                   AllowAnyMethod();
            }
        }));
        // 驗證：防偽驗證
        ConfigureAntiForgery();
        // 驗證：JWT驗證機制
        ConfigureAuthentication(context);
        // 設定Domain
        ConfigureUrls(configuration);
        // Quartz
        ConfigureQuartzOptions(context);
        // 重設身分驗證機制-降低密碼安全度
        ConfigureIdentityOptions();
        // 設定 Global樣式
        ConfigureBundles();
        // AutoMapper 自動對照(映射)
        ConfigureAutoMapper();
        // 設定虛擬目錄
        ConfigureVirtualFileSystem(hostingEnvironment);
        // 多語系設計
        ConfigureLocalizationServices();

        // DNS對應
        ConfigureNavigationServices();
        // API設定
        ConfigureAutoApiControllers();
        // Swagger 設定
        ConfigureSwaggerServices(context.Services, configuration);

        // 自訂錯誤處理(指定key值字串 對應 哪種狀態碼)
        ConfigureExceptionHandler(context);

        // 全域設定 上傳檔案 大小總限制
        ConfigureUploadFileSizeLimit(context, configuration);

        // 設定Session Config
        ConfigureSession(context);
    }

    #region 各種服務註冊

    /// <summary>
    /// 驗證：防偽驗證
    /// </summary>
    private void ConfigureAntiForgery()
    {
        /*context.Services.AddAntiforgery(options =>
        {
        });*/
        Configure<AbpAntiForgeryOptions>(options =>
        {
            //options.TokenCookie.Expiration = TimeSpan.FromDays(365);
            options.AutoValidate = false;
            /*options.AutoValidateIgnoredHttpMethods.Add("GET");
            options.AutoValidateIgnoredHttpMethods.Add("POST");
            options.TokenCookie.Expiration = TimeSpan.FromDays(365);
            // 防偽請求名稱
            options.AuthCookieSchemaName = "Requestverificationtoken";
            // 防偽請求名稱Cookie名稱
            options.TokenCookie.Name = "XSRF-TOKEN";
            // Cookie的安全策略
            options.TokenCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.TokenCookie.HttpOnly = false;*/
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// 重設身分驗證機制-降低密碼安全度
    /// </summary>
    private void ConfigureIdentityOptions()
    {
        Configure<IdentityOptions>(options =>
        {
            // 配置密碼策略
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 4;
        });
    }

    /// <summary>
    /// 設定Domain
    /// </summary>
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    /// <summary>
    /// 設定排程器
    /// </summary>
    /// <param name="context"></param>
    public void ConfigureQuartzOptions(ServiceConfigurationContext context)
    {

        context.Services.AddTransient<IJob, QuartzSample>();

        Configure<AbpBackgroundJobQuartzOptions>(options =>
        {
            options.RetryCount = 1;
            options.RetryIntervalMillisecond = 1000;
        });
    }

    /// <summary>
    /// 設定 Global樣式
    /// </summary>
    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    /// <summary>
    /// AutoMapper 自動對照(映射)
    /// </summary>
    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SampleWebModule>();
        });
    }

    /// <summary>
    /// 設定虛擬目錄：自訂虛擬目錄(記得要自己新增資料夾)
    /// </summary>
    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<SampleDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Sample.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<SampleDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Sample.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<SampleApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Sample.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<SampleApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Sample.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<SampleWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    /// <summary>
    /// 多語系設計
    /// </summary>
    private void ConfigureLocalizationServices()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italian", "it"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });
    }

    /// <summary>
    /// DNS對應
    /// </summary>
    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new SampleMenuContributor());
        });
    }

    /// <summary>
    /// API設定
    /// </summary>
    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(SampleApplicationModule).Assembly);
        });
    }

    /// <summary>
    /// Swagger設定
    /// </summary>
    private void ConfigureSwaggerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAbpSwaggerGenWithOAuth(
            // IConfigurations屬性
            configuration.GetSection("AuthServer").GetValue<String>("Authority") ?? throw new Exception("查無swagger設定檔"),
            new Dictionary<string, string>
            {
                {"Sample", "Sample Api"}
            },
            // 一般設定都在這邊
            options =>
            {
                // 新增summary說明
                //1.locate the xml file being generated
                //if you choose the default file path in the first step,
                //the file name is SolutionName.xml
                //the file path is the project path
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "TaipeiFishTradingSystem API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.AddSecurityDefinition(
                OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme()
                {

                    Type = SecuritySchemeType.OAuth2,
                    Description = "Standard authorisation using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                    OpenIdConnectUrl = new System.Uri(configuration["AuthServer:Authority"] + "/.well-known/openid-configuration"),
                    BearerFormat = "JWT",
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new System.Uri(configuration["AuthServer:Authority"] + "/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "TaipeiFishTradingSystem", "TaipeiFishTradingSystem Api" }
                            },
                            TokenUrl = new System.Uri(configuration["AuthServer:Authority"] + "/connect/token")
                        }
                    }
                });
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                    { Type = ReferenceType.SecurityScheme, Id = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme },
                                 Scheme = "oauth2",
                                 Name = "Bearer",
                                 In = ParameterLocation.Header,
                            },
                             new string[] {}
                        }
                    }
                );
            }
        );
    }

    /// <summary>
    /// 自訂錯誤處理(指定key值字串 對應 哪種狀態碼)
    /// </summary>
    /// <param name="context"> 服務參數設定實體 </param>
    private void ConfigureExceptionHandler(ServiceConfigurationContext context)
    {
        context.Services.Configure<Volo.Abp.AspNetCore.ExceptionHandling.AbpExceptionHttpStatusCodeOptions>(options =>
        {
            options.Map(AppErrorCodeList.NotModified.ToString(), HttpStatusCode.NotModified);
            options.Map(AppErrorCodeList.InternalServerError.ToString(), HttpStatusCode.InternalServerError);
        });
    }

    /// <summary>
    /// 全域設定 上傳檔案 大小總限制
    /// </summary>
    /// <param name="context"> 服務參數設定實體 </param>
    /// <param name="configuration"> 服務參數設定 </param>
    private void ConfigureUploadFileSizeLimit(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.Configure<FormOptions>(x =>
        {
            // 全域設定 上傳檔案 大小總限制
            x.MultipartBodyLengthLimit = configuration.GetSection("SystemParams").GetValue<long>("MultipartBodyLengthLimit");
            // 避免IIS預設設定
            x.ValueLengthLimit = int.MaxValue;
        });
    }

    /// <summary>
    /// 設定Session Config
    /// </summary>
    /// <param name="context"> 服務參數設定實體 </param>
    private void ConfigureSession(ServiceConfigurationContext context)
    {
        context.Services.AddDistributedMemoryCache();

        context.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(300);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    #endregion 各種服務註冊

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
