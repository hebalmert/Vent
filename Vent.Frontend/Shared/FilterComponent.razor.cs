using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Vent.Shared.Resources;

namespace Vent.Frontend.Shared;

public partial class FilterComponent
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = null!;

    [Parameter] public string FilterValue { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ApplyFilter { get; set; }

    private async Task ClearFilter()
    {
        FilterValue = string.Empty;
        await ApplyFilter.InvokeAsync(FilterValue);
    }

    private async Task OnfilterApply()
    {
        await ApplyFilter.InvokeAsync(FilterValue);
    }
}