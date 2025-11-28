namespace AnimeHubApi.Repository.IRepository
{
    public interface IFileService
    {
        // Handle file deletion
        void DeleteFile(string? filePath);

        // Handle multiple related files (for entity deletion)
        void DeleteFiles(IEnumerable<string?> filePaths);
    }
}
