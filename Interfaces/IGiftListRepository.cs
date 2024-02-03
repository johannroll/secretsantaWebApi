using SecretSantaApi.Models;

namespace SecretSantaApi.Interfaces
{
    public interface IGiftListRepository
    {
        Task<ICollection<GiftList>> GetListsByUserIdAsync(int userId);
        Task<GiftList> GetListAsync(int listId);
        Task<bool> ListExistsAsync(int listId);
        Task<bool> CreateListAsync(GiftList list);
        Task<bool> UpdateListAsync(GiftList list);
        Task<bool> DeleteListAsync(GiftList list);
        Task<bool> DeleteAllUserListsAsync(List<GiftList> lists);
        Task<bool> SaveAsync();
    }
}
