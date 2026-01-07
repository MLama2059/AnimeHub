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
            var proposal = await _context.AnimeProposals.FindAsync(proposalId);

            if (proposal == null || proposal.ProposalStatus != ProposalStatus.Pending)
            {
                return false;
            }

            Anime animeToSave;

            // STRATEGY: Determine if we are Creating or Updating
            if (proposal.ProposalType == ProposalType.Create)
            {
                animeToSave = new Anime();
                _context.Animes.Add(animeToSave); // Add to tracking
            }
            else
            {
                // Load existing anime including relations so we can clear them
                animeToSave = await _context.Animes
                    .Include(a => a.AnimeGenres)
                    .Include(a => a.AnimeStudios)
                    .FirstOrDefaultAsync(a => a.Id == proposal.TargetAnimeId);

                if (animeToSave == null) return false; // Target anime was deleted?

                // Clear existing relations to prevent duplicates
                animeToSave.AnimeGenres.Clear();
                animeToSave.AnimeStudios.Clear();
            }

            // --- FILE MOVING LOGIC ---
            // We use the "MoveFile" method we just created.
            // 1. ImageUrl -> Images/Animes
            if (!string.IsNullOrEmpty(proposal.ImageUrl))
            {
                animeToSave.ImageUrl = _fileService.MoveFile(proposal.ImageUrl, "Animes");
            }

            // 2. TrailerPosterUrl -> Images/Posters
            if (!string.IsNullOrEmpty(proposal.TrailerPosterUrl))
            {
                animeToSave.TrailerPosterUrl = _fileService.MoveFile(proposal.TrailerPosterUrl, "Posters");
            }

            // 3. TrailerUrl -> Videos/Trailers
            if (!string.IsNullOrEmpty(proposal.TrailerUrl))
            {
                animeToSave.TrailerUrl = _fileService.MoveFile(proposal.TrailerUrl, "Trailers");
            }

            // --- MAP DATA FROM PROPOSAL TO REAL ENTITY ---
            animeToSave.Title = proposal.Title;
            animeToSave.Description = proposal.Description;
            // Note: We already set the URLs above via MoveFile
            animeToSave.Rating = proposal.Rating;
            animeToSave.Episodes = proposal.Episodes;
            animeToSave.Season = proposal.Season;
            animeToSave.PremieredYear = proposal.PremieredYear;
            animeToSave.Status = proposal.Status;
            animeToSave.CategoryId = proposal.CategoryId;

            // --- DESERIALIZE JSON & REBUILD RELATIONS ---
            var genreIds = JsonSerializer.Deserialize<List<int>>(proposal.SerializedGenreIds) ?? new List<int>();
            var studioIds = JsonSerializer.Deserialize<List<int>>(proposal.SerializedStudioIds) ?? new List<int>();

            animeToSave.AnimeGenres = genreIds.Select(gId => new AnimeGenre { GenreId = gId }).ToList();
            animeToSave.AnimeStudios = studioIds.Select(sId => new AnimeStudio { StudioId = sId }).ToList();

            // --- FINALIZE PROPOSAL STATUS ---
            proposal.ProposalStatus = ProposalStatus.Approved;

            await _context.SaveChangesAsync();
            return true;
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
