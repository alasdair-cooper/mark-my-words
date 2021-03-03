using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using System.IO;
using System.IO.Compression;

using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public AssignmentController(IWebHostEnvironment webHostEnvironment)
        {
            hostEnvironment = webHostEnvironment;
        }

        // GET: api/<AssignmentController>
        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string AssignmentId)
        {
            string filePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.json");
            string compressedFilePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.gz");

            using (FileStream originalFileStream = System.IO.File.OpenRead(compressedFilePath))
            {
                using (FileStream decompressedFileStream = System.IO.File.Create(filePath))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }

            using FileStream readStream = System.IO.File.OpenRead(filePath);
            await readStream.CopyToAsync(HttpContext.Response.Body);

            return Ok();
        }

        // POST api/<AssignmentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string AssignmentId)
        {
            string filePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.json");
            string compressedFilePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.gz");

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            using (FileStream writeStream = System.IO.File.OpenWrite(filePath))
            {
                await HttpContext.Request.Body.CopyToAsync(writeStream);
            }
            using FileStream originalFileStream = System.IO.File.OpenRead(filePath);
            using FileStream compressedFileStream = System.IO.File.Create(compressedFilePath);
            using GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressionStream);
            
            return Ok();
        }

        // DELETE api/<AssignmentController>
        [HttpDelete]
        public IActionResult Delete([FromHeader] string AssignmentID)
        {
            System.IO.File.Delete($"./{AssignmentID}.json");
            return Ok();
        }
    }
}
