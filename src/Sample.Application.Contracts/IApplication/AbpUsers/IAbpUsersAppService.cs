//-----------------------------------------------------------------------
// <copyright file="IAbpUsersAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Sample.Dto.AbpUsers;

using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace Sample.IApplication.AbpUsers
{
    /// <summary>
    /// 使用者(帳號) App介面
    /// </summary>
    public interface IAbpUsersAppService : IApplicationService
    {
        #region 查詢

        /// <summary>
        /// 查詢：使用者列表 (帳號、組織、角色)
        /// </summary>
        /// <param name="request"> 搜尋條件 </param>
        /// <returns> 取得結果 </returns>
        Task<SearchUsersResponse> SearchUsersList(SearchUsersRequest request);

        #endregion 查詢
    }
}
