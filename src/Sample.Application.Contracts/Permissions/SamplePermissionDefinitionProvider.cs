using Microsoft.Extensions.Localization;

using Sample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Sample.Permissions;

/// <summary>
/// 權限：加入群組
/// </summary>
public class SamplePermissionDefinitionProvider : PermissionDefinitionProvider
{
    #region Fields

    private readonly IStringLocalizer<SampleResource> localizer;

    #endregion Fields

    #region Constructor

    public SamplePermissionDefinitionProvider(IStringLocalizer<SampleResource> localizer)
    {
        this.localizer = localizer;
    }

    #endregion Constructor

    /// <summary>
    /// 定義所有權限
    /// </summary>
    /// <param name="context"> IPermissionDefinitionContext </param>
    public override void Define(IPermissionDefinitionContext context)
    {
        var sampleGroup = context.AddGroup(SamplePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(SamplePermissions.MyPermission1, L("Permission:MyPermission1"));

        // 【內容管理功能】
        // ========= 系統管理 =========
        // 系統管理
        var systemManagementPermission = sampleGroup.AddPermission(SamplePermissions.SystemManagement.Default, L("Permission:SystemManagement"));
        systemManagementPermission.AddChild(SamplePermissions.SystemManagement.Create, L("Permission:SystemManagement.Create"));
        systemManagementPermission.AddChild(SamplePermissions.SystemManagement.Edit, L("Permission:SystemManagement.Edit"));
        systemManagementPermission.AddChild(SamplePermissions.SystemManagement.Delete, L("Permission:SystemManagement.Delete"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SampleResource>(name);
    }
}
