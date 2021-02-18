using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    /// <summary>
    /// Enum to represent how a comment is being edited
    /// </summary>
    public enum EditMode
    {
        None,
        Bank,
        Instance
    }

    public class CommentModel
    {
        /// <summary>
        /// The editmode specifies if a comment is being edited, or the attached original is.
        /// </summary>
        public EditMode EditMode { get; set; }

        /// <summary>
        /// Unique ID.
        /// </summary>
        public string CommentId { get; set; }

        /// <summary>
        /// The user decided content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Number of uses of a comment (for when it is in a bank).
        /// </summary>
        public int Uses { get; set; }

        // ID for the backend to use
        private string originalCommentModelId = null;
        private CommentModel _originalCommentModel = null;

        /// <summary>
        /// The comment where the comment came from (for comments in sections).
        /// </summary>
        public CommentModel OriginalCommentModel
        {
            get
            {
                return _originalCommentModel;
            }
            set
            {
                _originalCommentModel = value;
                if (value != null)
                {
                    originalCommentModelId = value.CommentId;
                }
                else
                {
                    originalCommentModelId = null;
                }
            } 
        }
      
        /// <summary>
        /// Cretes an empty comment from no original with a random ID. 
        /// </summary>
        public CommentModel()
        {
            EditMode = EditMode.None;
            CommentId = Guid.NewGuid().ToString();
            Content = "";
            Uses = -1;
        }

        /// <summary>
        /// Creates a comment from no original with a random ID and specified content.
        /// </summary>
        /// <param name="content">The text applied to the comment.</param>
        public CommentModel(string content)
        {
            EditMode = EditMode.None;
            CommentId = Guid.NewGuid().ToString();
            Content = content;
            Uses = -1;
        }

        /// <summary>
        /// Creates a comment from some original comment.
        /// </summary>
        /// <param name="originalCommentModel">The original comment model (bank comment).</param>
        public CommentModel(CommentModel originalCommentModel)
        {
            EditMode = EditMode.None;
            CommentId = originalCommentModel.CommentId;
            Content = originalCommentModel.Content;
            OriginalCommentModel = originalCommentModel;
            Uses = -1;
        }

        /// <summary>
        /// Edits a comment in a section.
        /// </summary>
        /// <param name="commentBank">The bank of the current section.</param>
        //public void EditCommentInstance(List<CommentModel> commentBank)
        //{
        //    CommentModel newBankComment = new CommentModel(Content);
        //    newBankComment.Uses = 1;

        //    OriginalCommentModel.Uses -= 1;
        //    OriginalCommentModel = newBankComment;

        //    commentBank.Add(newBankComment);
        //}

        /// <summary>
        /// Edits a comment in a bank
        /// </summary>
        //public void EditCommentGlobal()
        //{
        //    OriginalCommentModel.Content = Content;
        //}
    }
}
