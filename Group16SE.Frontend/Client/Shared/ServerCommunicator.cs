using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Client.Shared
{
    public static class ServerCommunicator
    {
        private const string MediaType = "application/json";
        private const string AssignmentRequestUri = "https://localhost:44387/api/assignment";
        private const string ListRequestUri = "https://localhost:44387/api/list";

        /// <summary>
        /// Serializes and sends an assignment to the server.
        /// </summary>
        /// <returns></returns>
        public static async Task AssignmentToServer(AssignmentModel assignment)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, AssignmentRequestUri);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            requestMessage.Content = new StringContent(JsonSerializer.Serialize<AssignmentModel>(assignment, options), Encoding.UTF8, MediaType);
            requestMessage.Headers.Add("AssignmentId", assignment.AssignmentId);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            responseMessage.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Fetches and deserializes an assignment from the server.
        /// </summary>
        /// <returns></returns>
        public static async Task<AssignmentModel> AssignmentFromServer(string assignmentId)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, AssignmentRequestUri);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            requestMessage.Headers.Add("AssignmentId", assignmentId);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            Stream jsonStream = await responseMessage.Content.ReadAsStreamAsync();
            AssignmentModel assignmentModel = await JsonSerializer.DeserializeAsync<AssignmentModel>(jsonStream, options);

            responseMessage.EnsureSuccessStatusCode();

            return assignmentModel;
        }
        /// <summary>
        /// Fetches a list of all the available assignments from the server.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> AssignmentListFromServer()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, ListRequestUri);

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            Stream jsonStream = await responseMessage.Content.ReadAsStreamAsync();
            List<string> assignments = await JsonSerializer.DeserializeAsync<List<string>>(jsonStream);

            responseMessage.EnsureSuccessStatusCode();

            return assignments;
        }
    }
}
