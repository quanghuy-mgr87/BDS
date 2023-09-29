using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_ActionLayer.Service
{
    public class DocumentService
    {
        private readonly string documentsPath = "C:\\Users\\Admin\\Downloads";

        public async Task<byte[]> GetDocumentBytesAsync(string fileName)
        {
            var filePath = Path.Combine(documentsPath, fileName);

            if (!File.Exists(filePath))
            {
                return null;
            }
            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
