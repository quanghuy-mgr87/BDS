using CMS_Design.Entities;
using CMS_Design.Handler.HandleEmail;
using CMS_Design.Payloads.DTOs.DataResponseToken;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.Requests.AuthRequests;
using CMS_Design.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.IService
{
    public interface IAuthService
    {
        Task<ResponseObject<UserDTO>> Register(Request_Register request);
        Task<ResponseObject<TokenDTO>> Login(Request_Login request);
        TokenDTO GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ResponseObject<TokenDTO> RenewAccessToken(TokenDTO request);
        string SendEmail(EmailTo emailTo);
        Task<string> ForgotPassword(Request_ForgotPassword request);
        Task<ResponseObject<UserDTO>> ConfirmCreateNewPassword(Request_ConfirmCreateNewPassword request);
        Task<string> ChangePassword(int userId, Request_ChangePassword request);
        Task<ResponseObject<UserDTO>> ConfirmCreateAccount(Request_ConfirmCreateAccount request);
        Task<ResponseObject<UserDTO>> ChangeDecentralizationForAdmin(Request_ChangeDecentralization request);
        Task<ResponseObject<UserDTO>> ChangeDecentralizationForManager(Request_ChangeDecentralization request);
    }
}
