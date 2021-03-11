﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;


namespace MarkMyWords.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public MarkController(IWebHostEnvironment webHostEnvironment)
        {
            hostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IDictionary<string, float>> Post([FromHeader] string AssignmentId, [FromHeader] string AttemptId, [FromHeader] string SectionId)
        {
            string absolutePath = Path.Combine(hostEnvironment.ContentRootPath, $"{AssignmentId}.json");

            using (FileStream fileStream = System.IO.File.Create(absolutePath))
            {
                using Stream bodyStream = HttpContext.Request.Body;
                await bodyStream.CopyToAsync(fileStream);
            }

            string pythonExecutablePath = Path.Combine(hostEnvironment.ContentRootPath, "python39/python.exe");
            string pythonCodeFilePath = Path.Combine(hostEnvironment.ContentRootPath, "Python/model_training.py");
            string jsonAssignmentPath = absolutePath;

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonExecutablePath,
                // Argument with file name and input parameters
                Arguments = $"{pythonCodeFilePath} {jsonAssignmentPath} {SectionId} {AttemptId}",
                UseShellExecute = false,
                CreateNoWindow = true,
                // Any output, generated by application will be redirected back
                RedirectStandardOutput = true,
                // Any error in standard output will be redirected back (for example exceptions)
                RedirectStandardError = true,
                // Runs process elevated
                // THIS PROPERTY IS VITAL FOR THE PROCESS TO RUN ON THE AZURE APP SERVICE
                // Doesn't matter when running locally (or in development environment maybe?)
                Verb = "runas"
            };
            using Process process = Process.Start(start);
            using StreamReader reader = process.StandardOutput;
            string stderr = process.StandardError.ReadToEnd();
            string result = reader.ReadToEnd();

            System.IO.File.Delete(absolutePath);

            //if (!string.IsNullOrWhiteSpace(stderr.Trim()))
            //{
            //    throw new Exception(stderr);
            //}

            Console.WriteLine(stderr);

            Dictionary<string, float> markRange = JsonSerializer.Deserialize<Dictionary<string, float>>(result);

            return markRange;
        }
    }
}