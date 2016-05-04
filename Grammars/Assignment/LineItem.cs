using System;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace Grammars.Assignment
{
    public class LineItem : IComparable<LineItem>
    {
        private const char DELIMITER = ':';
        private const string START_ITEM = "Start";

        private int _from;
        private int _to;

        public LineItem()
        {
        }

        public LineItem(string line)
        {
            var parts = line.Split(DELIMITER);
            Name = parts[0];
            Expression = parts[1];
            Weight = int.Parse(parts[2]);
            if (parts.GetUpperBound(0) == 3)
            {
                Next = parts[3];
            }
        }

        public LineItem(string[] fields)
        {
            Name = fields[0];
            Next = fields[1];
            Weight = int.Parse(fields[2]);
            Expression = fields[3];
        }

        public LineItem(string name, string next, string expression)
        {
            Name = name;
            Expression = expression;
            Next = next;
        }

        public LineItem(string name, string next, string expression, int weight)
        {
            Name = name;
            Expression = expression;
            Next = next;
            Weight = weight;
        }

        [XmlAttribute("name")]
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [XmlText]
        [YamlMember(Alias = "value")]
        public string Expression { get; set; }

        [XmlAttribute("weight")]
        [YamlMember(Alias = "weight")]
        public int Weight { get; set; } = 1;

        [XmlAttribute("next")]
        [YamlMember(Alias = "next")]
        public string Next { get; set; }

        [XmlAttribute("variable")]
        [YamlMember(Alias = "variable")]
        public string Variable { get; set; }

        [XmlAttribute("from")]
        [YamlMember(Alias = "from")]
        public int FromValue
        {
            get { return _from; }
            set
            {
                if (_from != value)
                {
                    _from = value;
                    if (_to >= _from)
                    {
                        Weight = _to - _from + 1;
                    }
                }
            }
        }

        [XmlAttribute("to")]
        [YamlMember(Alias = "to")]
        public int ToValue
        {
            get { return _to; }
            set
            {
                if (_to != value)
                {
                    _to = value;
                    if (_to >= _from)
                    {
                        Weight = _to - _from + 1;
                    }
                }
            }
        }

        public bool FromValueSpecified
        {
            get { return FromValue != 0; }
        }

        public bool ToValueSpecified
        {
            get { return ToValue != 0; }
        }

        public bool WeightSpecified
        {
            get { return Weight != 1 && FromValue == 0 && ToValue == 0; }
        }

        public bool NextSpecified
        {
            get { return !string.IsNullOrWhiteSpace(Next); }
        }

        public int CompareTo(LineItem other)
        {
            if (Name == START_ITEM && other.Name == START_ITEM)
            {
                return 0;
            }
            if (Name == START_ITEM)
            {
                return -1;
            }
            if (other.Name == START_ITEM)
            {
                return 1;
            }
            return Name.CompareTo(other.Name);
        }

        public override string ToString()
        {
            return Name + Convert.ToString(DELIMITER) + Expression + Convert.ToString(DELIMITER) +
                   Convert.ToString(Weight) + Convert.ToString(DELIMITER) + Next;
        }

        public static bool operator ==(LineItem l, LineItem r)
        {
            if (Equals(l, null) || Equals(r, null))
            {
                return Equals(l, null) && Equals(r, null);
            }

            return l.Name == r.Name &&
                   l.Next == r.Next &&
                   l.ToValue == r.ToValue &&
                   l.FromValue == r.FromValue &&
                   l.Weight == r.Weight;
        }

        public static bool operator !=(LineItem l, LineItem r)
        {
            return !(l == r);
        }
    }
}