using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace BLL
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile);
        void ModifyFile(string fileNameWithExtension);
        void DeleteFile(string fileNameWithExtension);
        public Task<byte[]> GetFileAsync(string imageName);
    }
    public class FileService(IWebHostEnvironment environment) : IFileService
    {
        public async Task<string> SaveFileAsync(IFormFile imageFile)
        {
            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "wwwroot");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = imageFile.FileName;
            var fileNameWithPath = Path.Combine(path, fileName);
            await using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return fileName;
        }
        public void ModifyFile(string fileNameWithExtension)
        {
            throw new NotImplementedException();
        }
        public void DeleteFile(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }
            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "wwwroot", fileNameWithExtension);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {fileNameWithExtension}");
            }
            File.Delete(path);
        }

        public async Task<byte[]> GetFileAsync(string imageName)
        {
            if (string.IsNullOrEmpty(imageName) || imageName.Contains(".."))
            {
                throw new ArgumentException("Invalid file name.");
            }

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "wwwroot", imageName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {imageName}");
            }

            return await File.ReadAllBytesAsync(path);
        }
    }
}
