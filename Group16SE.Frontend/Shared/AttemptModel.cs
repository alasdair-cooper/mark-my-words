using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class AttemptModel
    {
        public string AttemptID { get; set; }
        public List<SectionModel> Sections { get; set; }
        public bool Completed { get; set; }

        public AttemptModel(List<SectionModel> sections)
        {
            Sections = sections;

            Completed = false;
        }
    }
}
