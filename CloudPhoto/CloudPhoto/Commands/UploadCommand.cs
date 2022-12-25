using Cloudphoto.Commands.Interfaces;
using CommandLine;
using Cloudphoto.Services;

namespace Cloudphoto.Commands
{
    [Verb("upload", HelpText = "Upload photos to bucket.")]
    internal class UploadCommand : ICommand
    {
        [Option('a', "album", Required = true, HelpText = "Album where photos will be uploaded.")]
        public string Album { get; set; }

        [Option('p', "path", Required = false, HelpText = "Path from which photos will be loaded.")]
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
                try
                {
                    var paths = Directory.GetFiles(Path)
                        .Where(x => x.EndsWith(".jpeg") || x.EndsWith(".jpg"));

                    if (!paths.Any())
                    {
                        Console.Error.WriteLine(
                            $"There are no photos in {Path} directory.");
                        await Task.FromResult(0);
                    }

                    foreach (string path in paths)
                    {
                        try
                        {
                            await BucketService.UploadPhotoAsync(data, Album, path);
                        }
                        catch
                        {
                            Console.Error.WriteLine(
                                "Failed to upload {path} photo.");
                        }
                    }
                }
                catch
                {
                    throw new Exception($"No access to \"{Path}\" directory.");
                }
            }

            return await Task.FromResult(0);
        }
    }
}