using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

using System.Reflection;

namespace MarkMyWords.Shared
{
    public class AssignmentModel
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public string AssignmentId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Name of the assignment.
        /// </summary>
        public string AssignmentName { get; set; } = "";
        /// <summary>
        /// Banks of previously used comments.
        /// </summary>
        public Dictionary<string, List<CommentModel>> SectionCommentBanks { get; set; } = new Dictionary<string, List<CommentModel>>();
        /// <summary>
        /// Banks of all the points in each section.
        /// </summary>
        public Dictionary<string, List<PointModel>> SectionPointBanks { get; set; } = new Dictionary<string, List<PointModel>>();

        /// <summary>
        /// All the attempts to be marked and commented in this assignment.
        /// </summary>
        public List<AttemptModel> Attempts { get; set; } = new List<AttemptModel>();

        /// <summary>
        /// Empty constructor intended for the deserializer.
        /// </summary>
        public AssignmentModel() { }
        /// <summary>
        /// Creates a new assignment with a specified name, number of attempts and sections.
        /// </summary>
        /// <param name="assignmentName">The name of the assignment.</param>
        /// <param name="attemptCount">The number of attempts to be created in the assignment.</param>
        /// <param name="sections">The sections containing the points to be in each attempt</param>
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

        public Dictionary<string, string> GetAssignmentInfo()
        {
            List<PropertyInfo> propertyInfo = new List<PropertyInfo>(this.GetType().GetProperties());
            return propertyInfo
                .Where(propertyInfo => !(propertyInfo.PropertyType.IsGenericType) && (propertyInfo.GetValue(this) as string != null))
                .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(this)
                .ToString());
        }
    }
}
