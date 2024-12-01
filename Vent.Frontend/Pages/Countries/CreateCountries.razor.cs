using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;
using Vent.Shared.Resources;

namespace Vent.Frontend.Pages.Countries;

public partial class CreateCountries
{
    private FormCountry? FormCountry { get; set; }

    private Country Country = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.Post("/api/countries", Country);
        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[messageError!], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    private void Return()
    {
        FormCountry!.FormPostSuccessfully = true;
        NavigationManager.NavigateTo("/Countries");
    }
}