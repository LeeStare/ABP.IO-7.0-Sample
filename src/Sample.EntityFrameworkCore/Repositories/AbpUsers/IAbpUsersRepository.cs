﻿//-----------------------------------------------------------------------
// <copyright file="IAbpUsersRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Sample.Dto.AbpUsers;

using System;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Sample.Repositories.AbpUsers
{
    /// <summary>
    /// 帳號 儲存庫介面
    /// </summary>
    public interface IAbpUsersRepository : IRepository<IdentityUser, Guid>
    {
        #region 查詢

        /// <summary>
        /// 查詢：使用者列表
        /// </summary>
        /// <param name="request"> 搜尋條件 </param>
        /// <returns> 取得結果 </returns>
        Task<SearchUsersResponse> SearchUsersAsync(SearchUsersRequest request);

        #endregion 查詢
    }
}
