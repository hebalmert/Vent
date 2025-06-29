namespace Vent.App.Services;

public static class SessionManager
{
    private const string IsLoggedInKey = "IsLoggedIn";

    public static async Task<bool> IsLoggedIn()
    {
        var value = await SecureStorage.GetAsync(IsLoggedInKey);
        return value == "true";
    }

    public static async Task SetLoggedIn(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            await SecureStorage.SetAsync(IsLoggedInKey, "true");
        }
        else
        {
            SecureStorage.Remove(IsLoggedInKey);
        }
    }
}