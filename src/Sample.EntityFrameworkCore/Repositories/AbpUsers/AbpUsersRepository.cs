//-----------------------------------------------------------------------
// <copyright file="AbpUsersRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;

using Sample.Dto.AbpUsers;
using Sample.EntityFrameworkCore;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Sample.Repositories.AbpUsers
{
    /// <summary>
    /// 帳號 儲存庫
    /// </summary>
    public class AbpUsersRepository : SampleDbContextBase<IdentityUser, Guid>, IAbpUsersRepository
    {

        #region Fields

        /// <summary>
        /// SettingProvider
        /// </summary>
        private readonly IConfiguration appConfiguration;

        /// <summary>
        /// UnitOfWorkManager
        /// </summary>
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger log = Log.ForContext<AbpUsersRepository>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser currentUser;

        /// <summary>
        /// IdentityUserManager
        /// </summary>
        private readonly IdentityUserManager UserManager;

        /// <summary>
        /// 內建 使用者 儲存庫
        /// </summary>
        private readonly IIdentityUserRepository identityUserRepository;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpUsersRepository" /> class.
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="contextProvider"> dbContext </param>
        /// <param name="unitOfWorkManager"> Unit of Work Manager </param>
        /// <param name="currentUser"> 目前登入的使用者 </param>
        /// <param name="UserManager"> 內建 使用者Service層管理員 </param>
        /// <param name="identityUserRepository"> 內建 使用者 儲存庫 </param>
        public AbpUsersRepository(
            IConfiguration appConfiguration,
            IDbContextProvider<SampleDbContext> contextProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUser currentUser,
            IdentityUserManager UserManager,
            IIdentityUserRepository identityUserRepository) : base(contextProvider)
        {
            this.appConfiguration = appConfiguration;
            this.unitOfWorkManager = unitOfWorkManager;
            this.currentUser = currentUser;

            this.UserManager = UserManager;
            this.identityUserRepository = identityUserRepository;
        }

        #endregion Constructor

        #region 查詢

        /// <summary>
        /// 查詢：使用者列表
        /// </summary>
        /// <param name="request"> 搜尋條件 </param>
        /// <returns> 取得結果 </returns>
        public async Task<SearchUsersResponse> SearchUsersAsync(SearchUsersRequest request)
        {
            SearchUsersResponse result = new SearchUsersResponse();
            result.Items = new List<AbpUsersItem>();
            var dbContext = await GetDbContextAsync();
            try
            {
                var table = from u in dbContext.Users
                            where (request.IsActive == null || u.IsActive == request.IsActive)
                            && (!string.IsNullOrEmpty(request.name) ? u.Name.Contains(request.name) : true)
                            join ur in dbContext.UserRole on u.Id equals ur.UserId into LeftJoin
                            from lf in LeftJoin.DefaultIfEmpty()
                            join r in dbContext.Roles on lf.RoleId equals r.Id into LeftJoinLeftJoin
                            from lflf in LeftJoinLeftJoin.DefaultIfEmpty()
                            select new AbpUsersItem
                            {
                                // 使用者(帳號) AbpUsers
                                Id = u.Id,
                                UserName = u.UserName,
                                Name = u.Name,
                                IsActive = u.IsActive,
                                // 角色 AbpRoles
                                RoleName = lflf.Name ?? "未賦予角色"
                            };

                if (table.FirstOrDefault() != null)
                {
                    result.TotalCount = table.Count();

                    result.Items = table.Page(request.Page, request.RowPerPage).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace ?? "其他錯誤");
                throw;
            }

            return result;
        }

        #endregion 查詢
    }
}
