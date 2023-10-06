using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Converters
{
    public class UserConverter
    {
        public UserDTO EntityToDTO(User user)
        {
            return new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
