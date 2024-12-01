using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Vent.Shared.Entities;
using Vent.Shared.Resources;
using CurrieTechnologies.Razor.SweetAlert2;

namespace Vent.Frontend.Pages.Countries;

public partial class FormCountry
{
    [Inject] private SweetAlertService Swal { get; set; } = null!;
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;

    private EditContext EditContext = null!;

    [EditorRequired, Parameter] public Country Country { get; set; } = null!;

    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    public bool FormPostSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        EditContext = new(Country);
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = EditContext.IsModified();
        if (!formWasEdited || FormPostSuccessfully)
        {
            return;
        }

        var result = await Swal.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }
}