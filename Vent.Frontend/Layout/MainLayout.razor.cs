using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Vent.Shared.Resources;

namespace Vent.Frontend.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = true;

    private string _icon = Icons.Material.Filled.DarkMode;
    private bool _darkMode { get; set; } = true;

    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void DarkModeToggle()
    {
        _darkMode = !_darkMode;
        _icon = _darkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode;
    }
}