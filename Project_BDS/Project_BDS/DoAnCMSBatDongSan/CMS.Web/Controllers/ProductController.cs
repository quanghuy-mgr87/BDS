using CMS_ActionLayer.Service;
using CMS_Design.IService;
using CMS_Design.Payloads.DTOs;
using CMS_Design.Payloads.DTOs.DataProductInformation;
using CMS_Design.Payloads.DTOs.DataResponseListProductSoldAndPrice;
using CMS_Design.Payloads.DTOs.DataResponseProduct;
using CMS_Design.Payloads.DTOs.DataResponseProductSoldStaffCommission;
using CMS_Design.Payloads.DTOs.StatisticsSalesByTime.ByDay;
using CMS_Design.Payloads.Requests.ProductRequests;
using CMS_Design.Payloads.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _iProductService;
        public ProductController(IProductService iProductService)
        {
            _iProductService = iProductService;
        }
        #region Thống kê các sản phẩm của các đầu chủ phòng ban
        [HttpGet("/api/product/get-product-with-manager")]
        [Authorize(Roles = "Admin, Mod, Manager, Owner")]
        public async Task<IActionResult> GetStatisticalsProductOfManager(int pageSize, int pageNumber)
        {
            return Ok(await _iProductService.GetStatisticalsProductOfManager(pageSize, pageNumber));
        }
        #endregion
        #region Thống kê doanh số và lợi nhuận của công ty theo tháng
        [HttpGet("/api/product/SalesStatisticsOfCompany")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> SalesStatisticsOfCompany()
        {
            return Ok(await _iProductService.SalesStatisticsOfCompanyDuringMonth());
        }
        #endregion
        #region Thống kê doanh số và lợi nhuận của công ty theo tuần
        [HttpGet("/api/product/WeeklySalesStatisticsOfCompany")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> WeeklySalesStatisticsOfCompany()
        {
            return Ok(await _iProductService.SalesStatisticsOfCompanyDuringWeek());
        }
        #endregion
        #region Thống kê doanh số của các phòng ban
        [HttpGet("/api/product/SalesStatisticsOfTeam")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> SalesStatisticsOfTeam()
        {
            return Ok(await _iProductService.SalesStatisticsOfTeam());
        }
        #endregion
        #region Thống kê doanh số và lợi nhuận của phòng ban theo tuần
        [HttpGet("/api/product/WeeklySalesStatisticsOfTeam")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> WeeklySalesStatisticsOfTeam()
        {
            return Ok(await _iProductService.SalesStatisticsOfTeamDuringWeek());
        }
        #endregion
        #region Thống kê doanh số và lợi nhuận của phòng ban theo tháng
        [HttpGet("/api/product/MonthlySalesStatisticsOfTeam")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> MonthlySalesStatisticsOfTeam()
        {
            return Ok(await _iProductService.SalesStatisticsOfCompanyDuringMonth());
        }
        #endregion
        #region Thống kê doanh số và lượt dẫn khách trong ngày
        [HttpGet("/api/product/ViewSalesStatisticsAndNumberOfPassengersByDay")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> ViewSalesStatisticsAndNumberOfPassengersByDay()
        {
            return Ok(await _iProductService.ViewSalesStatisticsAndNumberOfPassengersByDay());
        }
        #endregion
        #region Thống kê doanh số và lượt dẫn khách trong tháng
        [HttpGet("/api/product/ViewSalesStatisticsAndNumberOfPassengersByMonth")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> ViewSalesStatisticsAndNumberOfPassengersByMonth()
        {
            return Ok(await _iProductService.ViewSalesStatisticsAndNumberOfPassengersByMonth());
        }
        #endregion
        #region Tỉ lệ doanh số và lợi nhuận của phòng ban và công ty
        [HttpGet("/api/product/SalesRatioBetweenTeamAndCompany")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<IActionResult> SalesRatioBetweenTeamAndCompany()
        {
            return Ok(await _iProductService.SalesRatioBetweenTeamAndCompany());
        }
        #endregion
        #region CRUD Sản phẩm
        [HttpPost("/api/product/AddNewProduct")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin, Manager, Owner")]
        public async Task<IActionResult> AddNewProduct(Request_AddNewProduct request)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                //if (!int.TryParse(HttpContext.User.FindFirst("Id")?.Value, out int id))
                //{
                //    return BadRequest("Id người dùng không hợp lệ");
                //}

                var id = request.DauChuId;

                var result = await _iProductService.AddNewProduct(id, request);

                if (result.Message.ToLower().Contains("Thêm sản phẩm thành công".ToLower()))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("/api/product/UpdateProduct/{productId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateProduct(int productId, Request_UpdateProduct request)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                //if (!int.TryParse(HttpContext.User.FindFirst("Id")?.Value, out int id))
                //{
                //    return BadRequest("Id người dùng không hợp lệ");
                //}

                var id = request.DauChuId;

                var result = await _iProductService.UpdateProduct(productId, id, request);

                if (result.Message.ToLower().Contains("Cập nhật thông tin sản phẩm thành công".ToLower()))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("/api/product/GetStatisticsProductInformation")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> GetStatisticsProductInformation()
        {
            return Ok(await _iProductService.GetStatisticsProductInformation());
        }
        [HttpPut("/api/product/DeleteProduct")]
        [Authorize(Roles = "Admin, Mod, Manager, Owner")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            return Ok(await _iProductService.DeleteProduct(productId));
        }
        [HttpPost("/api/product/AddImageForProduct/{productId}")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> AddImageForProduct(int productId, [FromForm] Request_CreateProductImg request)
        {
            return Ok(await _iProductService.AddProductImage(productId, request));
        }
        [HttpGet("/api/product/GetProductById")]
        [Authorize(Roles = "Admin, Mod, Owner")]
        public async Task<IActionResult> GetProductById(int productId, int pageSize, int pageNumber)
        {
            return Ok(await _iProductService.GetProductById(productId, pageSize, pageNumber));
        }

        [HttpGet("/api/product/GetProductByName")]
        [Authorize(Roles = "Admin, Mod, Owner")]
        public async Task<IActionResult> GetProductByName(string name, int pageSize, int pageNumber)
        {
            return Ok(await _iProductService.GetProductByName(name, pageSize, pageNumber));
        }

        [HttpGet("/api/product/GetProductByAddress")]
        [Authorize(Roles = "Admin, Mod, Owner")]
        public async Task<IActionResult> GetProductByAddress(string address, int pageSize, int pageNumber)
        {
            return Ok(await _iProductService.GetProductByAddress(address, pageSize, pageNumber));
        }

        [HttpGet("/api/product/GetProductByOwner")]
        [Authorize(Roles = "Admin, Mod, Owner")]
        public async Task<IActionResult> GetProductByOwner(int ownerId, int pageSize, int pageNumber)
        {
            return Ok(await _iProductService.GetProductById(ownerId, pageSize, pageNumber));
        }
        #endregion
        #region Thống kê về các bất động sản đã bán, chuyên viên chốt giá, giá, hoa hồng
        [HttpGet("/api/product/StatisticsProductOfOwnerAndOrtherInfomation")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> StatisticsProductOfOwnerAndOrtherInfomation()
        {
            return Ok(await _iProductService.StatisticsProductOfOwnerAndOrtherInfomation());
        }
        #endregion
        #region Thống kê số lượng bán theo ngày - tháng - năm
        [HttpGet("/api/product/GetStatisticsSalesByDay")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> GetStatisticsSalesByDay()
        {
            return Ok(await _iProductService.GetStatisticsSalesByDay());
        }
        [HttpGet("/api/product/GetStatisticsSalesByWeek")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> GetStatisticsSalesByWeek()
        {
            return Ok(await _iProductService.GetStatisticsSalesByWeek());
        }
        [HttpGet("/api/product/GetStatisticsSalesByMonth")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> GetStatisticsSalesByMonth()
        {
            return Ok(await _iProductService.GetStatisticsSalesByMonth());
        }
        #endregion
        #region Lấy sản phẩm chờ bán, giá, hoa hồng
        [HttpGet("/api/product/GetProductSoldAndPrices")]
        [Authorize(Roles = "Admin, Mod, Manager")]
        public async Task<IActionResult> GetProductSoldAndPrices()
        {
            return Ok(await _iProductService.GetProductSoldAndPrices());
        }
        #endregion
        #region Lấy tọa độ google map
        [HttpGet("/api/product/GetCoordinatesAsync")]
        public async Task<IActionResult> GetCoordinatesAsync(string address)
        {
            return Ok(await _iProductService.GetCoordinatesAsync(address));
        }
        #endregion
        #region Tạo yêu cầu thay đổi trạng thái bất động sản khi chốt thành công
        [HttpPut("/api/product/CreateRequestChangeStatusWhenClosingSuccessfully")]
        public async Task<IActionResult> CreateRequestChangeStatusWhenClosingSuccessfully(int productId)
        {
            return Ok(await _iProductService.CreateRequestChangeStatusWhenClosingSuccessfully(productId));
        }
        [HttpGet("/api/product/GetAll")]
        public async Task<IActionResult> GetAllProducts(int pageSize, int pageNumber)
        {
            return Ok(await _iProductService.GetAllProducts(pageSize, pageNumber));
        }
        #endregion
    }
}
