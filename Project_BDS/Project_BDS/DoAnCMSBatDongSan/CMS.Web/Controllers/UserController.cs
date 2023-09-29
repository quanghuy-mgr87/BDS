using CMS_Design.IService;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _iUserService;
        public UserController(IUserService iUserService)
        {
            _iUserService = iUserService;
        }
        [HttpPut("/api/user/GetUserByPhoneNumber")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> GetUserByPhoneNumber(string phoneNumber)
        {
            return Ok(await _iUserService.GetUserByPhoneNumber(phoneNumber));
        }
        [HttpPut("/api/user/DeleteUser")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            return Ok (await _iUserService.DeleteUser(userId));
        }
        [HttpGet("/api/user/GetAllUsers")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> GetAllUsers(int pageSize, int pageNumber)
        {
            return Ok(await _iUserService.GetAllUsers(pageSize, pageNumber));
        }
        [HttpGet("/api/user/GetUserByEmail")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            return Ok(await _iUserService.GetUserByEmail(email));
        }
        [HttpGet("/api/user/GetUserByName")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> GetUserByName(string name, int pageSize, int pageNumber)
        {
            return Ok(await _iUserService.GetUserByName(name, pageSize, pageNumber));
        }
    }
}
