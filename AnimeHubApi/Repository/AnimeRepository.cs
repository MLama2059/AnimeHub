using AnimeHubApi.Data;
using AnimeHubApi.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AnimeHubApi.Repository
{
    public class AnimeRepository : IAnimeRepository
    {
        private readonly AnimeDbContext _context;

        public AnimeRepository(AnimeDbContext context)
        {
            _context = context;
        }

        public async Task<Anime> AddAsync(Anime anime)
        {
            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();
            return anime;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime is null)
            {
                return false;
            }
            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Animes.Any(x => x.Id == id);
        }

        public async Task<List<Anime>> GetAllAsync()
        {
            return await _context.Animes.ToListAsync();
        }

        public async Task<Anime?> GetByIdAsync(int id)
        {
            return await _context.Animes.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, Anime anime)
        {
            if (id != anime.Id)
            {
                return false;
            }

            _context.Entry(anime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}
