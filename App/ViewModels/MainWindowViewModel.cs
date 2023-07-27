using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Game> Games { get; }

    public MainWindowViewModel()
    {
        var games = new List<Game>
        {
            new Game
            {
                ID = 1,
                Title = "Hals life",
                Url = "asdas"
            },
            new Game
            {
                ID = 2,
                Title = "Bango bungo",
                Url = "asdas"
            },
            new Game
            {
                ID = 3,
                Title = "tersis",
                Url = "asdas"
            }
        };
        Games = new ObservableCollection<Game>(games);
    }
}
