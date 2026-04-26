using Microsoft.AspNetCore.Http;

namespace Account;
public static class DefaultCookieOptions
{
    public static readonly string RefreshTokenKey = "RefreshToken";
    public static CookieOptions GetDefaultCookieOpitons() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddDays(1)
    };
}
