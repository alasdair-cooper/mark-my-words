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

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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

        //GET: api/<AssignmentController>
        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string AssignmentId)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string fileName = $"{AssignmentId}.gz";

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using Stream downloadStream = download.Content;

            using Stream bodyStream = HttpContext.Response.Body;
            using GZipStream decompressionStream = new GZipStream(downloadStream, CompressionMode.Decompress);

            await decompressionStream.CopyToAsync(bodyStream);

            return Ok();
        }

        //POST api/<AssignmentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        {
            string compressedFilePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.gz");
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string fileName = $"{AssignmentId}.gz";

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

            Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

            using (FileStream compressedFileStream = System.IO.File.Create(compressedFilePath))
            {
                using GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);

                using Stream bodyStream = HttpContext.Request.Body;
                await bodyStream.CopyToAsync(compressionStream);
            }
            using (FileStream fileStream = System.IO.File.OpenRead(compressedFilePath))
            {
                await blobClient.UploadAsync(fileStream, overwrite: true);
                await blobClient.SetMetadataAsync(assignmentInfo);
            }

            return Ok();
        }

       

        //public async Task<IActionResult> Post([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        //{
        //    string compressedFilePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.gz");
        //    string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
        //    string fileName = $"{AssignmentId}.gz";

        //    BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

        //    Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

        //    Stream input = HttpContext.Request.Body;
        //    using (MemoryStream compressStream = new MemoryStream())
        //    {
        //        using (GZipStream compressor = new GZipStream(compressStream, CompressionMode.Compress))
        //        {
        //            await input.CopyToAsync(compressor);

        //            compressStream.Position = 0;

        //            await blobClient.UploadAsync(compressStream);
        //            await blobClient.SetMetadataAsync(assignmentInfo);
        //        }
        //    }
        //    return Ok();
        //}

        // DELETE api/<AssignmentController>
        [HttpDelete]
        public IActionResult Delete([FromHeader] string AssignmentID)
        {
            System.IO.File.Delete($"./{AssignmentID}.json");
            return Ok();
        }
    }
}
