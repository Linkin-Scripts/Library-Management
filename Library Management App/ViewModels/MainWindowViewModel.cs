using Library_Management_App.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Library_Management_App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private LoginViewModel _loginViewModel = new("Data/users.json");
    private HomeViewModel _homeViewModel = new();

    private LoginView LoginView { get; }
    private HomeView HomeView { get; }

    [ObservableProperty]
    private UserControl _currentView;

    public MainWindowViewModel()
    {
        _loginViewModel.LoginSuccessful += () => CurrentView = HomeView;

        LoginView = new LoginView { DataContext = _loginViewModel };
        HomeView = new HomeView { DataContext = _homeViewModel };

        CurrentView = LoginView;
    }
}
