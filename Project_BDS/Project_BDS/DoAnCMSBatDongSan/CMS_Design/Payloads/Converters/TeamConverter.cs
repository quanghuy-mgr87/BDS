using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponseTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Converters
{
    public class TeamConverter
    {
        private readonly UserConverter _userConverter;
        public TeamConverter()
        {
            _userConverter = new UserConverter();
        }
        public TeamDTO EntityToDTO(Team team)
        {
            return new TeamDTO
            {
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Description = team.Description,
                Member = team.Member,
                Name = team.Name,
                Sologan = team.Sologan,
                StatusId = team.StatusId,
                TruongPhongId = team.TruongPhongId.Value
            };
        }
    }
}
