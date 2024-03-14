//-----------------------------------------------------------------------
// <copyright file="SampleEntity.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>LeeStar</author>
//-----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Volo.Abp.Domain.Entities;

namespace Sample.Entities;

/// <summary>
/// 範例資訊
/// </summary>
[Table("SampleEntity")]
[Comment("範例資訊")]
public class SampleEntity : Entity<Guid>
{

    /// <summary>
    /// 魚種資訊ID
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("範例資訊ID")]
    public override Guid Id { get; protected set; }

    /// <summary>
    /// 範例資訊名稱
    /// </summary>
    [Column("Name", TypeName = "varchar")]
    [Comment("範例資訊名稱")]
    [MaxLength(20)]
    public required string Name { get; set; }
}
