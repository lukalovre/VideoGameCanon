using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using IGDB;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<InputGameData> Games { get; }

    private const string GameCoverPath = @"../Output/GameCovers";
    private static IGDBClient m_api;

    public MainWindowViewModel()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        using (var reader = new StreamReader(@"../Input/VGC.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var games = csv.GetRecords<InputGameData>();
            Games = new ObservableCollection<InputGameData>(games);
        }
    }

    public async void OnClickCommand()
    {
        var outputGameDataList = new List<OutputGameData>();

        foreach (var game in Games)
        {
            var igdbData = await GetDataFromAPIAsync(game.Url);

            string developerName = string.Empty;
            long developerID = 0;

            if (
                igdbData != null
                && igdbData.InvolvedCompanies != null
                && igdbData.InvolvedCompanies.Values != null
                && igdbData.InvolvedCompanies.Values.FirstOrDefault(o => o.Developer.Value) != null
            )
            {
                var developerData = igdbData.InvolvedCompanies.Values
                    .FirstOrDefault(o => o.Developer.Value)
                    .Company.Value;

                developerName = developerData.Name;
                developerID = developerData.Id.Value;
            }

            try
            {
                var outputGameData = new OutputGameData
                {
                    ID = game.ID,
                    Title = game.Title,
                    Url = game.Url,
                    Summary = igdbData.Summary,
                    Year = igdbData.FirstReleaseDate.HasValue
                        ? igdbData.FirstReleaseDate.Value.Year
                        : 0,
                    DeveloperName = developerName,
                    GameID = igdbData.Id.Value,
                    DeveloperID = developerID
                };

                outputGameDataList.Add(outputGameData);
            }
            catch (Exception ex) { }
        }

        SaveToCSV(outputGameDataList);
    }

    public void SaveToCSV(List<OutputGameData> games)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            Encoding = Encoding.UTF8
        };

        using (var writer = new StreamWriter(@"../Output/VGC.csv"))
        using (var csvWriter = new CsvWriter(writer, csvConfig))
        {
            csvWriter.WriteHeader<OutputGameData>();
            csvWriter.WriteRecords(games);
        }
    }

    public static async Task<IGDB.Models.Game> GetDataFromAPIAsync(string igdbUrl)
    {
        m_api = GetApiClient();

        var games = await m_api.QueryAsync<IGDB.Models.Game>(
            IGDBClient.Endpoints.Games,
            $"fields name, url, summary, first_release_date, id, involved_companies.company.name, involved_companies.developer, cover.image_id, genres, platforms; where url = \"{igdbUrl.Trim()}\";"
        );
        var game = games.FirstOrDefault();

        if (game != null && game.Cover.Value != null)
        {
            var imageId = game.Cover.Value.ImageId;
            var coverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";
            var destinationFile = Path.Combine(GameCoverPath, $"{game.Id.Value}");

            DownloadPNG(coverUrl, destinationFile);
        }
        else
        {
            ///
        }

        return game;
    }

    private static IGDBClient GetApiClient()
    {
        var lines = File.ReadAllLines(@"Keys/igdb_keys.txt");

        var clientId = lines[0];
        var clientSecret = lines[1];

        return new IGDBClient(clientId, clientSecret);
    }

    internal static void DownloadPNG(string webFile, string destinationFile)
    {
        destinationFile = $"{destinationFile}.png";

        if (File.Exists(destinationFile))
        {
            return;
        }

        if (webFile == null || webFile == "N/A")
        {
            return;
        }

        var tempPath = Path.GetTempFileName();

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(new Uri(webFile), destinationFile);
        }
    }
}
