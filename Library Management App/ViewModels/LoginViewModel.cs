using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;


namespace Library_Management_App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    public event Action<string>? LoginSuccessful;
    public event Action? RegisterClicked;
    private IFIleBackend<IUser> _fileBackend;


    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [RelayCommand]
    private void Login()
    {
        if (!RegisterViewModel.CheckExistingUser(_fileBackend, UserName) && TryGetUserPermission(out string permission))
            LoginSuccessful?.Invoke(permission);
    }
    [RelayCommand]
    private void NavigateToRegister()
    {
        RegisterClicked?.Invoke();
    }

    public LoginViewModel(string filePath)
    {
        _fileBackend = new FileBackend(filePath);
    }

    private bool TryGetUserPermission(out string permission)
    {
        permission = "user";
        List<IUser> users = _fileBackend.Load();

        foreach(IUser user in users)
        {
            var userInfo = user.GetUserInformation();

            if(userInfo["UserName"] == UserName && userInfo["Password"] == Password)
            {
                if (userInfo.TryGetValue("Permission", out string? role) && !string.IsNullOrWhiteSpace(role))
                    permission = role;

                return true;
            }
        }
        
        return false;
    }
}
