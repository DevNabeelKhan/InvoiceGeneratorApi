using BusinessObjectsLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;




using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    { 
        Task<User> Authenticate(string UserId,string UserPassword);
        Task<List<User>> GetUserId(int UserId, List<int> BatchIds); 
        Task<dynamic> ChangePassword(ChangePassword changePassword);
        Task<dynamic> InsertUpdateUser(UserDto user);
        Task<dynamic> GetUserById(int? UserId,int? RoleId, int?pagenumber,int?pagesize, string? SearchText = null);
        Task<dynamic> DeleteUser(int Id); 
        Task<dynamic> GetRole();
        
        
    }
}
