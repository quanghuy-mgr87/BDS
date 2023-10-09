using CMS_Design.IService;
using CMS_Design.Payloads.DTOs.DataResponsePhieuXemNha;
using CMS_Design.Payloads.Requests.PhieuXemNhaRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [ApiController]
    public class PhieuXemNhaController : ControllerBase
    {
        private readonly IPhieuXemNhaService _phieuXemNhaService;
        public PhieuXemNhaController(IPhieuXemNhaService phieuXemNhaService)
        {
            _phieuXemNhaService = phieuXemNhaService;
        }

        [HttpPost("/api/phieuxemnha/CreatePhieuXemNha")]
        [Authorize(Roles = "Admin, Owner, Manager, Mod, Staff")]
        public async Task<IActionResult> CreatePhieuXemNha(Request_CreatePhieuXemNha request)
        {
            return Ok(await _phieuXemNhaService.CreatePhieuXemNha(request.NhaId, request.NhanVienId, request));
        }
        [HttpPut("/api/phieuxemnha/UpdatePhieuXemNha")]
        [Authorize(Roles = "Admin, Owner, Manager, Mod, Staff")]
        public async Task<IActionResult> UpdatePhieuXemNha(Request_UpdatePhieuXemNha request)
        {
            try
            {
                return Ok(await _phieuXemNhaService.UpdatePhieuXemNha(request));
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpDelete("/api/phieuxemnha/DeletePhieuXemNha/{phieuXemNhaId}")]
        [Authorize(Roles = "Admin, Owner, Manager, Mod, Staff")]
        public async Task<IActionResult> DeletePhieuXemNha(int phieuXemNhaId)
        {
            return Ok(await _phieuXemNhaService.DeletePhieuXemNha(phieuXemNhaId));
        }
        [HttpGet("/api/phieuxemnha/GetPhieuXemNhaByBanThanhCong")]
        [Authorize(Roles = "Admin, Owner, Manager")]
        public async Task<IActionResult> GetPhieuXemNhaByBanThanhCong(bool banThanhCong, int pageSize, int pageNumber)
        {
            return Ok(await _phieuXemNhaService.GetPhieuXemNhaByBanThanhCong(banThanhCong, pageSize, pageNumber));
        }
        [HttpGet("/api/phieuxemnha/GetPhieuXemNhaByCustomerName")]
        [Authorize(Roles = "Admin, Owner, Manager")]
        public async Task<IActionResult> GetPhieuXemNhaByCustomerName(string name, int pageSize, int pageNumber)
        {
            return Ok(await _phieuXemNhaService.GetPhieuXemNhaByCustomerName(name, pageSize, pageNumber));
        }
        [HttpGet("/api/phieuxemnha/GetAllPhieuXemNha")]
        [Authorize(Roles = "Admin, Owner, Manager")]
        public async Task<IActionResult> GetAllPhieuXemNha(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                return Ok(await _phieuXemNhaService.GetAllPhieuXemNha(pageSize, pageNumber));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
