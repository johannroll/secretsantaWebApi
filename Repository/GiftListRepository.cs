using Microsoft.EntityFrameworkCore;
using SecretSantaApi.Data;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;

namespace SecretSantaApi.Repository
{
    public class GiftListRepository : IGiftListRepository
    {
        private readonly SecretSantaApiDbContext _context;

        public GiftListRepository(SecretSantaApiDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateListAsync(GiftList list)
        {
            await _context.Lists.AddAsync(list);
            return await SaveAsync();
        }

        public async Task<bool> DeleteListAsync(GiftList list)
        {
            _context.Lists.Remove(list);
            return await SaveAsync();
        }

        public async Task<GiftList> GetListAsync(int listId)
        {
            return await _context.Lists.Where(l => l.ListId == listId).FirstOrDefaultAsync();
            
        }

        public async Task<ICollection<GiftList>> GetListsByUserIdAsync(int userId)
        {
            return await _context.Lists.Where(l => l.UserId == userId).ToListAsync();
        }

        public async Task<bool> ListExistsAsync(int listId)
        {
            return await _context.Lists.AnyAsync(l => l.ListId == listId); 
        }

        public async Task<bool> UpdateListAsync(GiftList list)
        {
            _context.Lists.Update(list);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var save = await _context.SaveChangesAsync();
            return save > 0 ? true : false;
        }

        public async Task<bool> DeleteAllUserListsAsync(List<GiftList> lists)
        {
            _context.RemoveRange(lists);
            return await SaveAsync();
        }
    }
}
