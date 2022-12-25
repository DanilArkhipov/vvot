using Cloudphoto.Commands.Interfaces;
using CommandLine;
using Cloudphoto.Services;
using Amazon.S3;

namespace Cloudphoto.Commands
{
    [Verb("download", HelpText = "Download photos from bucket.")]
    internal class DownloadCommand : ICommand
    {
        [Option('a', "album", Required = true, HelpText = "Album from which photos will be downloaded")]
        public string Album { get; set; }

        [Option('p', "path", Required = false, HelpText = "Path where photos will be stored.")]
        public string Path { get; set; }

        public async Task<int> InvokeAsync()
        {
            Path ??= Environment.CurrentDirectory;

            if (!Directory.Exists(Path))
            {
                throw new Exception($"Directory \"{Path}\" doesn't exist.");
            }

            using (var data = await SettingsService.GetClientData())
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

                foreach (string key in keys)
                {
                    try
                    {
                        await BucketService.DownloadObjectAsync(data, Album, Path, key);
                    }
                    catch (AmazonS3Exception)
                    {
                        Console.Error.WriteLine(
                            "Failed to download {key} object.");
                    }
                    catch
                    {
                        throw new Exception($"No access to \"{Path}\" directory.");
                    }
                }
            }

            return await Task.FromResult(0);
        }
    }
}