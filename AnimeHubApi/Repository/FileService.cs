using AnimeHubApi.Repository.IRepository;

namespace AnimeHubApi.Repository
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetFullPath(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Empty;
            }

            var cleanPath = relativePath.Replace("\\", "/");

            // Combine the server's root path (usually wwwroot) with the relative path
            return Path.Combine(_webHostEnvironment.ContentRootPath, cleanPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        }

        public void DeleteFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var fullPath = GetFullPath(filePath);

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {fullPath}: {ex.Message}");
                }
            }
        }

        public void DeleteFiles(IEnumerable<string?> filePaths)
        {
            if (filePaths == null)
            {
                return;
            }

            foreach (var path in filePaths)
            {
                DeleteFile(path);
            }
        }
    }
}
