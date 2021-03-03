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
        private const string ListRequestUri = "api/list";

        /// <summary>
        /// Serializes and sends an assignment to the server.
        /// </summary>
        /// <returns></returns>
        public static async Task AssignmentToServer(string baseUrl, AssignmentModel assignment, NavigationManager navMan)
        {
            if (await IsOnline())
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, baseUrl + AssignmentRequestUri);

                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

                requestMessage.Content = new StringContent(JsonSerializer.Serialize<AssignmentModel>(assignment, options), Encoding.UTF8, MediaType);
                requestMessage.Headers.Add("AssignmentId", assignment.AssignmentId);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                responseMessage.EnsureSuccessStatusCode();
            }
            else
            {
                navMan.NavigateTo("/offline");
            }
        }

        /// <summary>
        /// Fetches and deserializes an assignment from the server.
        /// </summary>
        /// <returns></returns>
        public static async Task<AssignmentModel> AssignmentFromServer(string baseUrl, string assignmentId, NavigationManager navMan)
        {
            if (await IsOnline())
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl + AssignmentRequestUri);

                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Converters.Add(new PointModelConverterWithTypeDiscriminator());

                requestMessage.Headers.Add("AssignmentId", assignmentId);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                Stream jsonStream = await responseMessage.Content.ReadAsStreamAsync();
                AssignmentModel assignmentModel = await JsonSerializer.DeserializeAsync<AssignmentModel>(jsonStream, options);

                responseMessage.EnsureSuccessStatusCode();

                return assignmentModel;
            }
            else
            {
                navMan.NavigateTo("/offline");
                return null;
            }
        }
        /// <summary>
        /// Fetches a list of all the available assignments from the server.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> AssignmentListFromServer(string baseUrl, NavigationManager navMan)
        {
            if (await IsOnline())
            {
                HttpClient client = new HttpClient();
                Console.WriteLine(baseUrl + ListRequestUri);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl + ListRequestUri);

                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                Stream jsonStream = await responseMessage.Content.ReadAsStreamAsync();
                List<string> assignments = await JsonSerializer.DeserializeAsync<List<string>>(jsonStream);

                responseMessage.EnsureSuccessStatusCode();

                return assignments;
            }
            else
            {
                navMan.NavigateTo("/offline");
                return null;
            }
        }

        public static async Task<bool> IsOnline()
        {
            try
            {
                //HttpClient client = new HttpClient();
                //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com/");
                //await client.SendAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
