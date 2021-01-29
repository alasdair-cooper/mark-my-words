using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class PointModel
    {
        private readonly int weight;
        public string Tag { get; set; }

        public PointModel(int newPointWeight = 1)
        {
            weight = newPointWeight;
        }
    }

    public class SliderPointModel : PointModel
    {
        public int Step { get; set; }
        public int Value { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        
        public SliderPointModel()
        {
            Tag = "";
            Value = 0;
            Min = 0;
            Max = 10;
            Step = 1;
        }

        public SliderPointModel(string tag, int step, int value, int min, int max)
        {
            Tag = tag;
            Step = step;
            Value = value;
            Min = min;
            Max = max;
        }
    }

    public class SwitchPointModel : PointModel
    {
        public bool Value { get; set; }

        public SwitchPointModel()
        {
            Tag = "";
            Value = false;
        }

        public SwitchPointModel(string tag, bool value = false)
        {
            Tag = tag;
            Value = value;
        }
    }

    public class AutocompletePointModel : PointModel
    {
        public string Value { get; set; }
        public string[] PossibleValues { get; set; }

        public AutocompletePointModel()
        {
            Value = "";
            PossibleValues = null;
        }
    }
}
