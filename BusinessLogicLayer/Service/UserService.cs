using Amazon.Runtime.Internal.Auth;
using BusinessLogicLayer.Interfaces;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;
using DataAccessLayer.Repositories;
using DataAccessLayer.Shared.Helper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessLogicLayer.Service
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly string? _secretKey;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly int _durationInMinutes;
        private readonly IConfiguration _configuration;


        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _secretKey = configuration["Jwt:SecretKey"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _durationInMinutes = int.Parse(configuration["Jwt:DurationInMinutes"]);
           _configuration  = configuration;

    }

    public async Task<BusinessObjectsLayer.Entities.User> Authenticate(string UserId, string password)
        {
            BusinessObjectsLayer.Entities.User user = await _userRepository.Authenticate(UserId.ToLower(),password);
            if (user == null || user.Password != password) // Password Hashed
            {
                return null;
            }
            
            var token = GenerateJwtToken(user);
            return new BusinessObjectsLayer.Entities.User { Token = token, Expiry = DateTime.UtcNow.AddMinutes(_durationInMinutes),FullName=user.FullName, Password = user.Password, UserName = user.UserName,  Id = user.Id,PictureUrl=user.PictureUrl };
        }

        public async Task<BusinessObjectsLayer.Entities.User> AuthenticateWithGoogle(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                var user = await _userRepository.GetUserByEmail(payload.Email.ToLower());

                if (user == null)
                {
                    user = new BusinessObjectsLayer.Entities.User
                    {
                        FullName = payload.Name,
                        UserName = payload.Email,
                        Email = payload.Email,
                        PictureUrl = payload.Picture
                    };
                }

                var token = GenerateJwtToken(user);
                return new BusinessObjectsLayer.Entities.User
                {
                    Token = token,
                    Expiry = DateTime.UtcNow.AddMinutes(_durationInMinutes),
                    FullName = user.FullName ?? payload.Name,
                    UserName = user.UserName ?? payload.Email,
                    Email = payload.Email,
                    Id = user.Id,
                    PictureUrl = user.PictureUrl ?? payload.Picture
                };
            }
            catch (InvalidJwtException)
            {
                return null;
            }
        }

        public async Task<dynamic> ChangePassword(ChangePassword changePassword)
        {
            var user = await _userRepository.ChangePassword(changePassword);
            return user;
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

        public async Task<List<BusinessObjectsLayer.Entities.User>> GetUserId(int UserId, List<int> BatchIds)
        {
            var user = await _userRepository.GetUserId(UserId,BatchIds);
            return user;
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
        private string GenerateJwtToken(BusinessObjectsLayer.Entities.User user)
        {
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString()),
                new Claim(ClaimTypes.Name,user.FullName?.ToString()),
               new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, user.UserName?.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_durationInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       

    }
}
