using System;
using System.Collections.Generic;
using System.Text;

namespace MarkMyWords.Shared
{
    public class AttemptModel
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public string AttemptId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Sections to display and mark.
        /// </summary>
        public List<SectionModel> Sections { get; set; } = new List<SectionModel>();
        /// <summary>
        /// Specifies if an attempt has finished being marked.
        /// </summary>
        public bool Completed { get; set; } = false;

        public bool Locked { get; set; } = false;

        /// <summary>
        /// Empty constructor intended for the deserializer.
        /// </summary>
        public AttemptModel()
        {
            
        }

        /// <summary>
        /// Creates a new attempt with specified sections and an optional ID, else a random ID.
        /// </summary>
        /// <param name="sections">The sections that the attempt should be composed of.</param>
        /// <param name="attemptId">A unique ID to represent th attempt.</param>
        public AttemptModel(List<SectionModel> sections, string attemptId = default)
        {
            if(attemptId == default)
            {
                attemptId = Guid.NewGuid().ToString();
            }
            AttemptId = attemptId;

            Sections = Utils.DeepClone<List<SectionModel>>(sections);

            Completed = false;
        }
    }
}
