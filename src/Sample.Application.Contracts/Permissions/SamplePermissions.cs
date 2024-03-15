namespace Sample.Permissions;

/// <summary>
/// 權限：定義欄位值、層級列表
/// </summary>
public static class SamplePermissions
{
    #region Fields 定義權限Key值
    public const string GroupName = "Sample";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    // 【內容管理功能】
    // ========= 系統管理 =========
    /// <summary>
    /// 系統管理 權限
    /// </summary>
    public static class SystemManagement
    {
        public const string Default = GroupName + ".SystemManagement";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    #endregion Fields 定義權限Key值
}
