﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class AttemptModel
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public string AttemptId { get; set; }
        /// <summary>
        /// Sections to display and mark.
        /// </summary>
        public List<SectionModel> Sections { get; set; }
        /// <summary>
        /// Specifies if an attempt has finished being marked.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Creates a new attempt with a random ID and empty sections.
        /// </summary>
        public AttemptModel()
        {
            AttemptId = Guid.NewGuid().ToString();
            Sections = new List<SectionModel>();
            Completed = false;
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

            Sections = sections;

            Completed = false;
        }
    }
}
