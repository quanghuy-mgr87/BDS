using CMS_Design.Entities;
using CMS_Design.Handler.HandleEmail;
using CMS_Design.IService;
using CMS_Design.Payloads.Converters;
using CMS_Design.Payloads.Requests.AuthRequests;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BcryptNet = BCrypt.Net.BCrypt;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using CMS_Design.Payloads.DTOs.DataResponseToken;
using CMS_Design.Payloads.DTOs.DataResponseUser;

namespace CMS_ActionLayer.Service
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ResponseObject<UserDTO> _responseObject;
        private readonly UserConverter _userConverter;
        private readonly IConfiguration _configuration;
        private readonly ResponseObject<TokenDTO> _responseObjectToken;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ResponseObject<UserDTO> responseObject, UserConverter userConverter, IConfiguration configuration, ResponseObject<TokenDTO> responseObjectToken, IHttpContextAccessor httpContextAccessor)
        {
            _responseObject = responseObject;
            _userConverter = userConverter;
            _configuration = configuration;
            _responseObjectToken = responseObjectToken;
            _httpContextAccessor = httpContextAccessor;
        }
        #region Đổi mật khẩu, nhận id người dùng chính là id của thằng đang đăng nhập
        public async Task<string> ChangePassword(int userId, Request_ChangePassword request)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.Id == userId);
            bool checkPass = BcryptNet.Verify(request.OldPassword, user.Password);
            if(!checkPass)
            {
                return "Mật khẩu cũ không chính xác";
            }
            if(request.NewPassword != request.ConfirmNewPassword)
            {
                return "Mật khẩu không trùng nhau! Vui lòng thử lại";
            }
            user.Password = BcryptNet.HashPassword(request.NewPassword);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return "Thay đổi mật khẩu thành công";
        }
        #endregion
        #region Xác nhận tạo mật khẩu mới sau khi người dùng đã nhập đúng code
        public async Task<ResponseObject<UserDTO>> ConfirmCreateNewPassword(Request_ConfirmCreateNewPassword request)
        {
            ConfirmEmail confirmEmail = await _context.confirmEmails.Where(x => x.Code.Equals(request.ConfirmCode)).FirstOrDefaultAsync();
            if (confirmEmail is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận không chính xác", null);
            }
            if (confirmEmail.ExpiredDateTime < DateTime.Now)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            User user = _context.users.FirstOrDefault(x => x.Id == confirmEmail.UserId);
            user.Password = BcryptNet.HashPassword(request.NewPassword);
            _context.confirmEmails.Remove(confirmEmail);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Tạo mật khẩu mới thành công", _userConverter.EntityToDTO(user));
        }
        #endregion
        #region Quên mật khẩu, gửi mã xác nhận về email
        public async Task<string> ForgotPassword(Request_ForgotPassword request)
        {
            User user = await _context.users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user is null)
            {
                return "Email không tồn tại trong hệ thống";
            }
            else
            {
                var confirms = _context.confirmEmails.Where(x => x.UserId == user.Id).ToList();
                _context.confirmEmails.RemoveRange(confirms);
                await _context.SaveChangesAsync();
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    IsConfirm = false,
                    ExpiredDateTime = DateTime.Now.AddHours(4),
                    Code = "MyBugs" + "_" + GenerateCodeActive().ToString()
                };
                await _context.confirmEmails.AddAsync(confirmEmail);
                await _context.SaveChangesAsync();
                string message = SendEmail(new EmailTo
                {
                    To = request.Email,
                    Subject = "Nhận mã xác nhận để tạo mật khẩu mới từ đây: ",
                    Content = $"Mã kích hoạt của bạn là: {confirmEmail.Code}, mã này sẽ hết hạn sau 4 tiếng"
                });
                return "Gửi mã xác nhận về email thành công, vui lòng kiểm tra email";
            }
        }
        #endregion
        #region Tạo ra code để active tài khoản
        private int GenerateCodeActive()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
        #endregion
        #region Tạo access token
        public TokenDTO GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value!);

            var decentralization = _context.userRoles.FirstOrDefault(x => x.Id == user.RoleId);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    //new Claim("Decentralization", user.Role.Code),
                    new Claim("UserName", user.UserName),
                    //new Claim("RoleId", user.RoleId.ToString()),
                    //new Claim(ClaimTypes.Role, decentralization?.Code ?? "")
                }),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken
            {
                Token = refreshToken,
                ExpiredTime = DateTime.Now.AddHours(4),
                UserId = user.Id
            };

            _context.refreshTokens.Add(rf);
            _context.SaveChanges();

            TokenDTO tokenDTO = new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokenDTO;
        }
        #endregion
        #region Tạo refresh token
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        #endregion
        #region Đăng nhập tài khoản
        public async Task<ResponseObject<TokenDTO>> Login(Request_Login request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            var phatTu = await _context.users.FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName));
            if (phatTu is null)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status404NotFound, "Tên tài khoản không tồn tại trên hệ thống", null);
            }
            bool checkPass = BcryptNet.Verify(request.Password, phatTu.Password);
            if (!checkPass)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu không chính xác", null);
            }
            else
            {
                return _responseObjectToken.ResponseSuccess("Đăng nhập thành công", GenerateAccessToken(phatTu));
            }
        }
        #endregion
        #region Đăng ký tài khoản người dùng
        public async Task<ResponseObject<UserDTO>> Register(Request_Register request)
        {
            if(string.IsNullOrWhiteSpace(request.UserName) 
             || string.IsNullOrWhiteSpace(request.Password)
             || string.IsNullOrWhiteSpace(request.Email)
             || string.IsNullOrWhiteSpace(request.PhoneNumber)
                )
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if(!Validate.IsValidEmail(request.Email))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng email không hợp lệ", null);
            }
            if (!Validate.IsValidPhoneNumber(request.PhoneNumber))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng số điện thoại không hợp lệ", null);
            }
            if(await _context.users.FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Tên tài khoản đã tồn tại trên hệ thống", null);
            }
            if (await _context.users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Email đã tồn tại trên hệ thống", null);
            }
            if(await _context.users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(request.PhoneNumber)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Số điện thoại đã tồn tại trên hệ thống", null);
            }
            else
            {
                try
                {
                    User user = new User();
                    user.UserName = request.UserName;
                    user.Email = request.Email;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Password = BcryptNet.HashPassword(request.Password);
                    user.CreateTime = DateTime.Now;
                    user.UpdateTime = DateTime.Now;
                    user.DateOfBirth = request.DateOfBirth;
                    user.Name = request.Name;
                    //user.RoleId = 6;
                    //user.StatusId = 1;
                    var team = await _context.teams.FirstOrDefaultAsync(x => x.Id == request.TeamId && x.StatusId == 1);
                    //if(team is null)
                    //{
                    //    throw new Exception($"Team có id: {request.TeamId} không tồn tại");
                    //}
                    //user.TeamId = request.TeamId;
                    await _context.users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    //team.Member = _context.users.Count(x => x.TeamId == team.Id);
                    //_context.teams.Update(team);
                    //await _context.SaveChangesAsync();
                    ConfirmEmail confirmEmail = new ConfirmEmail
                    {
                        UserId = user.Id,
                        IsConfirm = false,
                        ExpiredDateTime = DateTime.Now.AddHours(24),
                        Code = "MyBugs" + "_" + GenerateCodeActive().ToString(),
                        RequiredDateTime = DateTime.Now
                    };
                    await _context.confirmEmails.AddAsync(confirmEmail);
                    await _context.SaveChangesAsync();
                    string message = SendEmail(new EmailTo
                    {
                        To = request.Email,
                        Subject = "Nhận mã xác nhận để xác nhận đăng ký tài khoản mới từ đây: ",
                        Content = $"Mã kích hoạt của bạn là: {confirmEmail.Code}, mã này có hiệu lực là 24 tiếng"
                    });
                    return _responseObject.ResponseSuccess(message, _userConverter.EntityToDTO(user));
                }
                catch (Exception ex)
                {
                    return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
                }
            }
        }
        #endregion
        #region Làm mới token
        public ResponseObject<TokenDTO> RenewAccessToken(TokenDTO request)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration.GetSection("AppSettings:SecretKey").Value;

                var tokenValidation = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                var tokenAuthentication = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Token không hợp lệ", null);
                }

                var refreshToken = _context.refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

                if (refreshToken == null)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status404NotFound, "RefreshToken không tồn tại trong database", null);
                }

                if (refreshToken.ExpiredTime < DateTime.Now)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status401Unauthorized, "Token đã hết hạn", null);
                }

                var user = _context.users.FirstOrDefault(x => x.Id == refreshToken.UserId);

                if (user == null)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }

                var newToken = GenerateAccessToken(user);

                return _responseObjectToken.ResponseSuccess("Làm mới token thành công", newToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Lỗi xác thực token: " + ex.Message, null);
            }
            catch (Exception ex)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status500InternalServerError, "Lỗi không xác định: " + ex.Message, null);
            }
        }
        #endregion
        #region Phương thức hỗ trợ gửi mail
        public string SendEmail(EmailTo emailTo)
        {
            if (!Validate.IsValidEmail(emailTo.To))
            {
                return "Định dạng email không hợp lệ";
            }
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("minhquantb00@gmail.com", "jvztzxbtyugsiaea"),
                EnableSsl = true
            };
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("minhquantb00@gmail.com");
                message.To.Add(emailTo.To);
                message.Subject = emailTo.Subject;
                message.Body = emailTo.Content;
                message.IsBodyHtml = true;
                smtpClient.Send(message);

                return "Xác nhận gửi email thành công, lấy mã để xác thực";
            }
            catch (Exception ex)
            {
                return "Lỗi khi gửi email: " + ex.Message;
            }
        }
        #endregion
        #region Xác nhận kích hoạt tài khoản mới sau khi người dùng đã nhập đúng mã xác nhận
        public async Task<ResponseObject<UserDTO>> ConfirmCreateAccount(Request_ConfirmCreateAccount request)
        {
            ConfirmEmail confirmEmail = _context.confirmEmails.Where(x => x.Code.Equals(request.ConfirmCode)).SingleOrDefault();
            if(confirmEmail == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Xác nhận đăng ký tài khoản thất bại", null);
            }
            if(confirmEmail.ExpiredDateTime < DateTime.Now)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            User user = await _context.users.FirstOrDefaultAsync(x => x.Id == confirmEmail.UserId);
            //user.StatusId = 2;
            _context.confirmEmails.Remove(confirmEmail);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            
            Notification notification = new Notification
            {
                Title = "Thông báo kích hoạt tài khoản",
                Content = "Tài khoản của bạn đã được kích hoạt, vui lòng đăng nhập để trải nghiệm dịch vụ của chúng tôi",
                CreateId = user.Id,
                CreateTime = DateTime.Now,
                StatusId = 1
            };
            await _context.notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess(notification.Title + ": " + notification.Content, _userConverter.EntityToDTO(user));
        }
        #endregion
        #region Thay đổi quyền tài khoản cho admin và manager
        public async Task<ResponseObject<UserDTO>> ChangeDecentralizationForAdmin(Request_ChangeDecentralization request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == request.UserId);
            if(user is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy id người dùng", null);
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return _responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng không được xác thực hoặc không được xác định", null);
            }

            if (!currentUser.IsInRole("Admin"))
            {
                return _responseObject.ResponseError(StatusCodes.Status403Forbidden, "Người dùng không có quyền sử dụng chức năng này", null);
            }
            user.RoleId = request.RoleId;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thay đổi quyền người dùng thành công", _userConverter.EntityToDTO(user));

        }
        public async Task<ResponseObject<UserDTO>> ChangeDecentralizationForManager(Request_ChangeDecentralization request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy id người dùng", null);
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return _responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng không được xác thực hoặc không được xác định", null);
            }

            if (!currentUser.IsInRole("Manager"))
            {
                return _responseObject.ResponseError(StatusCodes.Status403Forbidden, "Người dùng không có quyền sử dụng chức năng này", null);
            }
            if(request.RoleId == 1 || request.RoleId == 2)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Bạn không có quyền chọn quyền admin hay mod", null);
            }
            user.RoleId = request.RoleId;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thay đổi quyền người dùng thành công", _userConverter.EntityToDTO(user));
        }
        #endregion
    }
}
