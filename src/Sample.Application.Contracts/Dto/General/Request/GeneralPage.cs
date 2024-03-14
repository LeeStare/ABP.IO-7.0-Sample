//-----------------------------------------------------------------------
// <copyright file="GeneralPage.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;

namespace Sample.Dto.General;

/// <summary>
/// 通用頁數 Request View Model
/// </summary>
public class GeneralPage
{

    #region 分頁

    /// <summary>
    /// 第幾頁
    /// </summary>
    [Required]
    [JsonProperty("page")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    [Required]
    [JsonProperty("rowPerPage")]
    public int RowPerPage { get; set; } = 10;

    #endregion 分頁
}
