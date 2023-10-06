using CMS_Design.Handler.HandleEmail;
using CMS_Design.IService;
using CMS_Design.Payloads.Converters;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.Requests.UserRequests;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_ActionLayer.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly ResponseObject<UserDTO> _responseObject;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserConverter _userConverter;
        public UserService(ResponseObject<UserDTO> responseObject, IHttpContextAccessor httpContextAccessor, UserConverter userConverter)
        {
            _responseObject = responseObject;
            _httpContextAccessor = httpContextAccessor;
            _userConverter = userConverter;
        }
        #region Xóa người dùng
        public async Task<string> DeleteUser(int userId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId);
            if(user is null)
            {
                return "Không tìm thấy người dùng";
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            user.IsActive = false;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return "Xóa người dùng thành công";
        }
        #endregion
        #region Lấy ra tất cả người dùng
        public async Task<IQueryable<UserDTO>> GetAllUsers(int pageSize, int pageNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            return _context.users.Where(x => x.IsActive == true).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _userConverter.EntityToDTO(x));
        }
        #endregion
        #region Lấy người dùng theo email
        public async Task<ResponseObject<UserDTO>> GetUserByEmail(string email)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            var user = await _context.users.SingleOrDefaultAsync(x => x.IsActive == true && x.Email.Equals(email));
            if(user == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }
            return _responseObject.ResponseSuccess("Lấy dữ liệu người dùng thành công", _userConverter.EntityToDTO(user));
        }
        #endregion
        #region Lấy người dùng theo tên
        public async Task<IQueryable<UserDTO>> GetUserByName(string name, int pageSize, int pageNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            return _context.users.Where(x => x.IsActive == true && ChuanHoaChuoi(x.Name).Contains(ChuanHoaChuoi(name))).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _userConverter.EntityToDTO(x));
        }
        #endregion
        #region Lấy người dùng qua số điện thoại
        public async Task<ResponseObject<UserDTO>> GetUserByPhoneNumber(string phoneNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            var user = await _context.users.SingleOrDefaultAsync(x =>  x.PhoneNumber == phoneNumber);
            if (user == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }
            return _responseObject.ResponseSuccess("Lấy thông tin người dùng thành công", _userConverter.EntityToDTO(user));
        }
        #endregion
        #region Cập nhật thông tin người dùng
        public async Task<ResponseObject<UserDTO>> UpdateUserInformation(Request_UpdateUser request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == request.UserId);
            if(user == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }
            user.DateOfBirth = request.DateOfBirth;
            user.UpdateTime = DateTime.Now;
            user.Email = request.Email;
            user.Name = request.Name;
            user.TeamId = request.TeamId;
            if (!Validate.IsValidPhoneNumber(request.PhoneNumber))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng số điện thoại không hợp lệ", null);
            }
            user.PhoneNumber = request.PhoneNumber;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin người dùng thành công", _userConverter.EntityToDTO(user));
        }
        #endregion
        #region Xử lí chuẩn hóa chuỗi
        private string ChuanHoaChuoi(string str)
        {
            str = str.ToLower().Trim();
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }
        #endregion
        #region Thêm thành viên vào phòng ban
        public async Task<ResponseObject<UserDTO>> AddUserInTeam(int userId, int teamId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId);
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new NotImplementedException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager"))
            {
                throw new NotImplementedException("Người dùng không có quyền sử dụng chức năng này");
            }
            if (user == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }

            var team = await _context.teams.SingleOrDefaultAsync(x => x.Id == teamId);
            if (team == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy phòng ban", null);
            }
            user.TeamId = teamId;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            team.Member += 1;
            _context.teams.Update(team);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm nhân viên vào phòng ban thành công", _userConverter.EntityToDTO(user));
        }
        #endregion
    }
}
