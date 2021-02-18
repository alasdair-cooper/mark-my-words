using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Group16SE.Frontend.Shared
{
    public class AssignmentModel
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public string AssignmentId { get; set; } = Guid.NewGuid().ToString();

        public string AssignmentName { get; set; } = "";
        /// <summary>
        /// Banks of previously used comments.
        /// </summary>
        public Dictionary<string, List<CommentModel>> SectionCommentBanks { get; set; } = new Dictionary<string, List<CommentModel>>();

        // public Dictionary<string, PointSet> SectionPointBanks { get; set; }

        public Dictionary<string, List<PointModel>> SectionPointBanks { get; set; } = new Dictionary<string, List<PointModel>>();

        /// <summary>
        /// All the attempts to be marked and commented in this assignment.
        /// </summary>
        public List<AttemptModel> Attempts { get; set; } = new List<AttemptModel>();

        private HttpClient client = new HttpClient();

        /// <summary>
        /// Creates a new assignment with a random ID and no attempts, as well as empty comment banks.
        /// </summary>
        public AssignmentModel()
        {
            
        }

        public AssignmentModel(string assignmentName, int attemptCount, List<SectionModel> sections)
        {
            AssignmentName = assignmentName;

            for (int i = 0; i < attemptCount; i++)
            {
                Attempts.Add(new AttemptModel(new List<SectionModel>(sections)));
            }

            foreach (SectionModel sectionModel in sections)
            {
                SectionPointBanks.Add(sectionModel.SectionID, sectionModel.Points);
                SectionCommentBanks.Add(sectionModel.SectionID, new List<CommentModel>());
            }
        }
    }
}
