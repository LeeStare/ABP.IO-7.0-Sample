//-----------------------------------------------------------------------
// <copyright file="SearchUsersResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Dto.AbpUsers
{
    /// <summary>
    /// 查詢：使用者列表 Response ViewModel - 1
    /// </summary>
    public class SearchUsersResponse
    {
        /// <summary>
        /// 查詢結果
        /// </summary>
        [JsonProperty("Items")]
        public List<AbpUsersItem>? Items { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        [JsonProperty("TotalCount")]
        public int TotalCount { get; set; }
    }
}
