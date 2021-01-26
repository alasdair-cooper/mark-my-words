using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class SectionModel
    {
        public string SectionID { get; set; }
        public List<CommentModel> Comments { get; set; }
        public List<SliderPointModel> SliderPoints { get; set; }
        public List<SwitchPointModel> SwitchPoints { get; set; }
        public List<AutocompletePointModel> AutocompletePoints { get; set; }
        public int GivenMark { get; set; }
        public int SuggestedMark { get; set; }
        public int MaximumMark { get; set; }

        public SectionModel(string sectionID, List<CommentModel> comments, List<SliderPointModel> sliderPoints = default, List<SwitchPointModel> switchPoints = default, List<AutocompletePointModel> autocompletePoints = default, int maximumMark = 10)
        {
            SectionID = sectionID;
            Comments = comments;

            if(sliderPoints == default)
            {
                SliderPoints = new List<SliderPointModel>();
            }
            else
            {
                SliderPoints = sliderPoints;
            }
            if(switchPoints == default)
            {
                SwitchPoints = new List<SwitchPointModel>();
            }
            else
            {
                SwitchPoints = switchPoints;
            }
            if(autocompletePoints == default)
            {
                AutocompletePoints = new List<AutocompletePointModel>();
            }
            else
            {
                AutocompletePoints = autocompletePoints;
            }

            GivenMark = 0;
            SuggestedMark = 0;
            MaximumMark = maximumMark;
        }
    }
}
