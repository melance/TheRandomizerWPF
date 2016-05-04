using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using Utility;
using YamlDotNet.Serialization;

namespace Grammars.Phonotactics
{
    public enum CaseType
    {
        None,
        Proper,
        Upper,
        Lower
    }

    [XmlRoot("Phonotactics", IsNullable = false)]
    public class PhonotacticsGrammar : BaseGrammar
    {
        [XmlArray("definitions")]
        [XmlArrayItem("item", IsNullable = false)]
        private ObservableCollection<Definition> _Definitions = new ObservableCollection<Definition>();

        [XmlArray("patterns")]
        [XmlArrayItem("item", IsNullable = false)]
        private ObservableCollection<Pattern> _Patterns = new ObservableCollection<Pattern>();

        [XmlElement("case")]
        public CaseType Case { get; set; }

        private ObservableCollection<Definition> Definitions
        {
            get { return _Definitions; }
            set { _Definitions = value; }
        }

        private ObservableCollection<Pattern> Patterns
        {
            get { return _Patterns; }
            set { _Patterns = value; }
        }

        public static PhonotacticsGrammar GenerateGrammar(
            string name,
            string description,
            string author,
            BindingList<string> tags,
            bool supportsMaxLength,
            string sampleFile,
            string definitionFile,
            int maxWeight)
        {
            var grammar = new PhonotacticsGrammar();
            var samples = new List<string>(File.ReadAllLines(sampleFile));
            var definitions = new List<string>(File.ReadAllLines(definitionFile));

            grammar.Name = name;
            grammar.Author = author;
            grammar.Tags = tags;
            grammar.Description = description;
            grammar.SupportsMaxLength = supportsMaxLength;

            foreach (var definition in definitions)
            {
                var key = definition[0];
                var newDef = new Definition
                {
                    Delimiter = ",",
                    Key = key.ToString(),
                    Value = definition.Substring(2)
                };
                grammar.Definitions.Add(newDef);
            }

            foreach (var sample in samples)
            {
                if (!string.IsNullOrWhiteSpace(sample))
                {
                    var item = sample;
                    var result = new string[item.Length + 1];
                    var pattern = "";
                    foreach (var definition in grammar.Definitions)
                    {
                        foreach (var value in definition.ValueList)
                        {
                            var index = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                                item,
                                value,
                                CompareOptions.IgnoreCase);
                            while (index >= 0)
                            {
                                result[index] = definition.Key;
                                index = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                                    item,
                                    value,
                                    index + 1,
                                    CompareOptions.IgnoreCase);
                            }
                        }
                    }
                    pattern = string.Join(string.Empty, result);
                    pattern = pattern.Trim();
                    if (grammar.Patterns.FirstOrDefault(p => p.Value == pattern) == null)
                    {
                        grammar.Patterns.Add(new Pattern(pattern));
                    }
                    else
                    {
                        grammar.Patterns.FirstOrDefault(p => p.Value == pattern).Weight++;
                    }
                }
            }

            return grammar;
        }

        public override string Analyze()
        {
            const string tableRow = "<tr>{0}</tr>";
            const string tableCell = "<td>{0}</td>";

            var html = Resources.PhonotacticsGrammarAnalysis;
            var parameters = new StringBuilder();

            html = html.Replace("[Name]", Name);
            html = html.Replace("[Description]", Description);
            html = html.Replace("[Author]", Author);
            html = html.Replace("[Category]", Category);
            html = html.Replace("[ParameterCount]", Parameters.Count.ToString());
            html = html.Replace("[DefinitionCount]", Definitions.Count.ToString("#,##0"));
            html = html.Replace("[PatternCount]", Patterns.Count.ToString("#,##0"));
            html = html.Replace(
                "[XMLLength]",
                Strings.Split(ToString(), Environment.NewLine).Length.ToString("#,##00"));

            foreach (var parameter in Parameters)
            {
                var row = new StringBuilder();
                row.AppendFormatLine(tableCell, parameter.Name);
                parameters.AppendFormatLine(tableRow, row.ToString());
            }

            html = html.Replace("[Parameters]", parameters.ToString());

            return html;
        }

        protected override string GenerateName()
        {
            var pattern = SelectPattern();
            var patternParts = ParsePattern(pattern);
            var name = string.Empty;

            foreach (var part in patternParts)
            {
                if (!part.Optional || (part.Optional && Random.Next(100) > 50))
                {
                    name += GetValue(part.Key);
                }
            }

            switch (Case)
            {
                case CaseType.Lower:
                    name = name.ToLower();
                    break;
                case CaseType.Upper:
                    name = name.ToUpper();
                    break;
                case CaseType.Proper:
                    name = Strings.StrConv(name, VbStrConv.ProperCase);
                    break;
            }

            return name;
        }

        private string SelectPattern()
        {
            var totalWeight = Patterns.Sum(p => Math.Max(p.Weight, 1));
            var selected = Random.Next()%totalWeight + 1;

            var currentWeight = 0;
            foreach (var pattern in Patterns)
            {
                currentWeight += pattern.Weight;
                if (currentWeight >= selected)
                {
                    return pattern.Value;
                }
            }

            return string.Empty;
        }

        private List<PatternPart> ParsePattern(string pattern)
        {
            const char OPTIONAL_START = '(';
            const char OPTIONAL_END = ')';

            var optional = false;
            var value = new List<PatternPart>();

            foreach (var c in pattern)
            {
                switch (c)
                {
                    case OPTIONAL_START:
                        optional = true;
                        break;
                    case OPTIONAL_END:
                        break;
                    default:
                        value.Add(new PatternPart(c.ToString(), optional));
                        optional = false;
                        break;
                }
            }

            return value;
        }

        private string GetValue(string key)
        {
            var definition = Definitions.FirstOrDefault(d => d.Key == key);
            if (definition != null)
            {
                var values = definition.ValueList;
                var index = Convert.ToInt32(Random.Next()%values.Count);
                return values[index];
            }
            return key;
        }

        private class PatternPart
        {
            public PatternPart()
            {
            }

            public PatternPart(string key, bool optional)
            {
                Key = key;
                Optional = optional;
            }

            public string Key { get; }
            public bool Optional { get; }
        }
    }

    public class Definition
    {
        private List<string> _valueList;

        public Definition()
        {
        }

        public Definition(string key, char delimiter, string value)
        {
            Key = key;
            Delimiter = delimiter.ToString();
            Value = value;
        }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("delimiter")]
        public string Delimiter { get; set; }

        [XmlText]
        public string Value { get; set; }

        [XmlIgnore]
        [YamlIgnore]
        internal List<string> ValueList
        {
            get
            {
                if (_valueList == null)
                {
                    _valueList = new List<string>();
                    _valueList.AddRange(Value.Split(Delimiter.ToCharArray()[0]));
                }
                return _valueList;
            }
        }
    }

    public class Pattern
    {
        [XmlAttribute("weight")]
        private int _Weight = 1;

        public Pattern()
        {
        }

        public Pattern(string value)
        {
            Value = value;
        }

        [XmlText]
        public string Value { get; set; }

        public int Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }
    }
}