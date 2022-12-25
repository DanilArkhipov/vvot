using Cloudphoto.Commands.Interfaces;
using Cloudphoto.Models;
using Cloudphoto.Services;
using CommandLine;

namespace Cloudphoto.Commands
{
    [Verb("init", HelpText = "Init settings file and create bucket.")]
    internal class InitCommand : ICommand
    {
        public async Task<int> InvokeAsync()
        {
            ShellService.SetEnvironmentVariable();

            var settings = new Settings()
            {
                Region = "ru-central1",
                EndpointUrl = "https://storage.yandexcloud.net"
            };

            Console.WriteLine("Please, write INPUT_BUCKET_NAME:");
            settings.BucketName = Console.ReadLine();

            Console.WriteLine("Please, write INPUT_AWS_ACCESS_KEY_ID:");
            settings.AwsSecretKeyId = Console.ReadLine();

            Console.WriteLine("Please, write INPUT_AWS_SECRET_ACCESS_KEY:");
            settings.AwsSecretAccessKey = Console.ReadLine();

            await SettingsService.SaveSettings(settings);

            if (await BucketService.TryCreateBucketAsync())
            {

                Console.WriteLine("New bucket created.");
            }
            else
            {
                Console.WriteLine("Failed to create bucket. There probably already exists a bucket with that name.");
            }

            return await Task.FromResult(0);
        }
    }
}