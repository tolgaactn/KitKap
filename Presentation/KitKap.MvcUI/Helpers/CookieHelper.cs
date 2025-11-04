public static class CookieHelper
{
    private const string GUEST_ID_KEY = "GuestId";
    private const int COOKIE_EXPIRATION_DAYS = 30;

    // ✅ Var olan metod - Create eder
    public static string GetOrCreateGuestId(HttpContext context)
    {
        var guestId = context.Request.Cookies[GUEST_ID_KEY];

        if (string.IsNullOrEmpty(guestId))
        {
            guestId = Guid.NewGuid().ToString();

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(COOKIE_EXPIRATION_DAYS),
                HttpOnly = true,
                Secure = true, // HTTPS için
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Append(GUEST_ID_KEY, guestId, options);
        }

        return guestId;
    }

    // ✅ YENİ - Sadece okur, create etmez
    public static string? GetGuestId(HttpContext context)
    {
        return context.Request.Cookies[GUEST_ID_KEY];
    }

    // ✅ YENİ - Cookie'yi siler
    public static void RemoveGuestId(HttpContext context)
    {
        context.Response.Cookies.Delete(GUEST_ID_KEY);
    }
}