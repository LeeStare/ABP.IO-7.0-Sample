//-----------------------------------------------------------------------
// <copyright file="AbpUsersAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using Sample.Dto.AbpUsers;
using Sample.Enum;
using Sample.IApplication.AbpUsers;
using Sample.Permissions;
using Sample.Repositories.AbpUsers;

using Serilog;

using System;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Validation;

namespace Sample.AbpUsers
{

    /// <summary>
    /// 使用者(帳號) App
    /// </summary>
    public class AbpUsersAppService : ApplicationService, IAbpUsersAppService
    {
        #region Fields

        /// <summary>
        /// SettingProvider
        /// </summary>
        private readonly IConfiguration appConfiguration;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger log = Log.ForContext<AbpUsersAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser currentUser;

        /// <summary>
        /// IHttpContextAccessor
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// ISession
        /// </summary>
        private ISession _session => httpContextAccessor.HttpContext.Session;

        /// <summary>
        /// 使用者 儲存庫
        /// </summary>
        private readonly IAbpUsersRepository abpUsersRepository;

        /// <summary>
        /// 內建 使用者Service層管理員
        /// </summary>
        private readonly IdentityUserManager userManager;

        /// <summary>
        /// 內建 使用者 儲存庫
        /// </summary>
        private readonly IIdentityUserRepository identityUserRepository;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpUsersAppService" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="httpContextAccessor"> IHttpContextAccessor </param>
        /// <param name="abpUsersRepository"> 使用者 儲存庫 </param>
        /// <param name="userManager"> 內建 使用者Service層管理員 </param>
        /// <param name="identityUserRepository"> 內建 使用者 儲存庫 </param>
        /// <param name="abpRolesRepository"> 角色 儲存庫 </param>
        public AbpUsersAppService(
            IConfiguration appConfiguration,
            ICurrentUser currentUser,
            IHttpContextAccessor httpContextAccessor,
            IAbpUsersRepository abpUsersRepository,
            IdentityUserManager userManager,
            IIdentityUserRepository identityUserRepository
            )
        {
            this.appConfiguration = appConfiguration;
            this.currentUser = currentUser;
            this.httpContextAccessor = httpContextAccessor;

            this.userManager = userManager;
            this.abpUsersRepository = abpUsersRepository;

            this.identityUserRepository = identityUserRepository;
        }

        #endregion Constructor

        #region 查詢

        /// <summary>
        /// 查詢：使用者列表
        /// </summary>
        /// <param name="request"> 搜尋條件 </param>
        /// <returns> 取得結果 </returns>
        /// <exception cref="AbpValidationException"> Request失敗 </exception>
        /// <exception cref="EntityNotFoundException"> 查無Entity </exception>
        /// <exception cref="UserFriendlyException"> 其他錯誤 </exception>
        //[Authorize(SamplePermissions.SystemManagement.Default)]
        public async Task<SearchUsersResponse> SearchUsersList(SearchUsersRequest request)
        {
            // === 回傳 ===
            SearchUsersResponse response = new SearchUsersResponse();

            try
            {
                // 分頁結果 (給預設值)
                request.Page = request.Page > 0 ? request.Page : 1;
                request.RowPerPage = request.RowPerPage > 0 ? request.RowPerPage : 10;

                // === 查詢 ===
                response = await this.abpUsersRepository.SearchUsersAsync(request);
            }
            catch (AbpValidationException ex)
            {
                this.log.Error(ex.StackTrace ?? "Request錯誤");
                throw new AbpValidationException(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                this.log.Error(ex.StackTrace ?? "查無Entity");
                throw new EntityNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace ?? "其他錯誤");
                throw new UserFriendlyException(ex.Message, code: AppErrorCodeList.InternalServerError.ToString());
            }

            return response;
        }

        #endregion 查詢
    }
}
