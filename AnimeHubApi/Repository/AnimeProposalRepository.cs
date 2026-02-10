using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.AnimeProposal;
using AnimeHub.Shared.Models.Enums;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AnimeHubApi.Repository
{
    public class AnimeProposalRepository : IAnimeProposalRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public AnimeProposalRepository(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<AnimeProposal> CreateProposalAsync(int userId, AnimeProposalCreateDto dto)
        {
            var proposal = new AnimeProposal
            {
                UserId = userId,
                TargetAnimeId = dto.TargetAnimeId,
                ProposalType = dto.TargetAnimeId.HasValue ? ProposalType.Update : ProposalType.Create,
                ProposalStatus = ProposalStatus.Pending,
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                TrailerUrl = dto.TrailerUrl,
                TrailerPosterUrl = dto.TrailerPosterUrl,
                Episodes = dto.Episodes,
                Rating = dto.Rating,
                Season = dto.Season.HasValue ? (Season?)dto.Season : null,
                PremieredYear = dto.PremieredYear,
                Status = dto.Status.HasValue ? (Status?)dto.Status : null,
                CategoryId = dto.CategoryId,
                SerializedGenreIds = JsonSerializer.Serialize(dto.GenreIds),
                SerializedStudioIds = JsonSerializer.Serialize(dto.StudioIds),
            };

            _context.AnimeProposals.Add(proposal);
            await _context.SaveChangesAsync();
            return proposal;
        }

        public async Task<bool> ApproveProposalAsync(int proposalId)
        {
            // 1. Start a transaction. If file movement works but DB fails, we want to roll back.
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var proposal = await _context.AnimeProposals.FindAsync(proposalId);
                if (proposal == null || proposal.ProposalStatus != ProposalStatus.Pending)
                    return false;

                Anime animeToSave;
                var filesToPurge = new List<string?>(); // To track old files that need deleting

                if (proposal.ProposalType == ProposalType.Create)
                {
                    animeToSave = new Anime();
                    _context.Animes.Add(animeToSave);
                }
                else
                {
                    // UPDATE CASE
                    animeToSave = await _context.Animes
                        .Include(a => a.AnimeGenres)
                        .Include(a => a.AnimeStudios)
                        .FirstOrDefaultAsync(a => a.Id == proposal.TargetAnimeId);

                    if (animeToSave == null) return false;

                    // TRACK OLD FILES: If the proposal has a NEW image, the current image is now garbage.
                    // We check if the proposal path contains "Temp" to see if it's a newly uploaded file.
                    if (!string.IsNullOrEmpty(proposal.ImageUrl) && proposal.ImageUrl.Contains("/Temp/"))
                        filesToPurge.Add(animeToSave.ImageUrl);

                    if (!string.IsNullOrEmpty(proposal.TrailerPosterUrl) && proposal.TrailerPosterUrl.Contains("/Temp/"))
                        filesToPurge.Add(animeToSave.TrailerPosterUrl);

                    if (!string.IsNullOrEmpty(proposal.TrailerUrl) && proposal.TrailerUrl.Contains("/Temp/"))
                        filesToPurge.Add(animeToSave.TrailerUrl);

                    // Clear many-to-many so they can be rebuilt
                    animeToSave.AnimeGenres.Clear();
                    animeToSave.AnimeStudios.Clear();
                }

                // 2. MOVE FILES (Temp -> Final)
                // If the path doesn't contain "Temp", MoveFile logic (as you wrote it) returns the original path.
                if (!string.IsNullOrEmpty(proposal.ImageUrl))
                    animeToSave.ImageUrl = _fileService.MoveFile(proposal.ImageUrl, "Animes");

                if (!string.IsNullOrEmpty(proposal.TrailerPosterUrl))
                    animeToSave.TrailerPosterUrl = _fileService.MoveFile(proposal.TrailerPosterUrl, "Posters");

                if (!string.IsNullOrEmpty(proposal.TrailerUrl))
                    animeToSave.TrailerUrl = _fileService.MoveFile(proposal.TrailerUrl, "Trailers");

                // 3. MAP DATA
                animeToSave.Title = proposal.Title;
                animeToSave.Description = proposal.Description;
                animeToSave.Rating = proposal.Rating;
                animeToSave.Episodes = proposal.Episodes;
                animeToSave.Season = proposal.Season;
                animeToSave.PremieredYear = proposal.PremieredYear;
                animeToSave.Status = proposal.Status;
                animeToSave.CategoryId = proposal.CategoryId;

                // 4. DESERIALIZE RELATIONS
                var genreIds = string.IsNullOrEmpty(proposal.SerializedGenreIds)
                               ? new List<int>()
                               : JsonSerializer.Deserialize<List<int>>(proposal.SerializedGenreIds) ?? new();

                var studioIds = string.IsNullOrEmpty(proposal.SerializedStudioIds)
                                ? new List<int>()
                                : JsonSerializer.Deserialize<List<int>>(proposal.SerializedStudioIds) ?? new();

                foreach (var gId in genreIds) animeToSave.AnimeGenres.Add(new AnimeGenre { GenreId = gId });
                foreach (var sId in studioIds) animeToSave.AnimeStudios.Add(new AnimeStudio { StudioId = sId });

                // 5. FINALIZE
                proposal.ProposalStatus = ProposalStatus.Approved;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6. CLEANUP: Now that the DB is safe, delete the old files that were replaced.
                _fileService.DeleteFiles(filesToPurge);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log ex.Message here
                return false;
            }
        }

        public async Task<bool> RejectProposalAsync(int proposalId, string feedback)
        {
            var proposal = await _context.AnimeProposals.FindAsync(proposalId);
            if (proposal == null)
            {
                return false;
            }

            // 1. Collect files to delete
            var filesToDelete = new List<string?>
            {
                proposal.ImageUrl,
                proposal.TrailerPosterUrl,
                proposal.TrailerUrl
            };

            // 2. Delete them from the physical drive
            _fileService.DeleteFiles(filesToDelete);

            // 3. Clear the paths in the DB so we don't have broken links
            proposal.ImageUrl = null;
            proposal.TrailerUrl = null;
            proposal.TrailerPosterUrl = null;

            proposal.ProposalStatus = ProposalStatus.Rejected;
            proposal.AdminFeedback = feedback;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AnimeProposal>> GetAllProposalsAsync()
        {
            return await _context.AnimeProposals
                .Include(p => p.User)
                .Where(p => p.ProposalStatus == ProposalStatus.Pending)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<AnimeProposal?> GetProposalByIdAsync(int proposalId)
        {
            return await _context.AnimeProposals
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == proposalId);
        }
    }
}
