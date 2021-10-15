using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;

using System.Text.Json;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, "mmw-storage");

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
        public async Task<ActionResult<AssignmentModel>> Get(string assignmentId, [FromHeader] string password = null)
        {
            string fileName = $"{assignmentId}.gz";

            AssignmentModel assignment = await Storage.DownloadFromStorage(fileName);

            if (password != null)
            {
                if (assignment.PasswordMatch(password))
                {
                    assignment = Storage.Decrypt(assignment, password);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(assignment);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AssignmentModel assignment, [FromHeader] string password = null)
        {
            if (password != null)
            {
                if (assignment.PasswordMatch(password))
                {
                     Storage.Encrypt(assignment, password);
                }
                else
                {
                    return BadRequest();
                }
            }

            string fileName = $"{assignment.AssignmentId}.gz";

            await Storage.UploadToStorage(fileName, assignment);

            return Ok();
        }

        [HttpPut("{attemptId}")]
        public async Task<ActionResult> Put([FromBody] AssignmentModel assignment, string attemptId, [FromHeader] string password = null)
        {
            if(password != null)
            {
                if(assignment.PasswordMatch(password))
                {
                    Storage.Encrypt(assignment, password);
                }
                else
                {
                    return BadRequest();
                }
            }

            string fileName = $"{assignment.AssignmentId}.gz";

            AssignmentModel assignmentFromServer = await Storage.DownloadFromStorage(fileName);

            int oldAttemptIndex = assignmentFromServer.Attempts.FindIndex(oldAttempt => oldAttempt.AttemptId == attemptId);
            assignmentFromServer.Attempts.RemoveAt(oldAttemptIndex);
            assignmentFromServer.Attempts.Insert(oldAttemptIndex, assignment.Attempts.Find(attempt => attempt.AttemptId == attemptId));

            foreach (KeyValuePair<string, List<CommentModel>> keyValuePair in assignment.SectionCommentBanks)
            {
                foreach (CommentModel comment in keyValuePair.Value)
                {
                    int index = assignmentFromServer.SectionCommentBanks[keyValuePair.Key].FindIndex(commentModel => commentModel.CommentId == comment.CommentId);

                    if (index == -1)
                    {
                        assignmentFromServer.SectionCommentBanks[keyValuePair.Key].Add(comment);
                    }
                    else 
                    {
                        assignmentFromServer.SectionCommentBanks[keyValuePair.Key].RemoveAt(index);
                        assignmentFromServer.SectionCommentBanks[keyValuePair.Key].Insert(index, comment);
                    } 
                }
            }

            await Storage.UploadToStorage(fileName, assignmentFromServer);

            return Ok();
        }

        [HttpPatch]
        public async Task Patch([FromBody] AssignmentModel assignment)
        {
            string fileName = $"{assignment.AssignmentId}.gz";

            MemoryStream stream = new MemoryStream();
            AssignmentModel assignmentFromServer = await Storage.DownloadFromStorage(fileName);
            stream.Position = 0;

            assignment.Attempts = assignmentFromServer.Attempts;
            assignment.SectionCommentBanks = assignmentFromServer.SectionCommentBanks;
            assignment.SectionPointBanks = assignmentFromServer.SectionPointBanks;

            await Storage.UploadToStorage(fileName, assignment);
        }
    }
}
