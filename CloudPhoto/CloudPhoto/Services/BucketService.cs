using Amazon.S3;
using Amazon.S3.Model;
using Cloudphoto.Models;

namespace Cloudphoto.Services
{
    internal class BucketService
    {
        public static async Task<bool> TryCreateBucketAsync()
        {
            using (var data = await SettingsService.GetClientData())
            {
                if (!await data.Client.DoesS3BucketExistAsync(
                        data.Settings.BucketName))
                {
                    await data.Client.PutBucketAsync(new PutBucketRequest
                    {
                        BucketName = data.Settings.BucketName,
                        CannedACL = S3CannedACL.PublicReadWrite
                    });
                    return true;
                }

                return false;
            }
        }

        public static async Task UploadPhotoAsync(
            ClientData data, 
            string album, 
            string path)
        {
            await data.Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = data.Settings.BucketName,
                Key = $"{album}/{Path.GetFileName(path)}",
                FilePath = path,
                ContentType = "image/jpeg"
            });
        }

        public static async Task UploadPageAsync(
            ClientData data,
            string path)
        {
            await data.Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = data.Settings.BucketName,
                Key = $"{Path.GetFileName(path)}",
                FilePath = path,
                ContentType = "text/html"
            });
        }

        public static async Task DownloadObjectAsync(
            ClientData data, 
            string album, 
            string path, 
            string key)
        {
            await data.Client.DownloadToFilePathAsync(
                data.Settings.BucketName,
                key,
                $"{path}/{key.Remove(0, album.Length + 1)}",
                null);
        }

        public static async Task DeleteObjectAsync(
            ClientData data, 
            string key)
        {
            await data.Client.DeleteAsync(
                data.Settings.BucketName,
                key,
                null);
        }

        public static async Task<IList<string>> GetAllObjectsInAlbumAsync(
            ClientData data, 
            string album)
        {
            return await data.Client.GetAllObjectKeysAsync(
                data.Settings.BucketName,
                $"{album}/",
                null);
        }

        public static async Task<GetObjectMetadataResponse> GetObjectMetadataAsync(
            ClientData data, 
            string key)
        {
            return await data.Client.GetObjectMetadataAsync(
                data.Settings.BucketName,
                key);
        }

        public static async Task<ListObjectsResponse> GetAllObjectsAsync(
            ClientData data)
        {
            return await data.Client.ListObjectsAsync(
                data.Settings.BucketName,
                null);
        }
    }
}