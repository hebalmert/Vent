using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.SoftPlanView;

public partial class EditSoftPlan
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private string BaseUrl = "api/softplans";

    private SoftPlan? SoftPlan;

    private FormSoftPlan? FormSoftPlan { get; set; }

    [Parameter]
    public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<SoftPlan>($"{BaseUrl}/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/softplans");
            return;
        }
        SoftPlan = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync($"{BaseUrl}", SoftPlan);

        if (responseHTTP.Error)
        {
            var mensajeError = await responseHTTP.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
        }
        else
        {
            FormSoftPlan!.FormPostedSuccessfully = true;
            _navigationManager.NavigateTo("/softplans");
        }
    }

    private void Return()
    {
        FormSoftPlan!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/softplans");
    }
}