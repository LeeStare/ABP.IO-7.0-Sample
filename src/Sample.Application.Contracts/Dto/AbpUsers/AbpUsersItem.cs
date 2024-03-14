//-----------------------------------------------------------------------
// <copyright file="AbpUsersItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

using System;
using System.ComponentModel.DataAnnotations;

namespace Sample.Dto.AbpUsers
{
    /// <summary>
    /// 查詢：使用者列表 (帳號、角色) Response ViewModel - 2
    /// 參考表：AbpUsers 使用者
    /// </summary>
    public class AbpUsersItem
    {
        #region 使用者(帳號) AbpUsers

        /// <summary>
        /// GUID
        /// </summary>
        [Required]
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        [JsonProperty("UserName")]
        public string UserName { get; set; } = "DataLoss";

        /// <summary>
        /// 姓名
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; } = "DataLoss";

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        #endregion 使用者(帳號) AbpUsers

        #region 角色 AbpRoles

        /// <summary>
        /// 角色名稱
        /// </summary>
        [JsonProperty("roleName")]
        public string RoleName { get; set; } = "DataLoss";

        #endregion 角色 AbpRoles

    }
}
