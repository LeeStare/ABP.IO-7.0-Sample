//-----------------------------------------------------------------------
// <copyright file="AbpUsersRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;

using Sample.EntityFrameworkCore;

using Serilog;

using System;

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

    }
}
