using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Server.Services
{
    interface ICosmosDbService
    {
        Task<IEnumerable<AttemptModel>> GetAttemptsAsync(string query);
        Task<AttemptModel> GetAttemptAsync(string id);
        Task AddAttemptAsync(AttemptModel attempt);
        Task UpdateAttemptAsync(string id, AttemptModel attempt);
        Task DeleteAttemptAsync(string id);

        Task<IEnumerable<AssignmentModel>> GetAssignmentsAsync(string query);
        Task<AssignmentModel> GetAssignmentAsync(string id);
        Task AddAssignmentAsync(AssignmentModel assignment);
        Task UpdateAssignmentAsync(string id, AssignmentModel assignment);
        Task DeleteAssignmentAsync(string id);
    }
}
