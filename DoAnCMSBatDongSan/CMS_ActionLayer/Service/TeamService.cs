using CMS_Design.Entities;
using CMS_Design.Handler.HandlePagination;
using CMS_Design.IService;
using CMS_Design.Payloads.Converters;
using CMS_Design.Payloads.DTOs.DataResponseTeam;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.Requests.TeamRequests;
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
    public class TeamService : BaseService, ITeamService
    {
        private readonly ResponseObject<TeamDTO> _responseObject;
        private readonly TeamConverter _teamConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserConverter _userConverter;
        public TeamService()
        {
            _httpContextAccessor = new HttpContextAccessor();
            _teamConverter = new TeamConverter();
            _responseObject = new ResponseObject<TeamDTO>();
            _userConverter = new UserConverter();
        }
        #region Tạo phòng ban mới
        public async Task<ResponseObject<TeamDTO>> CreateNewTeam(Request_CreateTeam request)
        {
            if(string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Code) || request.TruongPhongId == null || string.IsNullOrWhiteSpace(request.Description))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if(await _context.users.FirstOrDefaultAsync(x => x.Id == request.TruongPhongId) is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, $"Trưởng phòng có id: {request.TruongPhongId} không tồn tại", null);
            }
            else
            {
                try
                {
                    Team team = new Team();
                    team.Code = request.Code;
                    team.Name = request.Name;
                    team.Description = request.Description;
                    team.Sologan = request.Sologan;
                    team.StatusId = 2;
                    team.Member = 0;
                    team.TruongPhongId = request.TruongPhongId;
                    team.CreateTime = DateTime.Now;
                    team.UpdateTime = DateTime.Now;
                    await _context.teams.AddAsync(team);
                    await _context.SaveChangesAsync();
                    TeamDTO teamDTO = _teamConverter.EntityToDTO(team);
                    teamDTO.Users = _context.users.Where(x => x.TeamId == team.Id).Select(x => _userConverter.EntityToDTO(x)).ToList();
                    return _responseObject.ResponseSuccess("Thêm phòng ban thành công", teamDTO);
                }catch(Exception ex)
                {
                    return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
                }
            }
        }
        #endregion
        #region Xóa phòng ban
        public async Task<string> DeleteTeam(int teamId)
        {
            var team = await _context.teams.FirstOrDefaultAsync(x => x.Id == teamId && x.StatusId == 1);
            if(team == null)
            {
                return "Phòng ban đang không tồn tại hoặc đã ngưng hoạt động";
            }
            team.StatusId = 2;
            _context.teams.Update(team);
            await _context.SaveChangesAsync();
            return "Ngưng hoạt động phòng ban thành công";
        }
        #endregion
        #region Lấy ra tất cả phòng ban
        public async Task<PageResult<TeamDTO>> GetAllTeams(int pageSize = 10, int pageNumber = 1)
        {
            var teamQuery = await _context.teams.OrderBy(x => x.Id).Include(x => x.Users).Where(x => x.StatusId == 1).AsNoTracking().ToListAsync();
            var listTeamDTO = new List<TeamDTO>();
            foreach(var team in teamQuery)
            {
                var teamDTO = _teamConverter.EntityToDTO(team);
                if(teamDTO != null)
                {
                    listTeamDTO.Add(teamDTO);
                }
            }
            var data =  Pagination.GetPagedData(listTeamDTO.AsQueryable(), pageSize, pageNumber);
            return data;
        }
        #endregion
        #region Lấy ra phòng ban theo trưởng phòng
        public async Task<PageResult<TeamDTO>> GetTeambyManager(int? managerId, int pageSize = 10, int pageNumber = 1)
        {
            var teamQuery = await _context.teams.OrderBy(x => x.Id).Include(x => x.Users).Where(x => x.StatusId == 1 && x.TruongPhongId == managerId).AsNoTracking().ToListAsync();
            var listTeamDTO = new List<TeamDTO>();
            foreach (var team in teamQuery)
            {
                var teamDTO = _teamConverter.EntityToDTO(team);
                if (teamDTO != null)
                {
                    listTeamDTO.Add(teamDTO);
                }
            }
            var data = Pagination.GetPagedData(listTeamDTO.AsQueryable(), pageSize, pageNumber);
            return data;
        }
        #endregion
        #region Cập nhật thông tin phòng ban
        public async Task<ResponseObject<TeamDTO>> UpdateTeam(int teamId, Request_UpdateTeam request)
        {
            var team = await _context.teams.FirstOrDefaultAsync(x => x.Id == teamId);
            if(team is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Phòng ban không tồn tại", null);
            }
            team.UpdateTime = DateTime.Now;
            team.Name = request.Name;
            team.CreateTime = DateTime.Now;
            team.Member = _context.users.Count(x => x.TeamId == teamId);
            team.Description = request.Description;
            team.Sologan = request.Sologan;
            team.TruongPhongId = request.TruongPhongId;
            _context.teams.Update(team);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật phòng ban thành công", _teamConverter.EntityToDTO(team));

        }
        #endregion
        #region Thêm và thay trưởng phòng cho phòng ban
        public async Task<ResponseObject<TeamDTO>> AddManagerInTeam(int teamId, int userId)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId);
            if(user == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }
            var team = await _context.teams.SingleOrDefaultAsync(x => x.Id == teamId);
            if(team == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy phòng ban", null);
            }
            team.TruongPhongId = userId;
            user.TeamId = teamId;
            team.StatusId = 1;
            _context.users.Update(user);
            _context.teams.Update(team);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm trưởng phòng cho phòng ban thành công", _teamConverter.EntityToDTO(team));
        }
        public async Task<ResponseObject<TeamDTO>> UpdateManagerForTeam(int newManager, int teamId)
        {
            var team = await _context.teams.SingleOrDefaultAsync(x => x.Id == teamId && x.StatusId == 1);
            if(team == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy phòng ban", null);
            }
            var manager = await _context.users.SingleOrDefaultAsync(x => x.Id == newManager && x.RoleId == 4);
            if(manager == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }
            manager.TeamId = teamId;
            team.TruongPhongId = newManager;
            _context.users.Update(manager);
            _context.teams.Update(team);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thay đổi trưởng phòng thành công", _teamConverter.EntityToDTO(team));

        }
        #endregion

    }
}
