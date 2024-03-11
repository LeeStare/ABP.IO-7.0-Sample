//-----------------------------------------------------------------------
// <copyright file="AppErrorCodeList.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>Leestar</author>
//-----------------------------------------------------------------------

namespace Sample.Enum;

/// <summary>
/// 錯誤代碼
/// </summary>
public enum AppErrorCodeList
{
    /// <summary>
    /// 304 不可修改
    /// </summary>
    NotModified,

    /// <summary>
    /// 500 客製化取代 UserFriendlyException
    /// </summary>
    InternalServerError
}