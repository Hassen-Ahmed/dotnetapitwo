
using DotnetApi.Models;
using DotnetApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Services.UserRepositoryService;

public class UserRepository : IUserRepository
{

    DataContextEF _entityFramework;

    // Constructor Method
    public UserRepository(DataContextEF context)
    {
        _entityFramework = context;
    }

    // Util Methods
    public async Task<bool> SaveChanges() => await _entityFramework.SaveChangesAsync() > 0;

    public async void AddEntity<T>(T entityToAdd)
    {
        if (entityToAdd is not null)
            await _entityFramework.AddAsync(entityToAdd);
    }

    public void RemoveEntity<T>(T entityToAdd)
    {
        if (entityToAdd is not null)
            _entityFramework.Remove(entityToAdd);
    }

    public async Task<IEnumerable<UserJobInfo>> GetAllUserJobInfo()
    {
        IEnumerable<UserJobInfo> allUserJobinfo = await _entityFramework.UserJobInfo.ToListAsync<UserJobInfo>();
        return allUserJobinfo;
    }

    public async Task<UserJobInfo> GetSingleUser(int id)
    {
        var result = await _entityFramework.UserJobInfo.FindAsync(id);

        if (result is not null)
            return result;
        else
            throw new Exception("There is no UserJobInfo by this UserId.");

    }

    public async Task<UserJobInfo> UpdateUser(int id)
    {
        UserJobInfo? result = await _entityFramework.UserJobInfo
                           .Where(u => u.UserId == id)
                           .FirstOrDefaultAsync<UserJobInfo>();

        if (result is not null)
            return result;
        else
            throw new Exception("There is no UserJobInfo by this UserId.");
    }

    public async Task<UserJobInfo> DeleteUser(int userId)
    {
        UserJobInfo? result = await _entityFramework.UserJobInfo
                          .Where(u => u.UserId == userId)
                          .FirstOrDefaultAsync<UserJobInfo>();

        if (result is not null)
            return result;
        else
            throw new Exception("There is no UserJobInfo by this UserId.");
    }
}