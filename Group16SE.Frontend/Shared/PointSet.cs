using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group16SE.Frontend.Shared
{
    public class PointSet
    {
        public List<SliderPointModel> SliderPoints { get; set; }
        public List<SwitchPointModel> SwitchPoints { get; set; }
        public List<AutocompletePointModel> AutocompletePoints { get; set; }

        public PointSet(List<SliderPointModel> sliderPoints = default, List<SwitchPointModel> switchPoints = default, List<AutocompletePointModel> autocompletePoints = default)
        {
            if (sliderPoints == default)
            {
                sliderPoints = new List<SliderPointModel>();
            }
            SliderPoints = sliderPoints;

            if (switchPoints == default)
            {
                switchPoints = new List<SwitchPointModel>();
            }
            SwitchPoints = switchPoints;

            if (autocompletePoints == default)
            {
                autocompletePoints = new List<AutocompletePointModel>();
            }
            AutocompletePoints = autocompletePoints;
        }
    }
}
