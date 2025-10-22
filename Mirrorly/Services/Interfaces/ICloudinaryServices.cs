namespace Mirrorly.Services.Interfaces
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    public interface ICloudinaryServices
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
