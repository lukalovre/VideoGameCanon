using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Person> People { get; }

    public MainWindowViewModel()
    {
        var people = new List<Person>
        {
            new Person("Neil", "Armstrong"),
            new Person("Buzz", "Lightyear"),
            new Person("James", "Kirk")
        };
        People = new ObservableCollection<Person>(people);
    }
}
