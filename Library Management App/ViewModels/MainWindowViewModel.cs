using Library_Management_App.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Library_Management_App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly string _filePath = "Data/users.json";

    private LoginViewModel _loginViewModel;
    private HomeViewModel _homeViewModel;
    private RegisterViewModel _registerViewModel;

    private LoginView LoginView { get; }
    private HomeView HomeView { get; }
    private RegisterView RegisterView { get; }

    [ObservableProperty]
    private UserControl _currentView;

    public MainWindowViewModel()
    {
        _homeViewModel = new();
        _loginViewModel = new(_filePath);
        _registerViewModel = new(_filePath);

        LoginView = new() { DataContext = _loginViewModel };
        HomeView = new() { DataContext = _homeViewModel };
        RegisterView = new() { DataContext = _registerViewModel };

        CurrentView = LoginView;

        _loginViewModel.LoginSuccessful += () => CurrentView = HomeView;
        _loginViewModel.RegisterClicked += () => CurrentView = RegisterView;
        _registerViewModel.LoginClicked += () => CurrentView = LoginView;
    }
}
