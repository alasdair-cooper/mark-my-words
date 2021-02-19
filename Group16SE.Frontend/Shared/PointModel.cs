using System;
using System.Collections.Generic;
using System.Text;

namespace Group16SE.Frontend.Shared
{
    public enum PointType
    {
        Default,
        Slider,
        Switch
    }

    /// <summary>
    /// A parent class for all points in a section, that each represent a control type.
    /// </summary>
    public class PointModel
    {
        public string PointId { get; set; } = Guid.NewGuid().ToString();
        private string stringPointType = "";
        private PointType enumPointType = PointType.Default;
        /// <summary>
        /// The type of the control that represents this point graphically. It also updates the string representation for the backend.
        /// </summary>
        public PointType Type
        {
            get
            {
                return enumPointType;
            }
            set
            {
                stringPointType = value.ToString();
                enumPointType = value;
            }
        }
        /// <summary>
        /// A short string representing a label on a control.
        /// </summary>
        public string Tag { get; set; } = "";
    }

    /// <summary>
    /// Represents a slider control.
    /// </summary>
    public class SliderPointModel : PointModel
    {
        /// <summary>
        /// The distance between values on the slider.
        /// </summary>
        public int Step { get; set; } = 1;
        /// <summary>
        /// The current value.
        /// </summary>
        public int Value { get; set; } = 0;
        /// <summary>
        /// The minimum value.
        /// </summary>
        public int Min { get; set; } = 1;
        /// <summary>
        /// The maximum value.
        /// </summary>
        public int Max { get; set; } = 10;
        
        public SliderPointModel()
        {
            Type = PointType.Slider;
        }

        public SliderPointModel(string tag, int step, int value, int min, int max)
        {
            Tag = tag;
            Type = PointType.Slider;
            Step = step;
            Value = value;
            Min = min;
            Max = max;
        }
    }

    /// <summary>
    /// Represents a switch control.
    /// </summary>
    public class SwitchPointModel : PointModel
    {
        /// <summary>
        /// The current state of the switch control.
        /// </summary>
        public bool Value { get; set; } = false;

        public SwitchPointModel()
        {
            Type = PointType.Switch;
        }

        public SwitchPointModel(string tag, bool value)
        {
            Tag = tag;
            Type = PointType.Switch;
            Value = value;
        }
    }
}
