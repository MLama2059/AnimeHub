using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.AnimeProposal;
using Microsoft.AspNetCore.Components.Forms;

namespace AnimeHubClient.Services
{
    public interface IAnimeProposalService
    {
        Task<string?> UploadTempFileAsync(MultipartFormDataContent content, string type);
        Task<string?> UploadProposalImageAsync(IBrowserFile file);
        Task<AnimeProposal?> CreateProposalAsync(AnimeProposalCreateDto dto);
        Task<IEnumerable<AnimeProposal>> GetAllProposalsAsync();
        Task<AnimeProposal?> GetProposalByIdAsync(int id);
        Task<bool> ApproveProposalAsync(int proposalId);
        Task<bool> RejectProposalAsync(int proposalId, string feedback);
    }
}
