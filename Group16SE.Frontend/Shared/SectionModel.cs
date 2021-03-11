using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class SectionModel
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public string SectionID { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Name of the section.
        /// </summary>
        public string SectionName { get; set; } = "";
        /// <summary>
        /// Comments within the section.
        /// </summary>
        public List<CommentModel> Comments { get; set; } = new List<CommentModel>();

        public List<PointModel> Points { get; set; } = new List<PointModel>();
        /// <summary>
        /// Sliders in the section.
        /// </summary>
        //public List<SliderPointModel> SliderPoints { get; set; }
        /// <summary>
        /// Switches in the section.
        /// </summary>
        //public List<SwitchPointModel> SwitchPoints { get; set; }
        /// <summary>
        /// Mark specified by the user.
        /// </summary>
        public int GivenMark { get; set; } = 0;
        /// <summary>
        /// Minimum mark suggested by the system.
        /// </summary>
        public int SuggestedMark { get; set; } = 0;
        /// <summary>
        /// Maximum mark suggested by the system.
        /// </summary>
        public int MinSuggestedMark { get; set; }
        /// <summary>
        /// Maximum mark suggested by the system.
        /// </summary>
        public int MaxSuggestedMark { get; set; }
        /// <summary>
        /// Maximum mark allowed for the section.
        /// </summary>
        public int MaximumMark { get; set; } = 0;

        /// <summary>
        /// Empty constructor intended for the deserializer.
        /// </summary>
        public SectionModel()
        {

        }
    
        /// <summary>
        /// Adds a comment to the section from text.
        /// </summary>
        /// <param name="commentBank">Comment bank relating to the current section.</param>
        /// <param name="content">Text of comment to be added.</param>
        public void NewCommentFromString(List<CommentModel> commentBank, string content)
        {
            CommentModel newBankComment = new CommentModel(content);
            CommentModel newSectionComment = new CommentModel(newBankComment);

            newBankComment.Uses = 1;

            Comments.Add(newSectionComment);
            commentBank.Add(newBankComment);
        }

        /// <summary>
        /// Adds a comment to the section from another comment.
        /// </summary>
        /// <param name="bankComment">Comment from bank to be added.</param>
        public void NewCommentFromComment(CommentModel bankComment)
        {
            CommentModel newSectionComment = new CommentModel(bankComment);

            bankComment.Uses += 1;

            Comments.Add(newSectionComment);
        }

        /// <summary>
        /// Deletes a comment just from the current section.
        /// </summary>
        /// <param name="comment">Comment to be deleted.</param>
        public void DeleteCommentInstance(CommentModel comment, List<CommentModel> commentBank)
        {
            CommentModel OriginalCommentModel = commentBank.Find(bankComment => bankComment.CommentId == comment.OriginalCommentModelId);
            OriginalCommentModel.Uses -= 1;
            Comments.Remove(comment);
        }

        /// <summary>
        /// Deletes a comment just from the bank.
        /// </summary>
        /// <param name="commentBank">Comment bank relating to the current section.</param>
        /// <param name="comment">Comment to be deleted.</param>
        public void DeleteCommentGlobal(List<CommentModel> commentBank, CommentModel comment)
        {
            commentBank.Remove(comment);
            Comments.Remove(comment);
        }
    }
}
