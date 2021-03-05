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
    [ApiController]
    public class AttemptController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public AttemptController(IWebHostEnvironment webHostEnvironment)
        {
            hostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string AssignmentId)
        {
            string compressedFilePath = Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.gz");
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string fileName = $"{AssignmentId}.gz";


            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using Stream downloadStream = download.Content;

            AssignmentModel assignment = new AssignmentModel();
            AttemptModel newAttempt = new AttemptModel();

            using (GZipStream decompressionStream = new GZipStream(downloadStream, CompressionMode.Decompress))
            {
                assignment = await JsonSerializer.DeserializeAsync<AssignmentModel>(decompressionStream, options);
            }
            using (Stream bodyStream = HttpContext.Request.Body)
            {
                newAttempt = await JsonSerializer.DeserializeAsync<AttemptModel>(HttpContext.Request.Body);
            }

            Dictionary<string, string> assignmentInfo = assignment.GetAssignmentInfo();

            assignment.Attempts[assignment.Attempts.FindIndex(attempt => attempt.AttemptId == newAttempt.AttemptId)] = newAttempt;

            byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes<AssignmentModel>(assignment, options);

            using (FileStream compressedFileStream = System.IO.File.Create(compressedFilePath))
            {
                using GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);

                using Stream bodyStream = new MemoryStream(jsonString);
                await bodyStream.CopyToAsync(compressionStream);
            }
            using (FileStream fileStream = System.IO.File.OpenRead(compressedFilePath))
            {
                await blobClient.UploadAsync(fileStream, overwrite: true);
                await blobClient.SetMetadataAsync(assignmentInfo);
            }

            return Ok();
        }
    }
}
