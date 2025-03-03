namespace SimpliHR.Webui.Modals.Account;

public static class ThemeType
{
    public static string themeO { get; set; }
}
public class Login
{
    public int LoginId { get; set; }
    public int? EmpId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; } = true;
    public string DisplayMessage { get; set; } = "_blank";
}
