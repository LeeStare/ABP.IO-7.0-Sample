//-----------------------------------------------------------------------
// <copyright file="IAbpUsersRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using System;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Sample.Repositories.AbpUsers
{
    /// <summary>
    /// 帳號 儲存庫介面
    /// </summary>
    public interface IAbpUsersRepository : IRepository<IdentityUser, Guid>
    {
    }
}
