namespace AnimeHubApi.Repository.IRepository
{
    public interface IFileService
    {
        // Handle file deletion
        void DeleteFile(string? filePath);

        // Handle multiple related files (for entity deletion)
        void DeleteFiles(IEnumerable<string?> filePaths);

        // New method for moving temp files to permanent storage
        string MoveFile(string? relativeTempPath, string destinationSubFolder);
        Task<string> SaveFileAsync(IFormFile file, string[] allowedExtensions);
    }
}
