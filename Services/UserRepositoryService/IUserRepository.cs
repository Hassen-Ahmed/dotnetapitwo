

using DotnetApi.Models;

namespace DotnetApi.Services.UserRepositoryService;
public interface IUserRepository
{
    Task<bool> SaveChanges();
    void AddEntity<T>(T entityToAdd);
    void RemoveEntity<T>(T entityToAdd);
    Task<IEnumerable<UserJobInfo>> GetAllUserJobInfo();

    Task<UserJobInfo> GetSingleUser(int id);
    Task<UserJobInfo> UpdateUser(int id);
    Task<UserJobInfo> DeleteUser(int userId);
}