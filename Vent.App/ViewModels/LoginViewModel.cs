using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using Vent.App.Services;
using Vent.Shared.ResponsesSec;

namespace Vent.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private LoginDTO? _loginDTO = new();
    private string? errorMessage;
    private bool isBusy;

    public LoginDTO? LoginDTO
    {
        get => _loginDTO;
        set => SetProperty(ref _loginDTO, value);
    }

    public string? ErrorMessage
    {
        get => errorMessage;
        set => SetProperty(ref errorMessage, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    public LoginViewModel(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        // Verificar conexión a Internet
        if (!StatusConnection.IsInternetAvailable())
        {
            ErrorMessage = "No hay conexión a Internet.";
            return;
        }
        // Verificar que LoginDTO no sea nulo
        if (LoginDTO == null)
        {
            ErrorMessage = "Por favor, complete los campos de inicio de sesión.";
            return;
        }
        //Validamos que sea un formato de correo
        if (!EsCorreoValido(LoginDTO.Email))
        {
            ErrorMessage = "El correo electrónico no tiene un formato válido.";
            return;
        }
        //validamos que la clave tenga un minimo de 6 digitos
        if (string.IsNullOrWhiteSpace(LoginDTO.Password) || LoginDTO.Password.Length < 6)
        {
            ErrorMessage = "La clave debe tener al menos 6 caracteres.";
            return;
        }
        // Activamos el spinner
        IsBusy = true;

        bool isSuccess = await _loginService.LoginAsync(LoginDTO);
        if (!isSuccess)
        {
            ErrorMessage = "Error en el inicio de sesión.";
            await _loginService.LogoutAsync();
        }
        IsBusy = false;
        return;
    }

    private bool EsCorreoValido(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return false;
        return Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}