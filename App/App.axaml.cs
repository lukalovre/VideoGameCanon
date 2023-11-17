using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using App.ViewModels;
using App.Views;

namespace App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
	}

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel(), };

            // Temple standard
			desktop.MainWindow.Height = 480; 
			desktop.MainWindow.Width = 640; 
		}

		base.OnFrameworkInitializationCompleted();
    }
}
