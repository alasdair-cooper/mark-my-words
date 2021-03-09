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

using Group16SE.Frontend.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Group16SE.Frontend.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public ListController(IWebHostEnvironment webHostEnvironment)
        {
            hostEnvironment = webHostEnvironment;
        }

        // GET: api/<ListController>
        [HttpGet]
        public async Task<IEnumerable<IDictionary<string, string>>> Get()
        {
            //string folderPath = Path.Combine(hostEnvironment.ContentRootPath, "Test JSONs");
            //List<string> assignmentIds = new List<string>();
            //DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            //foreach (FileInfo file in directoryInfo.GetFiles())
            //{
            //    if (file.Extension == ".gz")
            //    {
            //        assignmentIds.Add(file.Name.Split('.')[0]);
            //    }
            //}

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
    }
}
