namespace Vent.App.Services;

public static class TokenStorage
{
    private const string TokenKey = "auth_token";

    public static async Task SetTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public static async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync(TokenKey);
    }

    public static void RemoveToken()
    {
        SecureStorage.Remove(TokenKey);
    }
}