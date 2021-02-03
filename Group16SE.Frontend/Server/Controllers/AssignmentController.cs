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
        public async Task<ActionResult<AssignmentModel>> Get([FromHeader] string AssignmentId)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            using FileStream openStream = System.IO.File.OpenRead(Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.json"));
            AssignmentModel model = await JsonSerializer.DeserializeAsync<AssignmentModel>(openStream, options);

            return Ok(model);
        }

        // POST api/<AssignmentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string AssignmentId, AssignmentModel assignmentModel)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            
            using FileStream writeStream = System.IO.File.OpenWrite(Path.Combine(hostEnvironment.ContentRootPath, $"Test JSONs/{AssignmentId}.json"));
            await JsonSerializer.SerializeAsync<AssignmentModel>(writeStream, assignmentModel, options);
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
