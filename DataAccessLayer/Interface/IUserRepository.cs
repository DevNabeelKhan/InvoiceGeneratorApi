

using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IUserRepository
    { 
        Task<User> Authenticate(string userId, string password);
        Task<List<User>> GetUserId(int UserId, List<int> BatchIds); 
        Task<dynamic> ChangePassword(ChangePassword change);
 
        Task<dynamic> InsertUpdateUser(UserDto user);
        Task<dynamic> GetUserById(int? UserId, int? UserTypeId, int? PageNumber, int? PageSize, string? SearchText = null);
  
        Task<dynamic> GetRole();
        Task<dynamic> DeleteUser(int? Id);
        
    }
}



