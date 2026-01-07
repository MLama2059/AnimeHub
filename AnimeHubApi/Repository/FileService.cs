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

        public async Task<string> SaveProposalFileAsync(IFormFile file, string[] allowedExtensions)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"Invalid file type {extension}");

            // Logic: Videos go to Videos/Temp, Images go to Images/Temp
            string parentFolder = (extension == ".mp4" || extension == ".mkv") ? "Videos" : "Images";
            string targetFolder = Path.Combine(_webHostEnvironment.ContentRootPath, parentFolder, "Temp");

            if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

            // Create unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path (e.g., "Images/Temp/guid.jpg")
            return Path.Combine(parentFolder, "Temp", fileName).Replace("\\", "/");
        }

        public string MoveFile(string? relativeTempPath, string destinationSubFolder)
        {
            // If no path provided, nothing to move
            if (string.IsNullOrEmpty(relativeTempPath)) return string.Empty;

            // Get full physical path of the temp file
            var sourceFullPath = Path.Combine(_webHostEnvironment.ContentRootPath, relativeTempPath);

            // If file doesn't exist (maybe user didn't upload one), return empty or original
            if (!File.Exists(sourceFullPath)) return relativeTempPath;

            try
            {
                // Determine destination (Images/Animes or Videos/Trailers)
                // Note: destinationSubFolder passed from Controller will be "Animes" or "Trailers"
                string parentFolder = destinationSubFolder.Equals("Trailers", StringComparison.OrdinalIgnoreCase) ? "Videos" : "Images";

                var fileName = Path.GetFileName(sourceFullPath);

                // Final Path: Images/Animes/filename.jpg
                var destinationRelativePath = Path.Combine(parentFolder, destinationSubFolder, fileName).Replace("\\", "/");
                var destinationFullPath = Path.Combine(_webHostEnvironment.ContentRootPath, parentFolder, destinationSubFolder, fileName);

                var destDir = Path.GetDirectoryName(destinationFullPath);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir!);

                // Move the file (Renames/Moves file on disk)
                File.Move(sourceFullPath, destinationFullPath, overwrite: true);

                return destinationRelativePath;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error moving file: {ex.Message}");
                return relativeTempPath; // Fallback
            }
        }

        // Don't forget to update your IFileService interface to include these signatures!
    }
}
