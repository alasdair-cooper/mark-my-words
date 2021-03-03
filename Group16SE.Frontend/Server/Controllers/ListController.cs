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
        public IEnumerable<string> Get()
        {
            string folderPath = Path.Combine(hostEnvironment.ContentRootPath, "Test JSONs");
            List<string> assignmentIds = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Extension == ".gz")
                {
                    assignmentIds.Add(file.Name.Split('.')[0]);
                }
            }
            return assignmentIds;
        }
    }
}
