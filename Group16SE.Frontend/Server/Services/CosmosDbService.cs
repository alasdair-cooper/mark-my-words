using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Server.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAttemptAsync(AttemptModel attempt)
        {
            await this._container.CreateItemAsync<AttemptModel>(attempt, new PartitionKey(attempt.AttemptId));
        }

        public async Task DeleteAttemptAsync(string id)
        {
            await this._container.DeleteItemAsync<AttemptModel>(id, new PartitionKey(id));
        }

        public async Task<AttemptModel> GetAttemptAsync(string id)
        {
            try
            {
                ItemResponse<AttemptModel> response = await this._container.ReadItemAsync<AttemptModel>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<AttemptModel>> GetAttemptsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<AttemptModel>(new QueryDefinition(queryString));
            List<AttemptModel> results = new List<AttemptModel>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAttemptAsync(string id, AttemptModel attempt)
        {
            await this._container.UpsertItemAsync<AttemptModel>(attempt, new PartitionKey(id));
        }

        public async Task AddAssignmentAsync(AssignmentModel assignment)
        {
            await this._container.CreateItemAsync<AssignmentModel>(assignment, new PartitionKey(assignment.AssignmentId));
        }

        public async Task DeleteAssignmentAsync(string id)
        {
            await this._container.DeleteItemAsync<AssignmentModel>(id, new PartitionKey(id));
        }

        public async Task<AssignmentModel> GetAssignmentAsync(string id)
        {
            try
            {
                ItemResponse<AssignmentModel> response = await this._container.ReadItemAsync<AssignmentModel>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<AssignmentModel>> GetAssignmentsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<AssignmentModel>(new QueryDefinition(queryString));
            List<AssignmentModel> results = new List<AssignmentModel>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAssignmentAsync(string id, AssignmentModel assignment)
        {
            await this._container.UpsertItemAsync<AssignmentModel>(assignment, new PartitionKey(id));
        }
    }
}
