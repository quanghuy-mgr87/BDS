using CMS_Design.Entities;
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
        private readonly UserConverter _userConverter;
        public TeamService()
        {
            _teamConverter = new TeamConverter();
            _responseObject = new ResponseObject<TeamDTO>();
            _userConverter = new UserConverter();
        }
        #region CRUD Team
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
                    team.StatusId = 1;
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

        public async Task<IEnumerable<TeamDTO>> GetAllTeams(int pageSize, int pageNumber)
        {
            var teamsQuery = await _context.teams
                .OrderBy(t => t.Id).Include(x => x.Users)
                .Where(x => x.StatusId == 1)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            var teamDTOs = teamsQuery.Select(x =>
            {
                var teamDTO = _teamConverter.EntityToDTO(x);
                if (x.Users != null)
                {
                    teamDTO.Users = _context.users.Where(y => y.TeamId == x.Id).Select(y => _userConverter.EntityToDTO(y)).ToList();
                }
                else
                {
                    teamDTO.Users = new List<UserDTO>();
                }

                return teamDTO;
            });

            return teamDTOs;
        }



        public async Task<IEnumerable<TeamDTO>> GetTeambyManager(int managerId, int pageSize, int pageNumber)
        {
            var teamsQuery = await _context.teams.Where(x => x.TruongPhongId == managerId)
                .OrderBy(t => t.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            var teamDTOs = teamsQuery.Select(x =>
            {
                var teamDTO = _teamConverter.EntityToDTO(x);
                if (x.Users != null)
                {
                    teamDTO.Users = _context.users.Select(y => _userConverter.EntityToDTO(y)).ToList();
                }
                else
                {
                    teamDTO.Users = new List<UserDTO>();
                }

                return teamDTO;
            });

            return teamDTOs;
        }

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
    }
}
