//-----------------------------------------------------------------------
// <copyright file="SampleDbContextBase.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author>Samuel</author>
//-----------------------------------------------------------------------

using Sample.EntityFrameworkCore;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Sample.Repositories;

/// <summary>
/// 泛型化資料庫
/// </summary>
public class TFTSDbContextBase<TEntity, TPrimaryKey> : EfCoreRepository<SampleDbContext, TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
{
    protected TFTSDbContextBase(IDbContextProvider<SampleDbContext> dbContextProvider)
            : base(dbContextProvider)
    {
    }
}