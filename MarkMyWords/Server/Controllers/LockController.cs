using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MarkMyWords.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarkMyWords.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Post([FromHeader] string assignmentId, [FromHeader] string attemptId, [FromHeader] bool locked)
        {
            string fileName = $"{assignmentId}.gz";

            AssignmentModel assignment = await Storage.DownloadFromStorage(fileName);

            int attemptIndex = assignment.Attempts.FindIndex(oldAttempt => oldAttempt.AttemptId == attemptId);
            AttemptModel attempt = assignment.Attempts.ElementAt(attemptIndex);
            attempt.Locked = locked;
            assignment.Attempts.RemoveAt(attemptIndex);
            assignment.Attempts.Insert(attemptIndex, attempt);

            await Storage.UploadToStorage(fileName, assignment);

            Console.WriteLine($"Attempt locked: {assignment.Attempts.ElementAt(attemptIndex).Locked}");

            return Ok();
        }
    }
}
