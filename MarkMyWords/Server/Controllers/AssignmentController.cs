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
        public async Task<AssignmentModel> Get(string assignmentId, [FromHeader] string attemptNameKey = null)
        {
            string fileName = $"{assignmentId}.gz";

            AssignmentModel assignment = await DownloadFromStorage(fileName);

            return assignment;
        }

        [HttpPost]
        public async Task Post([FromBody] AssignmentModel assignment)
        {
            string fileName = $"{assignment.AssignmentId}.gz";

            await UploadToStorage(fileName, assignment);
        }

        [HttpPut("{attemptId}")]
        public async Task Put([FromBody] AssignmentModel assignment, string attemptId)
        {
            string fileName = $"{assignment.AssignmentId}.gz";

            AssignmentModel assignmentFromServer = await DownloadFromStorage(fileName);

            int oldAttemptIndex = assignmentFromServer.Attempts.FindIndex(oldAttempt => oldAttempt.AttemptId == attemptId);
            assignmentFromServer.Attempts.RemoveAt(oldAttemptIndex);
            assignmentFromServer.Attempts.Insert(oldAttemptIndex, assignment.Attempts.Find(attempt => attempt.AttemptId == attemptId));

            await UploadToStorage(fileName, assignmentFromServer);
        }

        [HttpPatch]
        public async Task Patch([FromBody] AssignmentModel assignment)
        {
            string fileName = $"{assignment.AssignmentId}.gz";

            MemoryStream stream = new MemoryStream();
            AssignmentModel assignmentFromServer = await DownloadFromStorage(fileName);
            stream.Position = 0;

            assignment.Attempts = assignmentFromServer.Attempts;
            assignment.SectionCommentBanks = assignmentFromServer.SectionCommentBanks;
            assignment.SectionPointBanks = assignmentFromServer.SectionPointBanks;

            await UploadToStorage(fileName, assignment);
        }

        private static async Task<AssignmentModel> DownloadFromStorage(string fileName)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using Stream downloadStream = download.Content;

            Stream decompressedStream = await Decompress(downloadStream);

            AssignmentModel assignment = await JsonSerializer.DeserializeAsync<AssignmentModel>(decompressedStream, Utils.DefaultOptions());
            return assignment;
        }

        private static async Task UploadToStorage(string fileName, AssignmentModel assignment)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

            MemoryStream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, assignment, Utils.DefaultOptions());
            stream.Position = 0;
            Stream compressedStream = await Compress(stream);
            compressedStream.Position = 0;
            await blobClient.UploadAsync(compressedStream, overwrite: true);
            await blobClient.SetMetadataAsync(assignment.GetAssignmentInfo());
        }

        private static async Task<Stream> Compress(Stream streamToCompress)
        {
            MemoryStream memoryStream = new MemoryStream();
            using GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
            await streamToCompress.CopyToAsync(compressionStream);
            return memoryStream;
        }

        private static async Task<Stream> Decompress(Stream streamToDecompress)
        {
            MemoryStream stream = new MemoryStream();

            using GZipStream decompressionStream = new GZipStream(streamToDecompress, CompressionMode.Decompress, true);
            await decompressionStream.CopyToAsync(stream);

            stream.Position = 0;
            return stream;
        }
    }
}
