using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Vent.Shared.Resources;

namespace Vent.Frontend.Layout;

public partial class NavMenu
{
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;

    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}