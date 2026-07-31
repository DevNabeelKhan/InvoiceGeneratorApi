using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp; 
using System.Security.Claims; 
//using PuppeteerSharp;    
namespace DataAccessLayer.Shared.Helper { 

public static class Helper
{
      public static int UserId(IHttpContextAccessor? _httpContextAccessor)
    {
        if (_httpContextAccessor?.HttpContext?.User == null)
        {
            return 0;
        }

        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
        {
            return parsedUserId;
        }
        return 0;
    }

        
        public static async Task<(string FileName, string FileUrl)> AttachFileToS3Async(IFormFile attachFile, IConfiguration configuration)
        {
            if (attachFile == null || attachFile.Length == 0)
                return (null, null);

            try
            {
             

                var fileExtension = Path.GetExtension(attachFile.FileName)?.ToLower(); 

                var sanitizedFileName = Path.GetFileNameWithoutExtension(attachFile.FileName).Replace(" ", "_").Replace("..", "");
                var fileNamePrefix = string.IsNullOrEmpty(sanitizedFileName) ? "_" : sanitizedFileName.Length > 5 ? sanitizedFileName.Substring(0, 5) : sanitizedFileName;
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var actualFileName = $"{fileNamePrefix}_{timestamp}{fileExtension}";

                var bucketName = configuration["AWS:BucketName"];
                var accessKey = configuration["AWS:AccessKey"];
                var secretKey = configuration["AWS:SecretKey"];
                var region = configuration["AWS:Region"];

                using var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);
                using var transferUtility = new TransferUtility(s3Client);

                using (var fileStream = attachFile.OpenReadStream())
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = fileStream,
                        BucketName = bucketName,
                        Key = $"docs/{actualFileName}",
                        ContentType = attachFile.ContentType
                    };
                    await transferUtility.UploadAsync(uploadRequest);
                }
                var fileUrl = $"https://{bucketName}.s3.{region}.amazonaws.com/docs/{actualFileName}";
                return (actualFileName, fileUrl);
            }
            catch (Exception ex)
            { 
                return (null, null);
            }
        }
        //public static async Task<(string FileName, string FileUrl)> AttachFileAsync(IFormFile attachFile, IWebHostEnvironment environment, int? Type)
        //{
        //    try
        //    {
        //        var allowedTypes = new[]
        //        {
        //        "image/jpeg", "image/png", "image/gif",
        //        "application/pdf",
        //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        "application/vnd.ms-excel",
        //        "text/plain",
        //        "application/msword",
        //        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        //        "application/x-rar-compressed"};

        //        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".xlsx", ".xls", ".doc", ".docx", ".rar", ".zip" };

        //        var fileExtension = Path.GetExtension(attachFile.FileName).ToLower();

        //        if (!allowedTypes.Contains(attachFile.ContentType) || !allowedExtensions.Contains(fileExtension))
        //        {
        //            return (null, null);
        //        }
        //        string? selectFolder = "";
        //        var baseUploadPath = Path.Combine(environment.WebRootPath, "Attachments", selectFolder);
        //        Directory.CreateDirectory(baseUploadPath);


        //        var sanitizedFileName = Path.GetFileNameWithoutExtension(attachFile.FileName).Replace(" ", "_").Replace("..", "");
        //        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //        var actualFileName = $"{sanitizedFileName}_{timestamp}{fileExtension}";

        //        var filePath = Path.Combine(baseUploadPath, actualFileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await attachFile.CopyToAsync(stream);
        //        }
        //        var fileUrl = $"/Attachments/{selectFolder}/{actualFileName}";
        //        return (actualFileName, fileUrl);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return (null, null);
        //}


    }


}