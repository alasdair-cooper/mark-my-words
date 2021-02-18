using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public class PointModel
    {
        public string Type { get; set; }
        public string Tag { get; set; }
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
            Type = "Slider";
            Value = 0;
            Min = 0;
            Max = 10;
            Step = 1;
        }

        public SliderPointModel(string tag, int step, int value, int min, int max)
        {
            Tag = tag;
            Type = "Slider";
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
            Type = "Switch";
            Value = false;
        }

        public SwitchPointModel(string tag, bool value = false)
        {
            Tag = tag;
            Type = "Switch";
            Value = value;
        }
    }
}
