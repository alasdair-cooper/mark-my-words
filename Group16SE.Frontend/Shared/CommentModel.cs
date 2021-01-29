using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class CommentModel
    {
        public enum EditMode
        {
            None,
            Bank,
            Instance
        }
        public string Content { get; set; }

        public CommentModel OriginalCommentModel { get; set; }

        public CommentModel()
        {
            Content = "";
            OriginalCommentModel = null;
        }

        public CommentModel(string content = "")
        {
            Content = content;
        }

        public CommentModel(CommentModel originalCommentModel)
        {
            Content = originalCommentModel.Content;
            OriginalCommentModel = originalCommentModel;
            Console.WriteLine(OriginalCommentModel.Content);
        }
    }
}
