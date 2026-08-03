namespace Tests;

/// <summary>
///     Shared NUnit category names for filtering CI vs hardware lab suites.
/// </summary>
internal static class TestCategories
{
    public const string Unit = "Unit";
    public const string CI = "CI";
    public const string Hardware = "Hardware";
    public const string Interactive = "Interactive";
    public const string Destructive = "Destructive";
    public const string Admin = "Admin";
}
