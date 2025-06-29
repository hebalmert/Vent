using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Shared.Entities;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesViews.ManagerView;

public partial class FormManager
{
    private EditContext _editContext = null!;
    private Corporation? SelectedCorporation;
    private List<Corporation>? Corporations;
    private string? ImageUrl;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public Manager Manager { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Manager);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCorporation();
    }

    private async Task LoadCorporation()
    {
        var responseHTTP = await _repository.GetAsync<List<Corporation>>($"api/corporations/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/planillas");
            return;
        }

        Corporations = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedCorporation = Corporations!.Where(x => x.CorporationId == Manager.CorporationId)
                .Select(x => new Corporation { CorporationId = x.CorporationId, Name = x.Name }).FirstOrDefault();
            ImageUrl = Manager.ImageFullPath;
        }
    }

    private void ImageSelected(string imagenBase64)
    {
        Manager.ImgBase64 = imagenBase64;
        ImageUrl = null;
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = _editContext.IsModified();

        if (!formWasEdited)
        {
            return;
        }

        if (FormPostedSuccessfully)
        {
            return;
        }

        var result = await _sweetAlert.FireAsync(new SweetAlertOptions
        {
            Title = "Confirmación",
            Text = "¿Deseas abandonar la página y perder los cambios?",
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

    private void CorporationChanged(Corporation modelo)
    {
        Manager.CorporationId = modelo.CorporationId;
        SelectedCorporation = modelo;
    }

    private string GetDisplayName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;
            if (property != null)
            {
                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    return displayAttribute.Name!;
                }
            }
        }
        return "Texto no definido";
    }
}