using CMS_ActionLayer.DAO;
using CMS_Design.Entities;
using CMS_Design.Handler.HandleImage;
using CMS_Design.IService;
using CMS_Design.Payloads.Converters;
using CMS_Design.Payloads.DTOs.DataProductInformation;
using CMS_Design.Payloads.DTOs.DataResponseGeocoding;
using CMS_Design.Payloads.DTOs.DataResponseListProductSoldAndPrice;
using CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany;
using CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany.StatisticsByMonth;
using CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany.StatisticsByWeek;
using CMS_Design.Payloads.DTOs.DataResponseProduct;
using CMS_Design.Payloads.DTOs.DataResponseProductSoldStaffCommission;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndNumberOfPassengers;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByMonth;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByWeek;
using CMS_Design.Payloads.DTOs.SalesRatio;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByDay;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByMonth;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByWeek;
using CMS_Design.Payloads.Requests.ProductRequests;
using CMS_Design.Payloads.Responses;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CMS_ActionLayer.Service
{
    public class ProductService : BaseService, IProductService
    {
        private readonly ProductConverter _productConverter;
        private readonly ResponseObject<SalesStatisticsDTO> _responseObject;
        private readonly ResponseObject<ProductDTO> _responObjectProduct;
        private readonly PhieuXemNhaConverter _phieuXemNhaConverter;
        private readonly ProductImgConverter _productImgConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public static string apiKey = "AIzaSyDWCoeuHmZroAazHpXfo_pEt2FAB4YF2Ss";
        public ProductService(ProductConverter productConverter, ResponseObject<SalesStatisticsDTO> responseObject, ResponseObject<ProductDTO> responObjectProduct, PhieuXemNhaConverter phieuXemNhaConverter, ProductImgConverter productImgConverter, IHttpContextAccessor httpContextAccessor)
        {
            _productConverter = productConverter;
            _responseObject = responseObject;
            _responObjectProduct = responObjectProduct;
            _phieuXemNhaConverter = phieuXemNhaConverter;
            _productImgConverter = productImgConverter;
            _httpContextAccessor = httpContextAccessor;
        }
        #region Xem thống kê bất động sản của các đầu chủ phòng ban

        public async Task<IQueryable<ProductStatisticsDTO>> GetStatisticalsProductOfManager(int pageSize, int pageNumber)
        {
            var productQuery = await _context.products.Where(x => x.DauChu.RoleId == 3)
                                                    .Include(x => x.ProductImgs)
                                                    .Include(x => x.DauChu)
                                                    .Include(x => x.PhieuXemNhas).AsNoTracking()
                                                    .ToListAsync();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager") && !currentUser.IsInRole("Mod") && !currentUser.IsInRole("Owner"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            var productStatistics = new List<ProductStatisticsDTO>();
            foreach (var product in productQuery)
            {
                var owner = product.DauChu;
                int numOfProduct = productQuery.Count(x => x.DauChuId == owner.Id && x.StatusId == 1 && x.Id == product.Id);
                var productStatisticsItem = new ProductStatisticsDTO
                {
                    OwnerId = owner.Id,
                    NumberOfProduct = numOfProduct,
                    ProductDTOs = productQuery.Where(x => x.Id == product.Id).Select(x => _productConverter.EntityToDTO(x)).AsQueryable()
                };
                productStatistics.Add(productStatisticsItem);
            }

            var groupStatistics = productStatistics.GroupBy(x => x.OwnerId).Select(x => new ProductStatisticsDTO
            {
                NumberOfProduct = x.Sum(y => y.NumberOfProduct),
                OwnerId = x.Key,
                ProductDTOs = x.SelectMany(p => p.ProductDTOs).AsQueryable()
            }).AsQueryable().AsNoTracking();

            return groupStatistics.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        #endregion
        #region Doanh số và lợi nhuận của công ty theo tuần và tháng
        public async Task<IQueryable<SalesAndProfitOfCompanyByMonth>> SalesStatisticsOfCompanyDuringMonth()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var monthlyTeamStatistics = new List<SalesAndProfitOfCompanyByMonth>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }

            foreach (var product in productQuery)
            {
                var user = product.DauChu;

                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);

                if (isSold)
                {
                    double productPrice = product.GiaBan;
                    double commission = productPrice * product.PhanTramChiaNV;

                    int monthNumber = product.BatDauBan.Month;

                    var monthlyTeamStat = monthlyTeamStatistics.FirstOrDefault(mts => mts.MonthNumber == monthNumber);

                    if (monthlyTeamStat == null)
                    {
                        monthlyTeamStat = new SalesAndProfitOfCompanyByMonth
                        {
                            MonthNumber = monthNumber,
                            Sales = productPrice,
                            Profit = productPrice - commission,
                        };

                        monthlyTeamStatistics.Add(monthlyTeamStat);
                    }
                    else
                    {
                        monthlyTeamStat.Sales += productPrice;
                        monthlyTeamStat.Profit += productPrice - commission;
                    }
                }
            }

            return monthlyTeamStatistics.AsQueryable();
        }

        public async Task<IQueryable<SalesAndProfitOfCompanyByWeek>> SalesStatisticsOfCompanyDuringWeek() // doanh số và lợi nhuận của công ty theo tuần nhé
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var weeklyStatistics = new List<SalesAndProfitOfCompanyByWeek>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            foreach (var product in productQuery)
            {
                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);

                if (isSold)
                {
                    int weekNumber = ConvertWeekOfYear(product.BatDauBan);
                    var weeklyStat = weeklyStatistics.FirstOrDefault(w => w.WeekNumber == weekNumber);

                    if (weeklyStat == null)
                    {
                        weeklyStat = new SalesAndProfitOfCompanyByWeek
                        {
                            WeekNumber = weekNumber,
                            Sales = product.GiaBan,
                            Profit = product.GiaBan - (product.GiaBan * product.PhanTramChiaNV)
                        };

                        weeklyStatistics.Add(weeklyStat);
                    }
                    else
                    {
                        weeklyStat.Sales += product.GiaBan;
                        weeklyStat.Profit += product.GiaBan - (product.GiaBan * product.PhanTramChiaNV);
                    }
                }
            }

            return weeklyStatistics.AsQueryable();
        }
        #endregion
        #region Convert sang tuần dựa trên kiểu dữ liệu datetime
        public static int ConvertWeekOfYear(DateTime date)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }
        #endregion
        #region Lấy ra doanh số và lợi nhuận của cả team
        public async Task<IQueryable<SalesStatisticsOfTeamDTO>> SalesStatisticsOfTeam()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var teamStatistics = new List<SalesStatisticsOfTeamDTO>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Mod") && !currentUser.IsInRole("Manager"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            foreach (var product in productQuery)
            {
                var user = product.DauChu;

                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);

                if (isSold)
                {
                    double productPrice = product.GiaBan;
                    double commission = productPrice * product.PhanTramChiaNV;

                    var team = _context.teams.FirstOrDefault(t => t.Users.Any(u => u.Id == user.Id));

                    var teamStat = teamStatistics.FirstOrDefault(ts => ts.TeamId == team.Id);

                    if (teamStat == null)
                    {
                        teamStat = new SalesStatisticsOfTeamDTO
                        {
                            TeamId = team.Id,
                            Sales = productPrice,
                            Profit = productPrice - commission,
                        };

                        teamStatistics.Add(teamStat);
                    }
                    else
                    {
                        teamStat.Sales += productPrice;
                        teamStat.Profit += productPrice - commission;
                    }
                }
            }

            return teamStatistics.AsQueryable();
        }
        #endregion
        #region Doanh số của team theo tuần và tháng
        public async Task<IQueryable<SalesAndProfitOfTeamByWeek>> SalesStatisticsOfTeamDuringWeek()
        {
            var productQuery = await _context.products.OrderByDescending(x => x.BatDauBan)
                                    .Include(x => x.DauChu)
                                    .Include(x => x.PhieuXemNhas)
                                    .ToListAsync();
            var weeklyTeamStatistics = new List<SalesAndProfitOfTeamByWeek>();
            foreach (var product in productQuery)
            {
                var user = product.DauChu;
                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);
                if (isSold)
                {
                    double productPrice = product.GiaBan;
                    double commission = productPrice * product.PhanTramChiaNV;
                    int weekNumber = ConvertWeekOfYear(product.BatDauBan);
                    var team = _context.teams.FirstOrDefault(x => x.Users.Any(y => y.Id == user.Id));
                    var weeklyTeamStat = weeklyTeamStatistics.FirstOrDefault(x => x.TeamId == team.Id && x.SatisticsAndWeekNumber.WeekNumber == weekNumber);
                    if (weeklyTeamStat is null)
                    {
                        weeklyTeamStat = new SalesAndProfitOfTeamByWeek
                        {
                            TeamId = team.Id,
                            SatisticsAndWeekNumber = new StatisticsAndWeekNumber
                            {
                                WeekNumber = weekNumber,
                                Profit = productPrice - commission,
                                Sales = productPrice
                            }
                        };
                        weeklyTeamStatistics.Add(weeklyTeamStat);
                    }
                    else
                    {
                        weeklyTeamStat.SatisticsAndWeekNumber = new StatisticsAndWeekNumber
                        {
                            WeekNumber = weekNumber,
                            Sales = productPrice,
                            Profit = productPrice - commission
                        };
                    }
                }
            }
            return weeklyTeamStatistics.AsQueryable();
        }

        public async Task<IQueryable<SalesAndProfitOfTeamByMonth>> SalesStatisticsOfTeamDuringMonth()
        {
            var productQuery = await _context.products.OrderByDescending(x => x.BatDauBan)
                                    .Include(x => x.DauChu)
                                    .Include(x => x.PhieuXemNhas)
                                    .ToListAsync();
            var monthTeamStatistics = new List<SalesAndProfitOfTeamByMonth>();
            foreach (var product in productQuery)
            {
                var user = product.DauChu;
                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);
                if (isSold)
                {
                    double productPrice = product.GiaBan;
                    double commission = productPrice * product.PhanTramChiaNV;
                    int monthNumber = product.BatDauBan.Month;
                    var team = _context.teams.FirstOrDefault(t => t.Users.Any(u => u.Id == user.Id));
                    var monthlyTeamStat = monthTeamStatistics.FirstOrDefault(mts => mts.TeamId == team.Id && mts.StatisticsAndMonthNumber.MonthNumber == monthNumber);
                    if (monthlyTeamStat is null)
                    {
                        monthlyTeamStat = new SalesAndProfitOfTeamByMonth
                        {
                            TeamId = team.Id,
                            StatisticsAndMonthNumber = new StatisticsAndMonthNumber
                            {
                                MonthNumber = monthNumber,
                                Profit = productPrice - commission,
                                Sales = productPrice
                            }
                        };
                    }
                    else
                    {
                        monthlyTeamStat.StatisticsAndMonthNumber = new StatisticsAndMonthNumber
                        {
                            MonthNumber = monthNumber,
                            Sales = productPrice,
                            Profit = productPrice - commission
                        };
                    }
                }
            }
            return monthTeamStatistics.AsQueryable();
        }
        #endregion
        #region Doanh số và số lượt số lượt dẫn khách
        public async Task<IQueryable<SalesAndNumberOfPassengersByDayDTO>> ViewSalesStatisticsAndNumberOfPassengersByDay()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var dailyViewSalesStatistics = new List<SalesAndNumberOfPassengersByDayDTO>();

            foreach (var product in productQuery)
            {
                var user = product.DauChu;
                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);

                if (isSold)
                {
                    double productPrice = product.GiaBan;
                    int dayNumber = product.BatDauBan.Day;
                    int numberOfPassengers = product.PhieuXemNhas.Count();

                    var team = _context.teams.FirstOrDefault(t => t.Users.Any(u => u.Id == user.Id));
                    var dayTeamStat = dailyViewSalesStatistics.FirstOrDefault(mts => mts.DayNumber == dayNumber);

                    if (dayTeamStat == null)
                    {
                        dayTeamStat = new SalesAndNumberOfPassengersByDayDTO
                        {
                            DayNumber = dayNumber,
                            EmployeeId = user.Id,
                            NumberOfPassengers = numberOfPassengers,
                            TeamId = team.Id,
                            Sales = productPrice
                        };
                        dailyViewSalesStatistics.Add(dayTeamStat);
                    }
                    else
                    {
                        dayTeamStat.DayNumber = dayNumber;
                        dayTeamStat.NumberOfPassengers = numberOfPassengers;
                        dayTeamStat.TeamId = team.Id;
                        dayTeamStat.EmployeeId = user.Id;
                        dayTeamStat.Sales = productPrice;
                    }
                }
            }

            return dailyViewSalesStatistics.AsQueryable();
        }


        public async Task<IQueryable<SalesAndNumberOfPassengersByMonthDTO>> ViewSalesStatisticsAndNumberOfPassengersByMonth()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var monthlyViewSalesStatistics = new List<SalesAndNumberOfPassengersByMonthDTO>();

            foreach (var product in productQuery)
            {
                var user = product.DauChu;
                bool isSold = product.StatusId == 1 || product.PhieuXemNhas.Any(x => x.BanThanhCong == true);

                if (isSold)
                {
                    double productPrice = product.GiaBan;
                    int monthNumber = product.BatDauBan.Month;
                    int numberOfPassengers = product.PhieuXemNhas.Count();

                    var team = _context.teams.FirstOrDefault(t => t.Users.Any(u => u.Id == user.Id));
                    var monthTeamStat = monthlyViewSalesStatistics.FirstOrDefault(mts => mts.MonthNumber == monthNumber);

                    if (monthTeamStat == null)
                    {
                        monthTeamStat = new SalesAndNumberOfPassengersByMonthDTO
                        {
                            MonthNumber = monthNumber,
                            EmployeeId = user.Id,
                            NumberOfPassengers = numberOfPassengers,
                            TeamId = team.Id,
                            Sales = productPrice
                        };
                        monthlyViewSalesStatistics.Add(monthTeamStat);
                    }
                    else
                    {
                        monthTeamStat.MonthNumber = monthNumber;
                        monthTeamStat.NumberOfPassengers = numberOfPassengers;
                        monthTeamStat.TeamId = team.Id;
                        monthTeamStat.EmployeeId = user.Id;
                        monthTeamStat.Sales = productPrice;
                    }
                }
            }

            return monthlyViewSalesStatistics.AsQueryable();
        }

        #endregion
        #region Tỉ lệ doanh số và lợi nhuận giữa team và công ty theo tuần
        public async Task<IQueryable<SalesAndProfitOfCompanyAndTeam>> SalesRatioBetweenTeamAndCompany()
        {
            var salesAndProfitOfCompany = await SalesStatisticsOfCompanyDuringWeek();
            var salesAndProfitOfTeam = await SalesStatisticsOfTeamDuringWeek();

            var salesAndProfitOfCompanyAndTeam = new List<SalesAndProfitOfCompanyAndTeam>();

            foreach (var salesOfCompany in salesAndProfitOfCompany)
            {
                var matchingSalesOfTeam = salesAndProfitOfTeam.FirstOrDefault(s => s.SatisticsAndWeekNumber.WeekNumber == salesOfCompany.WeekNumber);
                if (matchingSalesOfTeam != null)
                {
                    var item = new SalesAndProfitOfCompanyAndTeam
                    {
                        ProfitOfCompany = salesOfCompany.Profit,
                        SalesOfCompany = salesOfCompany.Sales,
                        StatisticsOfTeam = new StatisticsOfTeam
                        {
                            TeamId = matchingSalesOfTeam.TeamId,
                            Profit = matchingSalesOfTeam.SatisticsAndWeekNumber.Profit,
                            WeekNumber = matchingSalesOfTeam.SatisticsAndWeekNumber.WeekNumber,
                            ProfitRatio = (matchingSalesOfTeam.SatisticsAndWeekNumber.Profit / salesOfCompany.Profit) * 100,
                            SalesRatio = (matchingSalesOfTeam.SatisticsAndWeekNumber.Sales / salesOfCompany.Sales) * 100,
                            Sales = matchingSalesOfTeam.SatisticsAndWeekNumber.Sales
                        }
                    };
                    salesAndProfitOfCompanyAndTeam.Add(item);
                }
            }

            return salesAndProfitOfCompanyAndTeam.AsQueryable();
        }
        #endregion
        #region Thêm và sửa thông tin sản phẩm
        public async Task<ResponseObject<ProductDTO>> AddNewProduct(int dauChuId, Request_AddNewProduct request)
        {
            try
            {
                var dauChu = await _context.users.FirstOrDefaultAsync(x => x.Id == dauChuId);
                var currentUser = _httpContextAccessor.HttpContext.User;

                //if (!currentUser.Identity.IsAuthenticated)
                //{
                //    throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
                //}

                //if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Owner") && !currentUser.IsInRole("Manager"))
                //{
                //    throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
                //}
                var createProduct = new Product
                {
                    BatDauBan = request.BatDauBan,
                    Build = request.Build,
                    CertificateOfLand1 = request.CertificateOfLand1,
                    CertificateOfLand2 = request.CertificateOfLand2,
                    DauChuId = dauChu.Id,
                    GiaBan = request.GiaBan,
                    Address = request.Address,
                    HoaHong = request.GiaBan - (request.GiaBan * request.PhanTramChiaNV),
                    PhanTramChiaNV = request.PhanTramChiaNV,
                    HostName = request.HostName,
                    HostPhoneNumber = request.HostPhoneNumber,
                    StatusId = 2
                };
                await _context.products.AddAsync(createProduct);
                await _context.SaveChangesAsync();
                return _responObjectProduct.ResponseSuccess("Thêm sản phẩm thành công", _productConverter.EntityToDTO(createProduct));
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<ProductImg>> AddProductImg(int productId, List<Request_CreateProductImg> requests)
        {
            var product = await _context.products.SingleOrDefaultAsync(x => x.Id == productId);
            if (product == null)
            {
                return null;
            }

            int imageSize = 2 * 1024 * 768;
            var list = new List<ProductImg>();

            foreach (var img in requests)
            {
                if (img.LinkImg != null)
                {
                    if (!HandleCheckImage.IsImage(img.LinkImg, imageSize))
                    {
                        throw new Exception("Ảnh không hợp lệ");
                    }

                    var imageProduct = await HandleUploadImage.Upfile(img.LinkImg);
                    var linkImg = string.IsNullOrWhiteSpace(imageProduct)
                        ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o="
                        : imageProduct;

                    var productImg = new ProductImg
                    {
                        ProductId = productId,
                        LinkImg = linkImg
                    };

                    list.Add(productImg);
                }
            }

            _context.productsImg.AddRange(list);
            await _context.SaveChangesAsync();
            return list;
        }


        public async Task<ResponseObject<ProductDTO>> UpdateProduct(int productId, int dauChuId, Request_UpdateProduct request)
        {
            var product = await _context.products.FirstOrDefaultAsync(x => x.Id == productId);
            var dauChu = await _context.users.FirstOrDefaultAsync(x => x.Id == dauChuId);
            if (product == null)
            {
                return _responObjectProduct.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy sản phẩm", null);
            }
            else
            {
                product.BatDauBan = request.BatDauBan;
                product.Build = request.Build;
                product.GiaBan = request.GiaBan;
                product.PhanTramChiaNV = request.PhanTramChiaNV;
                product.HostName = request.HostName;
                product.HostPhoneNumber = request.HostPhoneNumber;
                product.HoaHong = request.HoaHong;
                product.Address = request.Address;
                product.CertificateOfLand1 = request.CertificateOfLand1;
                product.CertificateOfLand2 = request.CertificateOfLand2;
                product.DauChu = dauChu;
                _context.products.Update(product);
                await _context.SaveChangesAsync();
                return _responObjectProduct.ResponseSuccess("Cập nhật thông tin sản phẩm thành công", _productConverter.EntityToDTO(product));
            }
        }
        public async Task<ResponseObject<ProductDTO>> AddProductImage(int productId, Request_CreateProductImg request)
        {
            var product = await _context.products.FirstOrDefaultAsync(x => x.Id == productId);

            if (product is null)
            {
                return _responObjectProduct.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy sản phẩm", null);
            }

            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return _responObjectProduct.ResponseError(StatusCodes.Status401Unauthorized, "Người dùng không được xác thực hoặc không được xác định", null);
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Owner"))
            {
                return _responObjectProduct.ResponseError(StatusCodes.Status403Forbidden, "Người dùng không có quyền sử dụng chức năng này", null);
            }

            var productImage = new ProductImg
            {
                ProductId = product.Id,
                LinkImg = await HandleUploadImage.Upfile(request.LinkImg)
            };

            await _context.productsImg.AddAsync(productImage);
            _context.products.Update(product);
            await _context.SaveChangesAsync();

            return _responObjectProduct.ResponseSuccess("Thêm ảnh cho sản phẩm thành công", _productConverter.EntityToDTO(product));
        }

        #endregion
        #region Xem thống kê bất động sản - giá bán - địa chỉ - hoa hồng - lịch sử dẫn khách
        public async Task<IQueryable<StatisticsProductInformation>> GetStatisticsProductInformation()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var statisticsProductInformation = productQuery.Select(product =>
            {
                var address = product.PhieuXemNhas.Where(x => !x.BanThanhCong && x.NhaId == product.Id).Select(x => _phieuXemNhaConverter.EntityToDTO(x)).ToList();
                var user = product.DauChu;
                var commission = product.GiaBan;

                return new StatisticsProductInformation
                {
                    Address = product.Address,
                    Commission = product.GiaBan - (product.GiaBan * product.PhanTramChiaNV),
                    ProductId = product.Id,
                    Price = product.GiaBan,
                    PhieuXemNhas = address.AsQueryable()
                };
            }).AsQueryable();

            return statisticsProductInformation;
        }

        #endregion
        #region Tìm kiếm bất động sản
        public async Task<IQueryable<ProductDTO>> GetProductById(int productId, int pageSize, int pageNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new NotImplementedException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Owner") && !currentUser.IsInRole("Mod"))
            {
                throw new NotImplementedException("Người dùng không có quyền sử dụng chức năng này");
            }
            var productQuery = _context.products.Where(x => x.Id == productId).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _productConverter.EntityToDTO(x));
            return productQuery;
        }
        private string ChuanHoaChuoi(string str)
        {
            str = str.ToLower().Trim();
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }
        public async Task<IQueryable<ProductDTO>> GetAllProducts(int pageSize, int pageNumber)
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();

            var productDTOs = productQuery.Select(x => _productConverter.EntityToDTO(x)).ToList();

            foreach (var product in productQuery)
            {
                bool isSold = product.StatusId == 2 || (product.PhieuXemNhas != null && product.PhieuXemNhas.Any(x => !x.BanThanhCong));

                if (isSold)
                {
                    var productImgs = product.ProductImgs?.Where(x => x.ProductId == product.Id).ToList();

                    if (productImgs != null)
                    {
                        var listProductImgDTO = productImgs.Select(x => _productImgConverter.EntityToDTO(x)).ToList();

                        foreach (var productDTO in productDTOs)
                        {
                            productDTO.ProductImgDTOs = listProductImgDTO.AsQueryable();
                        }
                    }
                }
            }

            return productDTOs.AsQueryable();
        }

        public async Task<IQueryable<ProductDTO>> GetProductByName(string name, int pageSize, int pageNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new NotImplementedException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Owner") && !currentUser.IsInRole("Mod"))
            {
                throw new NotImplementedException("Người dùng không có quyền sử dụng chức năng này");
            }
            var productQuery = _context.products.Where(x => ChuanHoaChuoi(x.HostName).Contains(ChuanHoaChuoi(name))).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _productConverter.EntityToDTO(x));
            return productQuery;
        }
        public async Task<IQueryable<ProductDTO>> GetProductByAddress(string address, int pageSize, int pageNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new NotImplementedException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Owner") && !currentUser.IsInRole("Mod"))
            {
                throw new NotImplementedException("Người dùng không có quyền sử dụng chức năng này");
            }
            var productQuery = _context.products.Where(x => ChuanHoaChuoi(x.Address).Contains(ChuanHoaChuoi(address))).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _productConverter.EntityToDTO(x));
            return productQuery;
        }
        public async Task<IQueryable<ProductDTO>> GetProductByOwner(int ownerId, int pageSize, int pageNumber)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new NotImplementedException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Owner") && !currentUser.IsInRole("Mod"))
            {
                throw new NotImplementedException("Người dùng không có quyền sử dụng chức năng này");
            }
            var productQuery = _context.products.Where(x => x.DauChuId == ownerId).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _productConverter.EntityToDTO(x));
            return productQuery;
        }
        #endregion
        #region Thống kê về các bất động sản đã bán, chuyên viên chốt giá, giá, hoa hồng
        public async Task<IQueryable<StatisticsAboutProductSoldStaffCommission>> StatisticsProductOfOwnerAndOrtherInfomation()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas).Include(x => x.ProductImgs).AsNoTracking()
                .ToListAsync();

            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }

            var list = new List<StatisticsAboutProductSoldStaffCommission>();

            foreach (var product in productQuery)
            {
                if (product.DauChu != null && _productImgConverter != null)
                {
                    var statisticsItem = new StatisticsAboutProductSoldStaffCommission
                    {
                        Price = product.GiaBan,
                        Commission = product.GiaBan * product.PhanTramChiaNV,
                        OwnerId = product.DauChu.Id
                    };

                    var productImgs = product.ProductImgs.Where(x => x.ProductId == product.Id).ToList();
                    var productDTOs = productQuery.Select(x => _productConverter.EntityToDTO(x)).ToList();

                    foreach (var productImg in productImgs)
                    {
                        var productImgDTO = _productImgConverter.EntityToDTO(productImg);
                        if (productImgDTO != null)
                        {
                            var listProductImgDTO = product.ProductImgs.Select(x => _productImgConverter.EntityToDTO(x)).ToList();
                            productDTOs.ForEach(x =>
                            {
                                x.ProductImgDTOs = listProductImgDTO.AsQueryable();
                            });
                        }
                    }

                    statisticsItem.ProductDTOs = productDTOs.AsQueryable();
                    list.Add(statisticsItem);
                }
            }

            return list.AsQueryable();
        }


        #endregion
        #region Thống kê số lượng bán theo ngày - tuần - tháng 
        public async Task<IQueryable<StatisticsSalesByDay>> GetStatisticsSalesByDay()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();
            var statisticsSalesByDay = new List<StatisticsSalesByDay>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            foreach (var product in productQuery)
            {
                int sellNumber = product.PhieuXemNhas.Count(x => x.BanThanhCong);
                int dayNumber = product.BatDauBan.Day;
                var dayValue = statisticsSalesByDay.FirstOrDefault(x => x.DayNumber == dayNumber);
                if (dayValue == null)
                {
                    dayValue = new StatisticsSalesByDay
                    {
                        DayNumber = dayNumber,
                        SellNumber = sellNumber
                    };
                    statisticsSalesByDay.Add(dayValue);
                }
                else
                {
                    dayValue.DayNumber = dayNumber;
                    dayValue.SellNumber = sellNumber;
                }
            }
            return statisticsSalesByDay.AsQueryable();
        }
        public async Task<IQueryable<StatisticsSalesByWeek>> GetStatisticsSalesByWeek()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();
            var statisticsSalesByWeek = new List<StatisticsSalesByWeek>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            foreach (var product in productQuery)
            {
                int sellNumber = product.PhieuXemNhas.Count(x => x.BanThanhCong);
                int weekNumber = ConvertWeekOfYear(product.BatDauBan);
                var statisticsByWeek = statisticsSalesByWeek.FirstOrDefault(x => x.WeekNumber == weekNumber);
                if (statisticsByWeek == null)
                {
                    statisticsByWeek = new StatisticsSalesByWeek
                    {
                        WeekNumber = weekNumber,
                        SellNumber = sellNumber
                    };
                    statisticsSalesByWeek.Add(statisticsByWeek);
                }
                else
                {
                    statisticsByWeek.WeekNumber = weekNumber;
                    statisticsByWeek.SellNumber = sellNumber;
                }
            }
            return statisticsSalesByWeek.AsQueryable();
        }
        public async Task<IQueryable<StatisticsSalesByMonth>> GetStatisticsSalesByMonth()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();
            var statisticsSalesByMonth = new List<StatisticsSalesByMonth>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            foreach (var product in productQuery)
            {
                int sellNumber = product.PhieuXemNhas.Count(x => x.BanThanhCong);
                int monthNumber = product.BatDauBan.Month;
                var monthValue = statisticsSalesByMonth.FirstOrDefault(x => x.MonthNumber == monthNumber);
                if (monthValue == null)
                {
                    monthValue = new StatisticsSalesByMonth
                    {
                        MonthNumber = monthNumber,
                        SellNumber = sellNumber
                    };
                    statisticsSalesByMonth.Add(monthValue);
                }
                else
                {
                    monthValue.MonthNumber = monthNumber;
                    monthValue.SellNumber = sellNumber;
                }
            }
            return statisticsSalesByMonth.AsQueryable();
        }
        #endregion
        #region Xem danh sách nhà đang chờ bán - giá - hoa hồng (Google Map API)
        public async Task<IQueryable<ProductNotYetSoldAndPrice>> GetProductSoldAndPrices()
        {
            var productQuery = await _context.products
                .OrderByDescending(x => x.BatDauBan)
                .Include(x => x.DauChu)
                .Include(x => x.PhieuXemNhas)
                .ToListAsync();
            var productSoldAndPrice = new List<ProductNotYetSoldAndPrice>();
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("Manager") && !currentUser.IsInRole("Mod"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            foreach (var product in productQuery)
            {
                bool isSold = product.StatusId == 2 || product.PhieuXemNhas.Any(x => !x.BanThanhCong);
                if (isSold)
                {
                    double price = product.GiaBan;
                    double commission = product.GiaBan * product.PhanTramChiaNV;
                    var productNotYetSold = new ProductNotYetSoldAndPrice
                    {
                        Price = price,
                        Commission = commission
                    };
                    var productImgs = product.ProductImgs.Where(x => x.ProductId == product.Id).ToList();
                    var productDTOs = productQuery.Select(x => _productConverter.EntityToDTO(x)).ToList();

                    foreach (var productImg in productImgs)
                    {
                        var productImgDTO = _productImgConverter.EntityToDTO(productImg);
                        if (productImgDTO != null)
                        {
                            var listProductImgDTO = product.ProductImgs.Select(x => _productImgConverter.EntityToDTO(x)).ToList();
                            productDTOs.ForEach(x =>
                            {
                                x.ProductImgDTOs = listProductImgDTO.AsQueryable();
                            });
                        }
                    }
                    productNotYetSold.ProductDTOs = productDTOs.AsQueryable();
                    productSoldAndPrice.Add(productNotYetSold);
                }
            }
            return productSoldAndPrice.AsQueryable();
        }
        public async Task<GeocodingResult> GetCoordinatesAsync(string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace("AIzaSyDWCoeuHmZroAazHpXfo_pEt2FAB4YF2Ss"))
                {
                    throw new ArgumentException("API Key is required.");
                }

                var apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key=AIzaSyDWCoeuHmZroAazHpXfo_pEt2FAB4YF2Ss";

                using (var httpClient = _httpClientFactory.CreateClient("YourHttpClientName")) // Thay "YourHttpClientName" bằng tên của HttpClient bạn muốn sử dụng
                {
                    var response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var googleMapsResponse = JsonConvert.DeserializeObject<GoogleMapsResponse>(content);

                        if (googleMapsResponse.Results.Count > 0)
                        {
                            var location = googleMapsResponse.Results[0].Geometry.Location;
                            return new GeocodingResult
                            {
                                Latitude = location.Lat,
                                Longitude = location.Lng
                            };
                        }
                        else
                        {
                            throw new InvalidOperationException("Không tìm thấy địa chỉ.");
                        }

                    }
                    else
                    {
                        throw new HttpRequestException($"Lỗi khi gửi yêu cầu lấy tọa độ từ Google Maps: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        #endregion
        #region Tạo yêu cầu thay đổi trạng thái khi bán bất động sản thành công
        public async Task<string> CreateRequestChangeStatusWhenClosingSuccessfully(int productId)
        {
            var product = await _context.products.SingleOrDefaultAsync(x => x.Id == productId);
            if (product == null)
            {
                return "Không tìm thấy sản phẩm mà bạn cần";
            }

            var phieuXemNha = _context.phieuXemNhas.FirstOrDefault(x => x.NhaId == productId && x.BanThanhCong);
            if (phieuXemNha == null)
            {
                return "Bất động sản vẫn chưa được bán";
            }

            List<PhieuXemNha> phieuCanCapNhat = new List<PhieuXemNha>(); // Danh sách tạm thời

            foreach (var phieu in product.PhieuXemNhas)
            {
                if (phieu == null)
                {
                    // Thêm kiểm tra cho trường hợp phiếu null
                    continue;
                }

                if (product.StatusId == 1)
                {
                    break;
                }

                if (phieu.Id == phieuXemNha.Id)
                {
                    product.StatusId = 1;
                    phieu.BanThanhCong = true;
                    phieuCanCapNhat.Add(phieu);
                }
            }

            foreach (var phieuItem in _context.phieuXemNhas.ToList())
            {
                if (phieuItem.NhaId == phieuXemNha.NhaId && !phieuCanCapNhat.Contains(phieuItem))
                {
                    phieuItem.BanThanhCong = false;
                }
            }

            _context.products.Update(product);
            await _context.SaveChangesAsync();

            return "Thay đổi status thành công";
        }



        #endregion
    }
}
