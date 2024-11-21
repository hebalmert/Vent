using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Vent.Shared.Resources;

namespace Vent.Frontend.Shared;

public partial class ConfirmDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;

    [Parameter] public string Message { get; set; } = null!;

    private void Accept()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}