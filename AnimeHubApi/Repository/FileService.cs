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

        public string MoveFile(string? relativeTempPath, string destinationSubFolder)
        {
            if (string.IsNullOrEmpty(relativeTempPath)) return string.Empty;

            // 1. Get the absolute path starting from the API project root
            // relativeTempPath usually looks like "Images/Temp/filename.jpg"
            var sourceFullPath = GetFullPath(relativeTempPath);

            // If the source file doesn't exist (maybe already moved or deleted), return original path
            if (!File.Exists(sourceFullPath)) return relativeTempPath;

            try
            {
                // 1. Determine Parent Folder (Images vs Videos)
                // Logic: If destination is "Trailers", it goes to Videos. Otherwise, it goes to Images (Animes/Posters).
                string parentFolder = destinationSubFolder.Equals("Trailers", StringComparison.OrdinalIgnoreCase)
                                      ? "Videos"
                                      : "Images";

                // 2. Build Destination Paths
                var fileName = Path.GetFileName(sourceFullPath);

                // Example: "Images/Animes/hero.jpg"
                var destinationRelativePath = Path.Combine(parentFolder, destinationSubFolder, fileName).Replace("\\", "/");
                var destinationFullPath = GetFullPath(destinationRelativePath);

                // 3. Ensure Directory Exists
                var destinationDirectory = Path.GetDirectoryName(destinationFullPath);
                if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // 4. Move the File (Overwrite if exists to prevent errors)
                File.Move(sourceFullPath, destinationFullPath, overwrite: true);

                return destinationRelativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving file from {relativeTempPath}: {ex.Message}");
                // Return the original path so we don't lose the reference in DB if move fails
                return relativeTempPath;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string[] allowedExtensions)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Invalid file type {extension}");
            }

            // Smart Folder Selection:
            // If it's a video extension, go to Videos/Temp. Otherwise Images/Temp.
            string parentFolder = (extension == ".mp4" || extension == ".mkv") ? "Videos" : "Images";
            string targetFolder = Path.Combine(_webHostEnvironment.ContentRootPath, parentFolder, "Temp");

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path like "Images/Temp/guid.jpg"
            return Path.Combine(parentFolder, "Temp", fileName).Replace("\\", "/");
        }
    }
}
