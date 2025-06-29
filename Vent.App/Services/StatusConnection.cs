namespace Vent.App.Services;

public static class StatusConnection
{
    public static bool IsInternetAvailable()
    {
        // Verificar si hay conexión a Internet
        var current = Connectivity.NetworkAccess;

        // Verificar el tipo de conexión
        if (current == NetworkAccess.Internet)
        {
            return true; // Hay acceso a internet
        }
        else
        {
            return false; // No hay acceso a internet
        }
    }
}