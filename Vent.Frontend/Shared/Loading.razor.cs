using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Vent.Shared.Resources;

namespace Vent.Frontend.Shared;

public partial class Loading
{
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
    [Parameter] public string? Label { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (string.IsNullOrEmpty(Label))
        {
            Label = Localizer["PleaseWait"];
        }
    }
}