using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.Requests.UserRequests;
using CMS_Design.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.IService
{
    public interface IUserService
    {
        Task<ResponseObject<UserDTO>> UpdateUserInformation(Request_UpdateUser request);
        Task<string> DeleteUser(int userId);
        Task<IQueryable<UserDTO>> GetAllUsers(int pageSize, int pageNumber);
        Task<IQueryable<UserDTO>> GetUserByName(string name, int pageSize, int pageNumber);
        Task<ResponseObject<UserDTO>> GetUserByPhoneNumber(string phoneNumber);
        Task<ResponseObject<UserDTO>> GetUserByEmail(string email);
        Task<ResponseObject<UserDTO>> AddUserInTeam(int userId, int teamId);
    }
}
