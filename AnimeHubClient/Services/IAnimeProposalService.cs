using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.AnimeProposal;

namespace AnimeHubClient.Services
{
    public interface IAnimeProposalService
    {
        Task<string?> UploadTempFileAsync(MultipartFormDataContent content, string type);
        Task<AnimeProposal?> CreateProposalAsync(AnimeProposalCreateDto dto);
        Task<IEnumerable<AnimeProposal>> GetAllProposalsAsync();
        Task<AnimeProposal?> GetProposalByIdAsync(int id);
        Task<bool> ApproveProposalAsync(int proposalId);
        Task<bool> RejectProposalAsync(int proposalId, string feedback);
    }
}
