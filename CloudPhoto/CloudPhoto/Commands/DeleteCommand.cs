using Cloudphoto.Commands.Interfaces;
using CommandLine;
using Amazon.S3;
using Cloudphoto.Services;
using Cloudphoto.Models;

namespace Cloudphoto.Commands
{
    [Verb("delete", HelpText = "Delete album or photo from bucket.")]
    internal class DeleteCommand : ICommand
    {
        [Option('a', "album", Required = true, HelpText = "Album which will be deleted. If photo is specified, only that photo will be deleted from album.")]
        public string Album { get; set; }

        [Option('p', "photo", Required = false, HelpText = "Specify photo which will be deleted in selected album.")]
        public string Photo { get; set; }

        public async Task<int> InvokeAsync()
        {
            using (var data = await SettingsService.GetClientData())
            {
                var keys = await BucketService.GetAllObjectsInAlbumAsync(
                    data,
                    Album);

                if (!keys.Any())
                {
                    throw new Exception($"Album \"{Album}\" doesn't exist.");
                }

                if (Photo is null)
                {
                    await DeletePhotosInAlbumAsync(data, keys);
                }
                else
                {
                    await DeletePhotoAsync(data, keys);
                }
            }

            return await Task.FromResult(0);
        }

        private async Task DeletePhotoAsync(ClientData data, IList<string> keys)
        {
            if (keys.FirstOrDefault(k => k.Equals($"{Album}/{Photo}")) is string key)
            {
                await BucketService.DeleteObjectAsync(data, key);
            }
            else
            {
                throw new Exception($"Photo \"{Photo}\" doesn't exist.");
            }
        }

        private static async Task DeletePhotosInAlbumAsync(ClientData data, IList<string> keys)
        {
            foreach (string key in keys)
            {
                try
                {
                    await BucketService.DeleteObjectAsync(data, key);
                }
                catch (AmazonS3Exception)
                {
                    Console.Error.WriteLine(
                        "Error encountered on server. " +
                        $"Message: Failed to delete {key} object.");
                }
            }
        }
    }
}