using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using IGDB;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Game> Games { get; }

    private const string GameCoverPath = @"../Output/GameCovers";

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

    private void asd()
    {
        var igdbData = await Igdb.GetDataFromAPIAsync(url);
    }

    public static async Task<IGDB.Models.Game> GetDataFromAPIAsync(
        string igdbUrl,
        bool downloadPoster = true
    )
    {
        var api = GetApiClient();

        var games = await api.QueryAsync<IGDB.Models.Game>(
            IGDBClient.Endpoints.Games,
            $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where url = \"{igdbUrl.Trim()}\";"
        );
        var game = games.FirstOrDefault();

        if (downloadPoster)
        {
            var imageId = game.Cover.Value.ImageId;
            var coverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";
            var destinationFile = Path.Combine(GameCoverPath, $"{game.Id.Value}");

            DownloadPNG(coverUrl, destinationFile);
        }

        return game;
    }

    private static IGDBClient GetApiClient()
    {
        var lines = File.ReadAllLines(@"Keys\igdb_keys.txt");

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
            client.DownloadFile(new Uri(webFile), tempPath);
        }

        using (var bmpTemp = new Bitmap(tempPath))
        {
            var imagesTemp = new Bitmap(bmpTemp);
            imagesTemp.Save(destinationFile, ImageFormat.Png);
        }

        File.Delete(tempPath);
    }
}
