using Cloudphoto.Commands.Interfaces;
using CommandLine;
using System.Text;
using Cloudphoto.Services;
using Cloudphoto.Models;
using Amazon.S3.Model;

namespace Cloudphoto.Commands
{
    [Verb("mksite", HelpText = "Generates and publishes web pages for bucket.")]
    internal class MksiteCommand : ICommand
    {
        public async Task<int> InvokeAsync()
        {
            using (var data = await SettingsService.GetClientData())
            {
                await data.Client.DeleteBucketWebsiteAsync(data.Settings.BucketName);

                var albums = await GetAlbumsDictionary(data);

                await UploadAlbumsPagesAsync(data, albums);

                await UploadIndexPageAsync(data, albums);

                await UploadErrorPageAsync(data);

                var response = await data.Client.PutBucketWebsiteAsync(
                    data.Settings.BucketName,
                    new WebsiteConfiguration()
                    {
                        IndexDocumentSuffix = "index.html",
                        ErrorDocument = "error.html"
                    });
                
                Console.WriteLine(
                    $"https://{data.Settings.BucketName}.website.yandexcloud.net");
            }

            return await Task.FromResult(0);
        }

        private static async Task UploadErrorPageAsync(
            ClientData data)
        {
            var errorPath =
                $"error.html";

            await File.WriteAllTextAsync(
                errorPath,
                PagesService.GetErrorPage());

            await BucketService.UploadPageAsync(data, errorPath);

            File.Delete(errorPath);
        }

        private static async Task UploadIndexPageAsync(
            ClientData data, 
            Dictionary<string, List<(string, string)>> albums)
        {
            var stringBuilder = new StringBuilder();
            var index = 1;
            foreach (KeyValuePair<string, List<(string, string)>> album in
                albums)
            {
                var albumUrl = $"album{index}.html";

                stringBuilder.Append($"<li><a href=\"{albumUrl}\">{album.Key}</a></li>");

                index++;
            }

            var indexPath =
                $"index.html";

            await File.WriteAllTextAsync(
                indexPath,
                PagesService.GetIndexPage(
                    stringBuilder.ToString()));

            await BucketService.UploadPageAsync(data, indexPath);

            File.Delete(indexPath);
        }

        private static async Task UploadAlbumsPagesAsync(
            ClientData data, 
            Dictionary<string, List<(string, string)>> albums)
        {
            var index = 1;
            foreach (KeyValuePair<string, List<(string, string)>> album in
                albums)
            {
                var stringBuilder = new StringBuilder();

                foreach ((string, string) item in album.Value)
                {
                    stringBuilder.Append(
                        $@"<img src=""{item.Item2}"" data-title=""{item.Item1}"">");
                }

                var albumPath =
                    $"album{index}.html";

                await File.WriteAllTextAsync(
                    albumPath,
                    PagesService.GetAlbumPage(stringBuilder.ToString()));

                await BucketService.UploadPageAsync(data, albumPath);

                File.Delete(albumPath);

                index++;
            }
        }

        private static async Task<Dictionary<string, List<(string, string)>>> GetAlbumsDictionary(
            ClientData data)
        {
            var albums = new Dictionary<string, List<(string, string)>>();

            var objects = await BucketService.GetAllObjectsAsync(data);

            foreach (S3Object @object in objects.S3Objects)
            {
                var metadata = await BucketService.GetObjectMetadataAsync(
                    data,
                    @object.Key);

                if (metadata.Headers.ContentType is "image/jpeg")
                {
                    var key = @object.Key.Split('/');

                    var photoUrl =
                        $"{data.Settings.EndpointUrl}/{data.Settings.BucketName}/{@object.Key}";
                    if (albums.ContainsKey(key[0]))
                    {
                        albums[key[0]].Add((key[1], photoUrl));
                    }
                    else
                    {
                        albums[key[0]] = new List<(string, string)>()
                        {
                            (key[1], photoUrl)
                        };
                    }
                }
            }

            return albums;
        }
    }
}