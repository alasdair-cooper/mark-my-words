using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using MarkMyWords.Shared;

using Microsoft.AspNetCore.Components;

namespace MarkMyWords.Client.Shared
{
    public static class ServerCommunicator
    {
        private const string MediaType = "application/json";

        private enum HttpMethodEnum
        {
            Post,
            Put,
            Patch
        }

        /// <summary>
        /// Creates a new assignment via HTTP POST to api/assignment.
        /// </summary>
        /// <param name="navMan"></param>
        /// <param name="assignment"></param>
        /// <returns></returns>
        public static async Task<bool> NewAssignment(NavigationManager navMan, AssignmentModel assignment)
        {
            string destinationUri = $"{navMan.BaseUri}api/assignment";

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            Dictionary<string, string> headers = new Dictionary<string, string>() 
            { 
                { "AssignmentId", assignment.AssignmentId } ,
                {"AssignmentInfo", JsonSerializer.Serialize(assignment.GetAssignmentInfo()) }
            };

            return await Upload(assignment, destinationUri, headers, options, HttpMethodEnum.Post);
        }

        /// <summary>
        /// Updates a single assignment via HTTP PUT to api/assignment.
        /// </summary>
        /// <param name="navMan"></param>
        /// <param name="assignment"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateAssignment(NavigationManager navMan, AssignmentModel assignment, AttemptModel attempt)
        {
            string destinationUri = $"{navMan.BaseUri}api/assignment";

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            Dictionary<string, string> headers;
            headers = new Dictionary<string, string>()
            {
                { "AssignmentId", assignment.AssignmentId },
                {"AssignmentInfo", JsonSerializer.Serialize(assignment.GetAssignmentInfo()) }
            };
          
            return await Upload(attempt, destinationUri, headers, options, HttpMethodEnum.Put);
            
        }

        public static async Task<bool> UpdateAssignmentProperties(NavigationManager navMan, AssignmentModel assignment)
        {
            string destinationUri = $"{navMan.BaseUri}api/assignment";

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            Dictionary<string, string> headers;
            headers = new Dictionary<string, string>()
            {
                { "AssignmentId", assignment.AssignmentId },
                {"AssignmentInfo", JsonSerializer.Serialize(assignment.GetAssignmentInfo()) }
            };

            return await Upload(assignment, destinationUri, headers, options, HttpMethodEnum.Patch);

        }

        /// <summary>
        /// Makes a request to api/assignment for a list of all assignments in storage.
        /// </summary>
        /// <param name="navMan"></param>
        /// <returns></returns>
        public static async Task<List<Dictionary<string, string>>> ListAssignments(NavigationManager navMan)
        {
            string destinationUri = $"{navMan.BaseUri}api/assignment";

            Stream jsonStream = await Download(destinationUri);
            List<Dictionary<string, string>> assignments = await JsonSerializer.DeserializeAsync<List<Dictionary<string, string>>>(jsonStream);

            return assignments;
        }

        /// <summary>
        /// Fetches a single assignment via HTTP GET from api/assignment/{assignmentId}.
        /// </summary>
        /// <param name="navMan"></param>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        public static async Task<AssignmentModel> FetchAssignment(NavigationManager navMan, string assignmentId)
        {
            string destinationUri = $"{navMan.BaseUri}api/assignment/{assignmentId}";

            Dictionary<string, string> headers = new Dictionary<string, string>() 
            { 
                { "AssignmentId", assignmentId } 
            };

            Stream jsonStream = await Download(destinationUri, headers);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            AssignmentModel assignmentModel = await JsonSerializer.DeserializeAsync<AssignmentModel>(jsonStream, options);

            return assignmentModel;

        }

        /// <summary>
        /// Fetches the range of suggested marks via HTTP POST from api/mark.
        /// </summary>
        /// <param name="navMan"></param>
        /// <param name="assignmentId"></param>
        /// <param name="attemptId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, float>> FetchMarkRange(NavigationManager navMan, AssignmentModel assignment, string attemptId, string sectionId)
        {
            string destinationUri = $"{navMan.BaseUri}api/mark";

            Dictionary<string, string> headers = new Dictionary<string, string>() 
            { 
                { "AssignmentId", assignment.AssignmentId }, 
                { "AttemptId", attemptId }, 
                { "SectionId", sectionId } 
            };

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            HttpClient client = new HttpClient();

            Console.WriteLine(JsonSerializer.Serialize(headers));
            Console.WriteLine(JsonSerializer.Serialize(assignment, options));

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, destinationUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(assignment, options), Encoding.UTF8, MediaType)
            };
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            foreach (KeyValuePair<string, string> header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
            Stream responseBody = await responseMessage.Content.ReadAsStreamAsync();
            Dictionary<string, float> markRange  = await JsonSerializer.DeserializeAsync<Dictionary<string, float>>(responseBody);

            return markRange;
        }

        private static async Task<Stream> Download(string uri, Dictionary<string, string> headers = default)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            return await responseMessage.Content.ReadAsStreamAsync();
        }

        private static async Task<bool> Upload(
            object payload, 
            string uri, 
            Dictionary<string, string> headers = default, 
            JsonSerializerOptions serializerOptions = default, 
            HttpMethodEnum method = HttpMethodEnum.Post)
        {
            HttpClient client = new HttpClient();
            HttpMethod httpMethod = method switch
            {
                HttpMethodEnum.Put => HttpMethod.Put,
                HttpMethodEnum.Patch => HttpMethod.Patch,
                _ => HttpMethod.Post,
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload, serializerOptions), Encoding.UTF8, MediaType)
            };

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            foreach (KeyValuePair<string, string> header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
            return responseMessage.IsSuccessStatusCode;
        }
    }
}
