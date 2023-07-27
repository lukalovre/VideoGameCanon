using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Game> Games { get; }

    public MainWindowViewModel()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        using (var reader = new StreamReader(@"../Input/VGC.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var games = csv.GetRecords<Game>();
            Games = new ObservableCollection<Game>(games);
        }
    }
}
