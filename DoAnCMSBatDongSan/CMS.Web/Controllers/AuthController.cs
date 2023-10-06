using CMS_Design.IService;
using CMS_Design.Payloads.DTOs.DataResponseToken;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.Requests.AuthRequests;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }
        [HttpPost]
        [Route("/api/auth/register")]
        public async Task<IActionResult> Register(Request_Register register)
        {
            var result = await _authService.Register(register);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost]
        [Route(("/api/auth/login"))]
        public async Task<IActionResult> Login(Request_Login request)
        {
            var result = await _authService.Login(request);
            if (result == null)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("/api/auth/renew-token")]
        public IActionResult RenewToken(TokenDTO token)
        {
            var result = _authService.RenewAccessToken(token);
            if (result == null)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        [HttpPut]
        [Route("/api/auth/change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] Request_ChangePassword request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!int.TryParse(HttpContext.User.FindFirst("Id")?.Value, out int id))
                {
                    return BadRequest("Id người dùng không hợp lệ");
                }

                var result = await _authService.ChangePassword(id, request);

                if (result.ToLower().Contains("Đổi mật khẩu thành công".ToLower()))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("/api/auth/forgot-password")]
        public async Task<IActionResult> ForgotPassword(Request_ForgotPassword request)
        {
            return Ok(await _authService.ForgotPassword(request));
        }

        [HttpPost]
        [Route("/api/auth/create-new-password")]
        public async Task<IActionResult> ConfirmCreateNewPassword(Request_ConfirmCreateNewPassword request)
        {
            return Ok(await _authService.ConfirmCreateNewPassword(request));
        }

        [HttpPost]
        [Route("/api/auth/confirm-create-account")]
        public async Task<IActionResult> ConfirmCreateAccount(Request_ConfirmCreateAccount request)
        {
            return Ok(await _authService.ConfirmCreateAccount(request));
        }
        [HttpPut("/api/auth/ChangeDecentralizationForAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeDecentralizationForAdmin([FromBody]Request_ChangeDecentralization request)
        {
            return Ok(await _authService.ChangeDecentralizationForAdmin(request));
        }
        [HttpPut("/api/auth/ChangeDecentralizationForManager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangeDecentralizationForManager(Request_ChangeDecentralization request)
        {
            return Ok(await _authService.ChangeDecentralizationForManager(request));
        }
    }
}
