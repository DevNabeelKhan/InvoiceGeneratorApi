using Amazon.Runtime.Internal.Auth;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Entities;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;
using DataAccessLayer.Repositories;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessLogicLayer.Service
{
    public class AdminService : IAdminService
    {

        private readonly IAdminRepository _userRepository;
        private readonly string? _secretKey;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly int _durationInMinutes;
        private readonly IConfiguration _configuration;


        public AdminService(IAdminRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _secretKey = configuration["Jwt:SecretKey"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _durationInMinutes = int.Parse(configuration["Jwt:DurationInMinutes"]);
           _configuration  = configuration;

    }

      
        public Task<dynamic> GetUsersDetails(int? Id)
        {
            try
            {
                var res = _userRepository.GetUsersDetails(Id);
                return res;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
       
        public Task<dynamic> GetFacilityByUser(int? UserId, string? Text, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                var res = _userRepository.GetFacilityByUser(UserId, Text,PageNumber,PageSize);
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       
        public Task<dynamic> GetApplicationByUser(int? UserId, int? FacilityId, string? Text, int? StatusId, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                var res = _userRepository.GetApplicationByUser(UserId, FacilityId, Text,StatusId,PageNumber,PageSize);
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> InsertUpdateUser(UserDto user)
        {
            try
            {
                var res = await _userRepository.InsertUpdateUser(user);
                return res;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }
        public Task<dynamic> DeleteUser(int Id)
        {
            try
            {
                var res = _userRepository.DeleteUser(Id);
                return res;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }
        public Task<dynamic> GetRole()
        {
            try
            {
                var res = _userRepository.GetRole();
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
      
         
        public async Task<dynamic> GetUserById(int? UserId, int? RoleId, int? pagenumber, int? pagesize, string? SearchText = null)
        {
            try
            {
                var res = await _userRepository.GetUserById(UserId, RoleId, pagenumber, pagesize, SearchText);
                return res;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }
      
        public async Task<dynamic> InsertUpdateUsersDetails(UserDetailModel user)
        {
            try
            {
                var res = await _userRepository.InsertUpdateUsersDetails(user);
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public Task<dynamic> GetUserName()
        {
            try
            {
                var res = _userRepository.GetUserName();
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
    
        public Task<dynamic> GetUser(int? Id)
        {
            try
            {
                var res = _userRepository.GetUser(Id);       
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> InsertUpdateUsers(UserModel user)
        {
            try
            {
                var res = await _userRepository.InsertUpdateUsers(user);
                return res;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<dynamic> DeleteTableRow(string TableName, int? Id)
        {
            try
            {

                var res = await _userRepository.DeleteTableRow(TableName, Id);
                return res;
            }
            catch (Exception ex)
            {
                return (null);
            }

        }


        public Task<dynamic> GetUser_OG()
        {
            try
            {
                var res = _userRepository.GetUser_OG();      
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
      


    }
}
