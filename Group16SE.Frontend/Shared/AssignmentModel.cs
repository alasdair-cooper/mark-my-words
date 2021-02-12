using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group16SE.Frontend.Shared
{
    public class AssignmentModel
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public string AssignmentId { get; set; }
        /// <summary>
        /// Banks of previously used comments.
        /// </summary>
        public Dictionary<string, List<CommentModel>> SectionCommentBanks { get; set; }

        public Dictionary<string, PointSet> SectionPointBanks { get; set; }

        /// <summary>
        /// All the attempts to be marked and commented in this assignment.
        /// </summary>
        public List<AttemptModel> Attempts { get; set; }

        /// <summary>
        /// Creates a new assignment with a random ID and no attempts, as well as empty comment banks.
        /// </summary>
        public AssignmentModel()
        {
            AssignmentId = Guid.NewGuid().ToString();
            Attempts = new List<AttemptModel>();
            SectionCommentBanks = new Dictionary<string, List<CommentModel>>();
        }

        /// <summary>
        /// Creates a new assignment with optional ID, attempts and banks.
        /// </summary>
        /// <param name="assignmentId">Unique ID, else becomes a new GUID.</param>
        /// <param name="attempts">Attempts to be added to assignment.</param>
        /// <param name="sectionCommentBanks">Banks to be linked to sections in attempts.</param>
        public AssignmentModel(string assignmentId = default, List<AttemptModel> attempts = default, Dictionary<string, List<CommentModel>> sectionCommentBanks = default)
        {
            if(assignmentId == default)
            {
                assignmentId = Guid.NewGuid().ToString();
            }
            AssignmentId = assignmentId;

            if (attempts == default)
            {
                attempts = new List<AttemptModel>();
            }
            Attempts = attempts;
            
            if (sectionCommentBanks == default)
            {
                sectionCommentBanks = new Dictionary<string, List<CommentModel>>();
            }
            SectionCommentBanks = sectionCommentBanks;    
        }
    }
}
