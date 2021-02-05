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
        public string SectionID { get; set; }
        /// <summary>
        /// Comments within the section.
        /// </summary>
        public List<CommentModel> Comments { get; set; }
        /// <summary>
        /// Sliders in the section.
        /// </summary>
        public List<SliderPointModel> SliderPoints { get; set; }
        /// <summary>
        /// Switches in the section.
        /// </summary>
        public List<SwitchPointModel> SwitchPoints { get; set; }
        /// <summary>
        /// Autocompletes in the section.
        /// </summary>
        public List<AutocompletePointModel> AutocompletePoints { get; set; }
        /// <summary>
        /// Mark specified by the user.
        /// </summary>
        public int GivenMark { get; set; }
        /// <summary>
        /// Minimum mark suggested by the system.
        /// </summary>
        public int SuggestedMark { get; set; }
        /// <summary>
        /// Maximum mark suggested by the system.
        /// </summary>
        public int MaxSuggestedMark { get; set; }
        /// <summary>
        /// Maximum mark allowed for the section.
        /// </summary>
        public int MaximumMark { get; set; }

        /// <summary>
        /// Creates an empty section with a random ID.
        /// </summary>
        public SectionModel()
        {
            SectionID = Guid.NewGuid().ToString();
            Comments = new List<CommentModel>();
            SliderPoints = new List<SliderPointModel>();
            SwitchPoints = new List<SwitchPointModel>();
            AutocompletePoints = new List<AutocompletePointModel>();
            GivenMark = 0;
            SuggestedMark = 0;
            MaximumMark = 10;
        }

        /// <summary>
        /// Creates a section with specified comments, and can also take a unique ID, point lists, and max mark.
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="sectionID"></param>
        /// <param name="sliderPoints"></param>
        /// <param name="switchPoints"></param>
        /// <param name="autocompletePoints"></param>
        /// <param name="maximumMark"></param>
        public SectionModel(List<CommentModel> comments, string sectionID = default, List<SliderPointModel> sliderPoints = default, List<SwitchPointModel> switchPoints = default, List<AutocompletePointModel> autocompletePoints = default, int maximumMark = 10)
        {
            if(sectionID == default)
            {
                sectionID = Guid.NewGuid().ToString();
            }
            SectionID = sectionID;

            Comments = comments;

            if(sliderPoints == default)
            {
                sliderPoints = new List<SliderPointModel>();
            }
            SliderPoints = sliderPoints;
            
            if(switchPoints == default)
            {
                switchPoints = new List<SwitchPointModel>();
            }
            SwitchPoints = switchPoints;
            
            if(autocompletePoints == default)
            {
                autocompletePoints = new List<AutocompletePointModel>();
            }
            AutocompletePoints = autocompletePoints;    

            GivenMark = 0;
            SuggestedMark = 0;
            MaximumMark = maximumMark;
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

            Console.WriteLine(newBankComment.Uses);

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

            Console.WriteLine(bankComment.Uses);

            Comments.Add(newSectionComment);
        }

        /// <summary>
        /// Deletes a comment just from the current section.
        /// </summary>
        /// <param name="comment">Comment to be deleted.</param>
        public void DeleteCommentInstance(CommentModel comment)
        {
            comment.OriginalCommentModel.Uses -= 1;
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
