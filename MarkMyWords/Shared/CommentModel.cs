using System;
using System.Collections.Generic;
using System.Text;

namespace MarkMyWords.Shared
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
        public EditMode EditMode { get; set; } = EditMode.None;

        /// <summary>
        /// Unique ID.
        /// </summary>
        public string CommentId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The user decided content.
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// Number of uses of a comment (for when it is in a bank).
        /// </summary>
        public int Uses { get; set; } = -1;

        // ID for the backend to use
        public string OriginalCommentModelId = null;

        /// <summary>
        /// Empty constructor intended for the deserializer.
        /// </summary>
        public CommentModel()
        {

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
            OriginalCommentModelId = originalCommentModel.CommentId;
            Uses = -1;
        }
    }
}
