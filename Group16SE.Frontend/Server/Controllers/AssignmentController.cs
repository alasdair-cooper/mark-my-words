using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IEnumerable<IDictionary<string, string>>> Get()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, "assignments");

            List<Dictionary<string, string>> blobMetadata = new List<Dictionary<string, string>>();

            // Call the listing operation and return pages of the specified size.
            IAsyncEnumerable<Azure.Page<BlobItem>> resultSegment = containerClient.GetBlobsAsync(BlobTraits.Metadata).AsPages(default);

            // Enumerate the blobs returned for each page.
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    blobMetadata.Add((Dictionary<string, string>)blobItem.Metadata);
                }
            }

            return blobMetadata;
        }

        [HttpGet("{assignmentId}")]
        public async Task<IActionResult> Get(string assignmentId)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string fileName = $"{assignmentId}.gz";

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using Stream downloadStream = download.Content;

            using Stream bodyStream = HttpContext.Response.Body;
            using GZipStream decompressionStream = new GZipStream(downloadStream, CompressionMode.Decompress);

            await decompressionStream.CopyToAsync(HttpContext.Response.Body);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string fileName = $"{AssignmentId}.gz";

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

            Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

            using MemoryStream memoryStream = new MemoryStream();
            using (Stream bodyStream = HttpContext.Request.Body)
            {
                using (GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    await bodyStream.CopyToAsync(compressionStream);
                }
            }
            memoryStream.Position = 0;

            await blobClient.UploadAsync(memoryStream, overwrite: true);
            await blobClient.SetMetadataAsync(assignmentInfo);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string fileName = $"{AssignmentId}.gz";

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

            Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

            using MemoryStream memoryStream = new MemoryStream() ;
            using (Stream bodyStream = HttpContext.Request.Body)
            {
                using (GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    await bodyStream.CopyToAsync(compressionStream);
                }
            }
            memoryStream.Position = 0;

            await blobClient.UploadAsync(memoryStream, overwrite: true);
            await blobClient.SetMetadataAsync(assignmentInfo);

            return Ok();
        }

        
    }
}
