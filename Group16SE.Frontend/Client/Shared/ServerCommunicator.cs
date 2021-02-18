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
        /// <summary>
        /// Serializes and sends an assignment to the server.
        /// </summary>
        /// <returns></returns>
        public static async Task AssignmentToServer(AssignmentModel assignment)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44387/api/assignment");

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            requestMessage.Content = new StringContent(JsonSerializer.Serialize<AssignmentModel>(assignment, options), Encoding.UTF8, "application/json");
            requestMessage.Headers.Add("AssignmentId", $"{assignment.AssignmentId.Substring(0, 5)}.{assignment.AssignmentName}");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            responseMessage.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Fetches and deserializes an assignment from the server.
        /// </summary>
        /// <returns></returns>
        public static async Task<AssignmentModel> AssignmentFromServer()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44387/api/assignment");

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

            requestMessage.Headers.Add("AssignmentId", "Test Assignment");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            Stream jsonStream = await responseMessage.Content.ReadAsStreamAsync();
            AssignmentModel assignmentModel = await JsonSerializer.DeserializeAsync<AssignmentModel>(jsonStream, options);

            responseMessage.EnsureSuccessStatusCode();

            return assignmentModel;
        }
    }
}
