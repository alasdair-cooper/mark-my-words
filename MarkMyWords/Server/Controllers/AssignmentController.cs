using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using System.Reflection;

using System.IO;
using System.IO.Compression;

using MarkMyWords.Shared;

namespace MarkMyWords.Server.Controllers
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
            string fileName = $"{assignmentId}.gz";

            await DownloadFromStorage(fileName, HttpContext.Response.Body);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        {
            string fileName = $"{AssignmentId}.gz";

            Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

            await UploadToStorage(fileName, HttpContext.Request.Body, assignmentInfo);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        {
            string fileName = $"{AssignmentId}.gz";
            Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

            MemoryStream stream = new MemoryStream();
            await DownloadFromStorage(fileName, stream);
            stream.Position = 0;

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());
            AssignmentModel assignment = await JsonSerializer.DeserializeAsync<AssignmentModel>(stream, options);
            AttemptModel attempt = await JsonSerializer.DeserializeAsync<AttemptModel>(HttpContext.Request.Body, options);
            int oldAttemptIndex = assignment.Attempts.FindIndex(oldAttempt => oldAttempt.AttemptId == attempt.AttemptId);
            assignment.Attempts.RemoveAt(oldAttemptIndex);
            assignment.Attempts.Insert(oldAttemptIndex, attempt);

            MemoryStream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, assignment, options);
            memoryStream.Position = 0;
            await UploadToStorage(fileName, memoryStream, assignmentInfo);
           
            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromHeader] string AssignmentId, [FromHeader] string AssignmentInfo)
        {
            string fileName = $"{AssignmentId}.gz";
            Dictionary<string, string> assignmentInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(AssignmentInfo);

            MemoryStream stream = new MemoryStream();
            await DownloadFromStorage(fileName, stream);
            stream.Position = 0;

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());
            AssignmentModel oldAssignment = await JsonSerializer.DeserializeAsync<AssignmentModel>(stream, options);
            AssignmentModel currentAssignment = await JsonSerializer.DeserializeAsync<AssignmentModel>(HttpContext.Request.Body, options);

            currentAssignment.Attempts = oldAssignment.Attempts;
            currentAssignment.SectionCommentBanks = oldAssignment.SectionCommentBanks;
            currentAssignment.SectionPointBanks = oldAssignment.SectionPointBanks;

            MemoryStream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, currentAssignment, options);
            memoryStream.Position = 0;
            await UploadToStorage(fileName, memoryStream, assignmentInfo);

            return Ok();
        }

        private static async Task DownloadFromStorage(string fileName, Stream streamToWriteTo)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using Stream downloadStream = download.Content;

            await Decompress(downloadStream, streamToWriteTo);
        }

        private static async Task UploadToStorage(string fileName, Stream content, Dictionary<string, string> metadata = null)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

            using Stream compressedStream = await Compress(content);

            await blobClient.UploadAsync(compressedStream, overwrite: true);
            if (metadata != null)
            {
                await blobClient.SetMetadataAsync(metadata);
            }
        }

        private static async Task<Stream> Compress(Stream streamToCompress)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                await streamToCompress.CopyToAsync(compressionStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static async Task Decompress(Stream streamToDecompress, Stream streamToWriteTo)
        {
            GZipStream decompressionStream = new GZipStream(streamToDecompress, CompressionMode.Decompress, true);
            await decompressionStream.CopyToAsync(streamToWriteTo);
        }
    }
}
