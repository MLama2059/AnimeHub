using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.AnimeProposal;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IAnimeProposalRepository
    {
        Task<AnimeProposal> CreateProposalAsync(int userId, AnimeProposalCreateDto dto);
        Task<bool> ApproveProposalAsync(int proposalId);
        Task<bool> RejectProposalAsync(int proposalId, string feedback);
        Task<IEnumerable<AnimeProposal>> GetAllProposalsAsync();
        Task<AnimeProposal?> GetProposalByIdAsync(int proposalId);
    }
}
