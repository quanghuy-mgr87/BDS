using CMS_ActionLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }
        [HttpGet("/api/document/DownloadDocument/{fileName}")]
        public async Task<IActionResult> DownloadDocument(string fileName)
        {
            var documentBytes = await _documentService.GetDocumentBytesAsync(fileName);

            if (documentBytes == null)
            {
                return NotFound();
            }

            return File(documentBytes, "application/octet-stream", fileName);
        }
    }
}
