using BusinessObjectLayer.Entities;
using BusinessObjectsLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAdminService
    {
     
        Task<dynamic> GetUsersDetails(int? Id);
       
        Task<dynamic> GetUser_OG();

        Task<dynamic> InsertUpdateUser(UserDto user);
     
        Task<dynamic> InsertUpdateUsersDetails(UserDetailModel user);

       

        Task<dynamic> GetUserById(int? UserId, int? RoleId, int? pagenumber, int? pagesize, string? SearchText = null);
        Task<dynamic> DeleteUser(int Id);
     
        Task<dynamic> GetUserName();
     
        Task<dynamic> GetRole();
       
        Task<dynamic> GetUser(int? Id);
        Task<dynamic> InsertUpdateUsers(UserModel user);
        Task<dynamic> DeleteTableRow(string? tableName, int? Id);


    }
}
