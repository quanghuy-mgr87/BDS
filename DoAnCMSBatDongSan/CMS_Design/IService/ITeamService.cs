using CMS_Design.Handler.HandlePagination;
using CMS_Design.Payloads.DTOs.DataResponseTeam;
using CMS_Design.Payloads.Requests.TeamRequests;
using CMS_Design.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.IService
{
    public interface ITeamService
    {
        Task<ResponseObject<TeamDTO>> CreateNewTeam(Request_CreateTeam request);
        Task<ResponseObject<TeamDTO>> UpdateTeam(int teamId, Request_UpdateTeam request);
        Task<string> DeleteTeam(int teamId);
        Task<PageResult<TeamDTO>> GetAllTeams(int pageSize = 10, int pageNumber = 1);
        Task<PageResult<TeamDTO>> GetTeambyManager(int? managerId, int pageSize = 10, int pageNumber = 1);
        Task<ResponseObject<TeamDTO>> AddManagerInTeam(int teamId, int userId);
        Task<ResponseObject<TeamDTO>> UpdateManagerForTeam(int newManager, int teamId);
    }
}
