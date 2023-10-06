using CMS_Design.IService;
using CMS_Design.Payloads.DTOs.DataResponseTeam;
using CMS_Design.Payloads.Requests.TeamRequests;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _iTeamService;
        public TeamController(ITeamService iTeamService)
        {
            _iTeamService = iTeamService;
        }

        [HttpPost("/api/team/create-team")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> CreateNewTeam(Request_CreateTeam request)
        {
            return Ok(await _iTeamService.CreateNewTeam(request));
        }

        [HttpGet("/api/team/get-all")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> GetAlls(int pageSize, int pageNumber)
        {
            return Ok(await _iTeamService.GetAllTeams(pageSize, pageNumber));
        }

        [HttpGet("/api/team/get-team-by-manager")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> GetTeamByManager(int managerId, int pageSize, int pageNumber)
        {
            return Ok(await _iTeamService.GetTeambyManager(managerId, pageSize, pageNumber));
        }

        [HttpPut("/api/team/update-team")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> UpdateTeam(int teamId, Request_UpdateTeam request)
        {
            return Ok(await _iTeamService.UpdateTeam(teamId, request));
        }

        [HttpDelete("/api/team/delete-team")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            return Ok(await _iTeamService.DeleteTeam(teamId));
        }
        [HttpPost("/api/team/AddManagerInTeam")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManagerInTeam(int teamId, int userId)
        {
            return Ok(await _iTeamService.AddManagerInTeam(teamId, userId));
        }
        [HttpPut("/api/team/UpdateManagerForTeam")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateManagerForTeam(int newManager, int teamId)
        {
            return Ok(await _iTeamService.UpdateManagerForTeam(newManager, teamId));
        }
    }
}
