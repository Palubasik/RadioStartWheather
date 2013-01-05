using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace RadioStart.WheatherGadgetProcess
{
    [Serializable]
    public class WheatherGadgetData
    {
        public List<GadgetLayerData> Layers = new List<GadgetLayerData>();
        public string ConnectionString = "http://www.google.com/ig/api?weather=brest,belarus";
        public string SubForm = "current_conditions";
        public int Timeout = 360000;
        public List<GadgetRuleData> Rules = new List<GadgetRuleData>();
        public List<GadgetItemTagData> Tags = new List<GadgetItemTagData>();
        public int W = 300;
        public int H = 300;
        public List<string> Parameters;
        public WheatherGadgetData() {
            Parameters = new List<string>();
            foreach(string s in Enum.GetNames(typeof(GadgetItem)))
                Parameters.Add(s);
        } 
    }   

    [Serializable]
    public class GadgetLayerData 
    {
        public int Position;
        public string Name;
        public int X = 0;
        public int Y = 0;
        public int W = 10;
        public int H = 10;
        public LayerType Type = LayerType.Unknown;
        public string Sample;
        public SerializableFont Font = new SerializableFont(new Font("Microsoft Sans Serif", 12));
        public int Color = -16777216;//Black
        public bool IsNullable;

        [NonSerialized]
        public string Value;

        public string ListName { get { return Name; } }
        
    }

    public sealed class SerializableFont
    {
        // public properties are required for serialization only
        public string FamilyName;
        public float EmSize;

        // required for serializaton only
        public SerializableFont()
        {
        }

        public SerializableFont(Font font)
        {
            FamilyName = font.FontFamily.Name;
            EmSize = font.Size;
        }

        [XmlIgnore]
        public Font Font
        {
            get
            {
                if (FamilyName == null || EmSize == 0)
                    return null;
                return new Font(FamilyName, EmSize);
            }
        }
    }

    [Serializable]
    public enum LayerType 
    {
        Text,
        Picture,
        Unknown
    }

    [Serializable]
    public class GadgetRuleData
    {
        public string RegexValue;
        public string Parameter;
        public string Layer;
        public Operation Operation;
        public string FileName;
        public string Name;
        public string ListName { get { return Name; } }
        [NonSerialized]
        public bool Correct = false;
        
    }

    [Serializable]
    public enum GadgetItem 
    {
        Temperature,
        Humidity,
        Condition,
        Wind
    }

    
    [Serializable]
    public class GadgetItemTagData    
    {
        public string Parameter;
        public string TagName;
        [NonSerialized]
        public string Value;
        public string VisibleName { get { return String.Format("{0} - {1}", Parameter, TagName); } }
    }

    [Serializable]
    public enum Operation
    {
        Great,
        GreatEqual,
        LessEqual,
        Less,
        Equal
    }
}
