//-----------------------------------------------------------------------
// <copyright file="SearchUsersRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------
using Newtonsoft.Json;

namespace Sample.Dto.AbpUsers
{
    /// <summary>
    /// 查詢：使用者列表 Request ViewModel
    /// </summary>
    public class SearchUsersRequest : General.GeneralPage
    {
        #region 搜尋條件

        /// <summary>
        /// 姓名 = AbpUsers.Name (模糊搜尋)
        /// </summary>
        [JsonProperty("Name")]
        public string? name { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("IsActive")]
        public bool? IsActive { get; set; }

        #endregion 搜尋條件
    }
}
