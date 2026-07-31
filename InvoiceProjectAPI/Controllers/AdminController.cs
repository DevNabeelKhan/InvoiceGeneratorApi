using Amazon.S3;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessLogicLayer.Service;
using BusinessObjectLayer.Entities;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConvergeAPI.Controllers
{

    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class AdminController : ControllerBase
    {
        #region Depedenices
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAdminService _userService;
        private readonly IConfiguration _configuration;
        #endregion  
        public AdminController(IWebHostEnvironment webHostEnvironment, IAdminService userService, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _userService = userService;
            _configuration = configuration;
        }


        [HttpGet("GetUsersDetails")]
        public async Task<IActionResult> GetUsersDetails(int? Id)
        {
            try
            {
                var resp = await _userService.GetUsersDetails(Id);
                return Ok(ResponseHelper.GetSuccessResponse(resp));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        

        [HttpGet("GetUser_OG")]
        public async Task<IActionResult> GetUser_OG()
        {
            try
            {
                var resp = await _userService.GetUser_OG();
                return Ok(ResponseHelper.GetSuccessResponse(resp));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("InsertUpdateUser")]
        public async Task<IActionResult> InsertUpdateUser([FromForm] UserDto user, IFormFile? attachProfilePicture = null)
        {
            try
            {
                string ProfilePictureName = null;
                string ProfilePictureUrl = null;
                if (attachProfilePicture != null && attachProfilePicture.Length > 0)
                {
                    (ProfilePictureName, ProfilePictureUrl) = await Helper.AttachFileToS3Async(attachProfilePicture, _configuration);
                    user.PictureUrl = ProfilePictureUrl;
                }
                var result = await _userService.InsertUpdateUser(user);
                if (result.Id == 0)
                {
                    return Ok(ResponseHelper.GetDublicateResponse(result, ResponseMessage.DuplicateUser));
                }
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("DeleteTableRow")]
        public async Task<IActionResult> DeleteTableRow(string? tableName, int? Id)
        {
            try
            {
                var result = await _userService.DeleteTableRow(tableName, Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());

            }

        }

        [HttpGet("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            try
            {
                var result = await _userService.DeleteUser(Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUserById(int? UserId, int? RoleId, int? PageNumber = 1, int? PageSize = 50, string? SearchText = null)
        {
            try
            {
                var result = await _userService.GetUserById(UserId, RoleId, PageNumber, PageSize, SearchText);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
        [HttpGet("GetRole")]
        public async Task<IActionResult> GetRole()
        {
            try
            {
                var result = await _userService.GetRole();
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
        

        [HttpPost("InsertUpdateUsersDetails")]
        public async Task<IActionResult> InsertUpdateUsersDetails(UserDetailModel user)
        {
            try
            {

                var result = await _userService.InsertUpdateUsersDetails(user);
                if (result.Id == 0)
                {
                    return Ok(ResponseHelper.GetFailureResponse());
                }
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }


        [HttpGet("GetUserName")]
        public async Task<IActionResult> GetUserName()
        {
            try
            {
                var result = await _userService.GetUserName();
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
        
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(int? Id)
        {
            try
            {
                var resp = await _userService.GetUser(Id);
                return Ok(ResponseHelper.GetSuccessResponse(resp));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }


        [HttpPost("InsertUpdateUsers")]
        public async Task<IActionResult> InsertUpdateUsers(UserModel user)
        {
            try
            {

                var result = await _userService.InsertUpdateUsers(user);
                if (result.Id == 0)
                {
                    return Ok(ResponseHelper.GetDublicateResponse(result, ResponseMessage.DuplicateUser));
                }
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

    }
}

public class PdfRequest
{
    public string? HtmlContent { get; set; }
}
