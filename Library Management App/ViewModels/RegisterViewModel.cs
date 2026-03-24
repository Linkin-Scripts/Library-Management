using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;

namespace Library_Management_App.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private IFIleBackend<IUser> _fileBackend;
    public event Action? LoginClicked;

    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _confirmPassword = "";

    [ObservableProperty]
    private string _selectedRole = "User";

    [ObservableProperty]
    private string _librarianPassword = "";

    public IReadOnlyList<string> AvailableRoles { get; } = new List<string> { "User", "Librarian" };

    public bool IsLibrarianSelected => SelectedRole.Equals("Librarian", StringComparison.OrdinalIgnoreCase);

    public RegisterViewModel(string filePath)
    {
        _fileBackend = new FileBackend(filePath);
    }

    [RelayCommand]
    private void Register()
    {
        if (IsConfirmPassword() && UserName != "" && CheckExistingUser(_fileBackend, UserName) && IsLibrarianPasswordValid())
        {
            LoginClicked?.Invoke();
            SaveUser();
        }
    }

    [RelayCommand]
    private void NavigateToLogin()
    {
        LoginClicked?.Invoke();
    }

    private bool SaveUser()
    {
        _fileBackend.Save(new User(){ UserName = UserName, Password = Password, Permission = SelectedRole });

        return true;
    }

    partial void OnSelectedRoleChanged(string value)
    {
        OnPropertyChanged(nameof(IsLibrarianSelected));

        if (!IsLibrarianSelected)
            LibrarianPassword = "";
    }

    static public bool CheckExistingUser(IFIleBackend<IUser> fIleBackend, String username)
    {
        List<IUser> users = fIleBackend.Load();
    
        foreach(IUser user in users)
        {
            var userInfo = user.GetUserInformation();
            if(userInfo["UserName"] == username)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsConfirmPassword()
    {
        if (Password == ConfirmPassword && Password != "")
            return true;
        else
            return false;
    }

    private bool IsLibrarianPasswordValid()
    {
        if (!IsLibrarianSelected)
            return true;

        return LibrarianPassword == "admin";
    }

}
