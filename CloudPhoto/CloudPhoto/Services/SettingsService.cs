using Amazon;
using Amazon.S3;
using Cloudphoto.Models;

namespace Cloudphoto.Services
{
    internal class SettingsService
    {
        private static readonly string _settingsPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                ".config", 
                "cloudphoto", 
                "cloudphotorc");

        public static async Task<ClientData> GetClientData(Settings settings = null)
        {
            settings ??= await LoadSettings();

            return new ClientData()
            {
                Client = new AmazonS3Client(
                    settings.AwsSecretKeyId,
                    settings.AwsSecretAccessKey,
                    new AmazonS3Config()
                    {
                        RegionEndpoint = RegionEndpoint
                            .GetBySystemName(settings.Region),
                        ServiceURL = settings.EndpointUrl
                    }),
                Settings = settings
            };
        }

        private static async Task<Settings> LoadSettings()
        {
            if (!File.Exists(_settingsPath))
            {
                throw new Exception("Settings file doesn't exist. " +
                    "First you must use \"init\" command.");
            }

            var lines = await File.ReadAllLinesAsync(_settingsPath);

            try
            {
                var settings = new Settings()
                {
                    BucketName = lines[1].Split(" = ")[1],
                    AwsSecretKeyId = lines[2].Split(" = ")[1],
                    AwsSecretAccessKey = lines[3].Split(" = ")[1],
                    Region = lines[4].Split(" = ")[1],
                    EndpointUrl = lines[5].Split(" = ")[1]
                };

                foreach (var property in
                    settings.GetType().GetProperties())
                {
                    if (property.GetValue(settings) is string data &&
                        data.Replace(" ", "") is not "")
                    {
                        continue;
                    }

                    throw new Exception("Incorrect data in settings file.");
                }

                return settings;
            }
            catch
            {
                throw new Exception("Failed to read settings file.");
            }
        }

        public static async Task SaveSettings(Settings settings)
        {
            var path = Directory.CreateDirectory(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                    .FullName;

            path = Directory.CreateDirectory(Path.Combine(path, ".config"))
                .FullName;

            Directory.CreateDirectory(Path.Combine(path, "cloudphoto"));

            await File.WriteAllTextAsync(
                _settingsPath,
                "[DEFAULT]\n" +
                $"bucket = {settings.BucketName}\n" +
                $"aws_access_key_id = {settings.AwsSecretKeyId}\n" +
                $"aws_secret_access_key = {settings.AwsSecretAccessKey}\n" +
                $"region = {settings.Region}\n" +
                $"endpoint_url = {settings.EndpointUrl}");
        }
    }
}