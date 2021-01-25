using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class CommentModel
    {
        public string Content { get; set; }

        CommentModel OriginalCommentModel { get; set; }

        public CommentModel(string content = "")
        {
            Content = content;
        }

        public CommentModel(ref CommentModel originalCommentModel)
        {
            Content = originalCommentModel.Content;
            OriginalCommentModel = originalCommentModel;
            Console.WriteLine(OriginalCommentModel.Content);
        }
    }
}
