using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace Group16SE.Frontend.Shared
{
    public class PointModelConverterWithTypeDiscriminator : JsonConverter<PointModel>
    {
        public override bool CanConvert(Type typeToConvert) 
        {
            return typeof(PointModel).IsAssignableFrom(typeToConvert); 
        }

        public override PointModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if(reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if(propertyName != "Type")
            {
                throw new JsonException();
            }

            reader.Read();
            if(reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            string pointType = reader.GetString();
            PointModel pointModel = pointType switch
            {
                "Slider" => new SliderPointModel(),
                "Switch" => new SwitchPointModel(),
                _ => throw new JsonException()
            };

            while (reader.Read())
            {
                if(reader.TokenType == JsonTokenType.EndObject)
                {
                    return pointModel;
                }

                if(reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Tag":
                            string tag = reader.GetString();
                            pointModel.Tag = tag;
                            break;
                        case "Value":
                            switch (pointType)
                            {
                                case "Slider":
                                    int intValue = reader.GetInt32();
                                    ((SliderPointModel)pointModel).Value = intValue;
                                    break;
                                case "Switch":
                                    bool boolValue = reader.GetBoolean();
                                    ((SwitchPointModel)pointModel).Value = boolValue;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Min":
                            int min = reader.GetInt32();
                            ((SliderPointModel)pointModel).Min = min;
                            break;
                        case "Max":
                            int max = reader.GetInt32();
                            ((SliderPointModel)pointModel).Max = max;
                            break;
                        case "Step":
                            int step = reader.GetInt32();
                            ((SliderPointModel)pointModel).Step = step;
                            break;
                    }
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, PointModel pointModel, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("Type", pointModel.Type);

            if(pointModel is SwitchPointModel switchPointModel)
            {
                writer.WriteBoolean("Value", switchPointModel.Value);
            }
            else if(pointModel is SliderPointModel sliderPointModel)
            {
                writer.WriteNumber("Value", sliderPointModel.Value);
                writer.WriteNumber("Step", sliderPointModel.Step);
                writer.WriteNumber("Min", sliderPointModel.Min);
                writer.WriteNumber("Max", sliderPointModel.Max);
            }

            writer.WriteString("Tag", pointModel.Tag);

            writer.WriteEndObject();
        }
    }
}
