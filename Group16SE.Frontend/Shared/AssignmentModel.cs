using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group16SE.Frontend.Shared
{
    public class AssignmentModel
    {
        public string AssignmentID { get; set; }
        public Dictionary<string, List<CommentModel>> SectionCommentBanks { get; set; }
        public List<AttemptModel> Attempts { get; set; }

        public AssignmentModel()
        {
            AssignmentID = "";
            Attempts = new List<AttemptModel>();
            SectionCommentBanks = new Dictionary<string, List<CommentModel>>();
        }

        public AssignmentModel(List<AttemptModel> attempts = default, Dictionary<string, List<CommentModel>> sectionCommentBanks = default)
        {
            if(attempts == default)
            {
                Attempts = new List<AttemptModel>();
            }
            else
            {
                Attempts = attempts;
            }


            if (sectionCommentBanks == default)
            {
                SectionCommentBanks = new Dictionary<string, List<CommentModel>>();
            }
            else 
            {
                SectionCommentBanks = sectionCommentBanks;
            }
        }
    }
}
