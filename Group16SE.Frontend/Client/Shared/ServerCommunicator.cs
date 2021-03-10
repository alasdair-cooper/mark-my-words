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
using Group16SE.Frontend.Shared;

using Microsoft.AspNetCore.Components;

namespace Group16SE.Frontend.Client.Shared
{
    public static class ServerCommunicator
    {
        private const string MediaType = "application/json";
        private const string AssignmentRequestUri = "api/assignment";
        private const string AttemptRequestUri = "api/attempt";
        private const string ListRequestUri = "api/list";

        private enum HttpMethodEnum
        {
            Post,
            Put
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
        public static async Task<bool> UpdateAssignment(NavigationManager navMan, AssignmentModel assignment, string finalUpdateAttemptId = null)
        {
            string destinationUri = $"{navMan.BaseUri}api/assignment";

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            Dictionary<string, string> headers;
            if (finalUpdateAttemptId == null)
            {
                headers = new Dictionary<string, string>()
                {
                    { "AssignmentId", assignment.AssignmentId },
                    {"AssignmentInfo", JsonSerializer.Serialize(assignment.GetAssignmentInfo()) },
                    { "FinalUpdate", "false" }
                };
            }
            else
            {
                headers = new Dictionary<string, string>()
                {
                    { "AssignmentId", assignment.AssignmentId },
                    {"AssignmentInfo", JsonSerializer.Serialize(assignment.GetAssignmentInfo()) },
                    { "FinalUpdate", "true" },
                    { "AttemptId", finalUpdateAttemptId }
                };
            }
            return await Upload(assignment, destinationUri, headers, options, HttpMethodEnum.Put);
            
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
            Console.WriteLine($"Sending request to {destinationUri}.");

            Dictionary<string, string> headers = new Dictionary<string, string>() 
            { 
                { "AssignmentId", assignmentId } 
            };

            Stream jsonStream = await Download(destinationUri, headers);

            StreamReader reader = new StreamReader(jsonStream);
            Console.WriteLine(reader.ReadToEnd());
            jsonStream.Position = 0;


            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            AssignmentModel assignmentModel = await JsonSerializer.DeserializeAsync<AssignmentModel>(jsonStream, options);

            return assignmentModel;

        }

        /// <summary>
        /// Fetches the range of suggested marks via HTTP GET from api/mark.
        /// </summary>
        /// <param name="navMan"></param>
        /// <param name="assignmentId"></param>
        /// <param name="attemptId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public static async Task<Tuple<int, int>> FetchMarkRange(NavigationManager navMan, AssignmentModel assignment, string attemptId, string sectionId)
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
            Tuple<int, int> markRange  = await JsonSerializer.DeserializeAsync<Tuple<int, int>>(responseBody);

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

        private static async Task<bool> Upload(object payload, string uri, Dictionary<string, string> headers = default, JsonSerializerOptions serializerOptions = default, HttpMethodEnum method = HttpMethodEnum.Post)
        {
            HttpClient client = new HttpClient();
            HttpMethod httpMethod = method switch
            {
                HttpMethodEnum.Put => HttpMethod.Put,
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
