//using Microsoft.AspNetCore.Mvc.ModelBinding; 
using System.ComponentModel;
using BusinessObjectsLayer.Entities;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BusinessLogicLayer.Response
{
    public enum ResponseMessage
    {
        [Description("Invalid input")]
        InvalidInput = 10001,
        [Description("Not found.")]
        NotFound,
        [Description("Error accord.")]
        Exception,
        [Description("You're now authorized.")]
        Authorized,
        [Description("Login successfully.")]
        LoginSuccess,
        [Description("You'r not authorized to accesss this API.")]
        UnAuthorized,
        [Description("Email address is already exist in our system.")]
        EmailAlreadyExists,
        [Description("Email address doesn't exist in our system.")]
        EmailDoesNotExist,
        [Description("Invalid email address or password. ")]
        InvalidCredentials,
        [Description("Invalid username and password Please try again")]
        AccountNoVerified,
        [Description("Signup successfully")]
        SignUpSucess,
        [Description("Password reset link is invalid.")]
        InvalidResetLink,
        [Description("Deleted successfuly")]
        Deleted,
        [Description("Invalid emaill address.")]
        InvalidEmaill,
        [Description("Successfuly.")]
        Successfuly,
        [Description("Record Successfuly inserted.")]
        RecordInserted,
        [Description("Record Already Exists.")]
        DublicateRecord,
        [Description("Record Has Been Successfuly Updated")]
        RecordUpdated,
        [Description("Record Has Been Successfuly Processed")]
        RecordProcess,
        [Description("User name or email already exists.")]
        DuplicateUser,
    }
    public static class ResponseHelper
    {
        
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);
                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return null;
        }
        private static string GetString(ResponseMessage message)
        {
            if (message != 0)
            {
                return message.GetDescription();
            }
            return null;
        }
        public static ResponseModel GetSuccessResponse(object o, ResponseMessage message = 0)
        {
            return new ResponseModel()
            {
                Response = true,
                StatusCode = 200,
                IsSuccess= true,
                Data = o,
                Message = GetString(ResponseMessage.RecordProcess),
            };
        }

        public static ResponseModel GetDublicateResponse(object o, ResponseMessage message = 0)
        {
            return new ResponseModel()
            {
                Response = true,
                StatusCode = 187,
                IsSuccess = false,
                Data = o,
                Message = GetString(message),
            };
        }

        public static ResponseModel GetValidationFailureResponse(ModelStateDictionary dictionary = null)
        {
            var csvMessages = dictionary.Values.Select(x => String.Join(Environment.NewLine, x.Errors.Select(y => y.ErrorMessage)));
            var errors = String.Join(Environment.NewLine, csvMessages);
            return new ResponseModel()
            {
                Response = false,
                Data = null,
                IsSuccess = false,
                StatusCode =400,
                Message = errors
            };
        }
        public static ResponseModel GetNotFoundResponse(ResponseMessage message = ResponseMessage.NotFound)
        {
            return new ResponseModel()
            {
                Response = true,
                Data = null,
                IsSuccess = false,
                StatusCode = 404,
                Message = GetString(message),
            };
        }

        public static ResponseModel GetExceptionResponse(ResponseMessage message = ResponseMessage.Exception)
        {
            return new ResponseModel()
            {
                Response = false,
                Data = null,
                StatusCode = 500,
                Message = GetString(message)
            };
        }
        public static ResponseModel GetFailureResponse(ResponseMessage message = ResponseMessage.InvalidInput, ModelStateDictionary dictionary = null)
        {
            var errors = dictionary?.Select(x => x.Value.Errors).Where(y => y.Any());
            return new ResponseModel()
            {
                Response = false,
                StatusCode = 400,
                Message = GetString(message)
            };
        }
        public static ResponseModel GetFailureResponse(string message, ModelStateDictionary dictionary = null)
        {
            var errors = dictionary?.Select(x => x.Value.Errors).Where(y => y.Any());
            return new ResponseModel()
            {
                Response = false,
                Data = null,
                StatusCode = 400,
                IsSuccess = false, 
                Message = message
            };
        }
        public static ResponseModel GetAuthorizedResponse(ResponseMessage message = ResponseMessage.Authorized)
        {

            return new ResponseModel()
            {
                Response = true,
                Data = null,
                StatusCode = 200,
                IsSuccess = false,
                Message = GetString(message)
            };
        }

        public static ResponseModel GetUnAuthorizedResponse(ResponseMessage message = ResponseMessage.UnAuthorized)
        {
            return new ResponseModel()
            {
                Response = false,
                Data = null,
                IsSuccess = false,
                Message = GetString(message),
                StatusCode = 400
            };
        }
    }
}
