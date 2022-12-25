using CommandLine;
using Cloudphoto.Commands.Interfaces;
using Cloudphoto.Services;
using Cloudphoto.Models;

namespace Cloudphoto.Commands
{
    [Verb("list", HelpText = "Display albums or photos in bucket.")]
    internal class ListCommand : ICommand
    {
        [Option('a', "album", Required = false, HelpText = "Album which photos will be displayed.")]
        public string Album { get; set; }

        public async Task<int> InvokeAsync()
        {
            using (var data = await SettingsService.GetClientData())
            {
                if (Album is null)
                {
                    await ShowAlbumsAsync(data);
                }
                else
                {
                    await ShowPhotosInAlbumAsync(data);
                }
            }

            return await Task.FromResult(0);
        }

        private async Task ShowPhotosInAlbumAsync(ClientData data)
        {
            var keys = await BucketService.GetAllObjectsInAlbumAsync(data, Album);

            if (!keys.Any())
            {
                throw new Exception($"Album \"{Album}\" doesn't exist.");
            }

            foreach (string key in keys.ToList())
            {
                var metadata = await BucketService.GetObjectMetadataAsync(data, key);

                if (metadata.Headers.ContentType is not "image/jpeg")
                {
                    keys.Remove(key);
                }
            }

            if (!keys.Any())
            {
                throw new Exception($"No photos in album \"{Album}\".");
            }

            foreach (string key in
                keys.Select(k => k.Remove(0, Album.Length + 1)))
            {
                Console.WriteLine(key);
            }
        }

        private static async Task ShowAlbumsAsync(ClientData data)
        {
            var objects = await BucketService.GetAllObjectsAsync(data);

            var albums = objects.S3Objects
                .Where(o => o.Key.Contains('/'))
                .Select(o => o.Key.Split('/')[0])
                .Distinct();

            if (!albums.Any())
            {
                throw new Exception("No albums in bucket.");
            }

            foreach (string album in albums)
            {
                Console.WriteLine(album);
            }
        }
    }
}