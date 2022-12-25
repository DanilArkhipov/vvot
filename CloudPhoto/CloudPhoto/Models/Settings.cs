namespace Cloudphoto.Models
{
    internal class Settings
    {
        public string BucketName { get; set; }
        public string AwsSecretKeyId { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string Region { get; set; }
        public string EndpointUrl { get; set; }
    }
}