using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using NCalc;
using Utility;
using YamlDotNet.Serialization;

namespace Grammars.Assignment
{
    [XmlRoot("grammar")]
    public class AssignmentGrammar : BaseGrammar
    {
        private const char IDENTIFIER_START = '[';
        private const char IDENTIFIER_END = ']';
        private const char CALC_NOTIFICATION = '=';
        private const string START_LABEL = "Start";
        private const string END_ITEM = "Stop";
        private const string LENGTH_PARAMETER = "Length";
        private const char OR_OPERATOR = '|';
        private const char AND_OPERATOR = '+';
        private const string LIBRARY_EXTENSION = ".lib.xml";

        private const string ITEMS_PARAMETER = "Items";
        private const string ITEMS_FUNCTION = "Items";
        private const string ITEM_FORMAT = "[{0}]";
        private const string ITEM_FORMAT_WITH_PREFIX = "[{0}{1}]";
        private const string NAME_FORMAT_WITH_PREFIX = "{0}{1}";

        private const string REPEAT_FUNCTION = "Repeat";

        private Calculator _calculator;
        private ObservableCollection<string> _import = new ObservableCollection<string>();
        private int _itemsEvaluated;
        private Dictionary<string, int> _itemsEvaluatedByName = new Dictionary<string, int>();
        private string _name;


        private ObservableCollection<LineItem> _rules = new ObservableCollection<LineItem>();

        private Dictionary<string, CachedLineItem> _rulesByName = new Dictionary<string, CachedLineItem>();
        private string _startItem = START_LABEL;

        public AssignmentGrammar()
        {
            _calculator = new Calculator(this);

            _calculator.EvaluateFunction += _calculator_EvaluateFunction;
            _calculator.EvaluateParameter += _calculator_EvaluateParameter;
        }


        [XmlArray("items")]
        [XmlArrayItem("item")]
        [YamlMember(Alias = "items")]
        public ObservableCollection<LineItem> Rules
        {
            get { return _rules; }
            set
            {
                _rules = value;
                _rules.CollectionChanged += _rules_CollectionChanged;
                OnPropertyChanged();
            }
        }

        [XmlArray("imports")]
        [XmlArrayItem("import")]
        [YamlMember(Alias = "imports")]
        public ObservableCollection<string> Import
        {
            get { return _import; }
            set
            {
                _import = value;
                _import.CollectionChanged += _import_CollectionChanged;
                OnPropertyChanged();
            }
        }


        public override string Analyze()
        {
            const string tableRow = "<tr>{0}</tr>";
            const string tableCell = "<td>{0}</td>";
            const string tableNumberCell = "<td class=\'number\'>{0}</td>";

            GatherImports();

            var html = Resources.AssignmentGrammarAnalysis;
            html = html.Replace("[Name]", Name);
            html = html.Replace("[Description]", Description);
            html = html.Replace("[Author]", Author);
            html = html.Replace("[Category]", Category);
            html = html.Replace("[ParameterCount]", Parameters.Count.ToString());
            html = html.Replace("[RuleCount]", Rules.Count.ToString("#,##0"));
            html = html.Replace(
                "[XMLLength]",
                Strings.Split(ToString(), Environment.NewLine).Length.ToString("#,##00"));

            var parameters = new StringBuilder();
            foreach (var parameter in Parameters)
            {
                var row = new StringBuilder();
                row.AppendFormatLine(tableCell, parameter.Name);
                parameters.AppendFormatLine(tableRow, row.ToString());
            }

            html = html.Replace("[Parameters]", parameters.ToString());
            var query = Rules.GroupBy(li => li.Name).Select(
                li => new
                {
                    key = li.Key,
                    weight = li.Sum(w => w.Weight),
                    count = li.Count()
                });

            var rules = new StringBuilder();
            foreach (var current in query)
            {
                var row = new StringBuilder();
                row.AppendFormatLine(tableCell, current.key);
                row.AppendFormatLine(tableNumberCell, current.count);
                row.AppendFormatLine(tableNumberCell, current.weight);
                rules.AppendFormatLine(tableRow, row.ToString());
            }

            html = html.Replace("[Rules]", rules.ToString());

            var warnings = new StringBuilder();
            var foundRules = new List<string>();
            foreach (var rule in Rules)
            {
                var parsed = ParseExpression(rule.Expression);
                var next = rule.Next;

                if (!string.IsNullOrWhiteSpace(next) && !foundRules.Contains(next))
                {
                    if (Rules.Any(li => li.Name == next))
                    {
                        foundRules.Add(next);
                    }
                    else
                    {
                        var warning = new StringBuilder();
                        warning.AppendFormatLine(tableCell, next);
                        warning.AppendFormatLine(tableCell, "next");
                        warning.AppendFormatLine(tableCell, "Could not locate rule.");
                        warnings.AppendFormatLine(tableRow, warning);
                    }
                }

                foreach (var item in parsed)
                {
                    if (item.Key == TokenType.Identifier)
                    {
                        var parts = item.Value.Split(AND_OPERATOR, OR_OPERATOR);
                        foreach (var part in parts)
                        {
                            var name = part.Remove(IDENTIFIER_START, IDENTIFIER_END);
                            if (!foundRules.Contains(name) && Rules.Any(li => li.Name == name))
                            {
                                foundRules.Add(name);
                            }
                            else
                            {
                                var warning = new StringBuilder();
                                warning.AppendFormatLine(tableCell, name);
                                warning.AppendFormatLine(tableCell, "expression");
                                warning.AppendFormatLine(tableCell, "Could not locate rule.");
                                warnings.AppendFormatLine(tableRow, warning);
                            }
                        }
                    }
                }
            }

            html = html.Replace("[Warnings]", warnings.ToString());

            return html;
        }


        protected override string GenerateName()
        {
            GatherImports();
            _name = string.Empty;
            var calc = new Expression(_startItem);
            calc.EvaluateParameter += EvaluateParameter;
            var startItem = GetItemByName(_startItem);
            Evaluate(startItem);

            return _name.Length > MaxLength ? (_name.Substring(0, MaxLength)) : _name;
        }


        public static dynamic MCGenerator(
            string name,
            string description,
            string author,
            BindingList<string> tags,
            bool supportsMaxLength,
            string sourceFile,
            int syllableLength,
            int maxWeight,
            string prefix,
            bool createLibrary)
        {
            var samples = new List<SampleItem>();
            var source = new List<string>(File.ReadAllLines(sourceFile));
            var rules = new List<LineItem>();

            foreach (var s in source)
            {
                var sample = s;
                var diff = sample.Length%syllableLength;

                if (diff != 0)
                {
                    sample += Strings.Space(diff);
                }
                string current;
                var previous = string.Empty;
                for (var i = 0; i < sample.Length; i += syllableLength)
                {
                    current = sample.Substring(i, syllableLength);
                    var existing = samples.Find(si => (si.Previous == previous && si.Next == current));
                    if (existing != null)
                    {
                        existing.Weight++;
                    }
                    else
                    {
                        samples.Add(new SampleItem(previous, current));
                    }
                    previous = current;
                }
                current = string.Empty;
                var final = samples.Find(si => (si.Previous == previous && si.Next == current));
                if (final != null)
                {
                    final.Weight++;
                }
                else
                {
                    samples.Add(new SampleItem(previous, current));
                }
            }

            foreach (var item in samples)
            {
                if (string.IsNullOrWhiteSpace(item.Previous))
                {
                    if (createLibrary)
                    {
                        rules.Add(
                            new LineItem(
                                prefix,
                                string.Empty,
                                string.IsNullOrEmpty(prefix)
                                    ? string.Format(ITEM_FORMAT, item.Next)
                                    : string.Format(ITEM_FORMAT_WITH_PREFIX, prefix, item.Next),
                                item.Weight));
                    }
                    else
                    {
                        rules.Add(
                            new LineItem(
                                START_LABEL,
                                string.Empty,
                                string.IsNullOrEmpty(prefix)
                                    ? string.Format(ITEM_FORMAT, item.Next)
                                    : string.Format(ITEM_FORMAT_WITH_PREFIX, prefix, item.Next),
                                item.Weight));
                    }
                }
                else
                {
                    rules.Add(
                        new LineItem(
                            string.Format(NAME_FORMAT_WITH_PREFIX, prefix, item.Previous),
                            string.IsNullOrEmpty(prefix)
                                ? item.Next
                                : string.Format(NAME_FORMAT_WITH_PREFIX, prefix, item.Next),
                            item.Previous,
                            item.Weight));
                }
            }

            rules.Sort();

            object value;
            if (createLibrary)
            {
                var library = new Library();
                library.Rules.AddRange(rules);
                value = library;
            }
            else
            {
                var grammar = new AssignmentGrammar
                {
                    Name = name,
                    Description = description,
                    Author = author,
                    Tags = tags,
                    SupportsMaxLength = supportsMaxLength
                };
                grammar.Rules.AddRange(rules);
                value = grammar;
            }

            return value;
        }

        public static AssignmentGrammar ItemListConverter(
            string name,
            string description,
            string author,
            BindingList<string> tags,
            bool supportsMaxLength,
            IEnumerable<ItemFile> listFiles,
            int weightAdjustment,
            bool removeDuplicates,
            bool caseSensitive)
        {
            var grammar = new AssignmentGrammar
            {
                Name = name,
                Description = description,
                Author = author,
                Tags = tags,
                SupportsMaxLength = supportsMaxLength
            };
            var rules = CreateRules(
                listFiles,
                weightAdjustment,
                removeDuplicates,
                caseSensitive);
            foreach (var rule in rules)
            {
                grammar.Rules.Add(rule);
            }

            return grammar;
        }

        internal static List<LineItem> CreateRules(
            IEnumerable<ItemFile> listFiles,
            int weightAdjustment,
            bool removeDuplicates,
            bool caseSensitive)
        {
            var lines = GetFileContents(
                listFiles,
                removeDuplicates,
                caseSensitive);
            var rules = new List<LineItem>();
            foreach (var entry in lines)
            {
                var label = entry.Key.Label;
                var rank = weightAdjustment < 0 ? entry.Value.Count : 1;

                foreach (var line in entry.Value)
                {
                    var rule = new LineItem(
                        label,
                        string.Empty,
                        line);
                    rule.Weight = rank;
                    rank += weightAdjustment;
                    rules.Add(rule);
                }
            }
            return rules;
        }

        private static Dictionary<ItemFile, List<string>> GetFileContents(
            IEnumerable<ItemFile> files,
            bool removeDuplicates,
            bool caseSensitive)
        {
            var values = new Dictionary<ItemFile, List<string>>();

            foreach (var file in files)
            {
                var fileValues = new List<string>(File.ReadAllLines(file.FileName));

                fileValues.RemoveAll(string.IsNullOrWhiteSpace);

                if (removeDuplicates)
                {
                    var comparer = caseSensitive
                        ? StringComparer.CurrentCulture
                        : StringComparer.CurrentCultureIgnoreCase;

                    var newValues = new List<string>();
                    foreach (var line in fileValues)
                    {
                        if (!newValues.Contains(line, comparer))
                        {
                            newValues.Add(line);
                        }
                    }

                    fileValues = newValues;
                }

                values.Add(file, fileValues);
            }
            return values;
        }


        private void GatherImports()
        {
            var paths = new List<string>();
            if (!string.IsNullOrEmpty(FileName))
            {
                paths.Add(FileName);
            }
            paths.AddRange(Utility.GrammarFilePaths);
            foreach (var item in Import)
            {
                var libraryPath = item;
                if (!Path.HasExtension(libraryPath))
                {
                    libraryPath += LIBRARY_EXTENSION;
                }
                if (!Path.IsPathRooted(libraryPath))
                {
                    var path = string.Empty;
                    var i = 0;
                    while (!File.Exists(path) && i < paths.Count)
                    {
                        path = Path.Combine(paths[i], libraryPath);
                        i++;
                    }
                    if (!File.Exists(path))
                    {
                        throw (new FileNotFoundException("Could not locate library the file " + item));
                    }
                    libraryPath = path;
                }
                var library = Library.FromFile(libraryPath);
                foreach (var rule in library.Rules)
                {
                    Rules.Add(rule);
                }
            }
            Import.Clear();
        }

        private Expression GetCalculator(string expression)
        {
            var calc = new Expression(expression);
            calc.Parameters.Add(LENGTH_PARAMETER, _name.Length);
            calc.EvaluateParameter += EvaluateParameter;
            return calc;
        }

        private LineItem GetItemByName(string name)
        {
            var nameListOr = Strings.Split(name, OR_OPERATOR.ToString());

            if (nameListOr.Length > 1)
            {
                name = nameListOr[Random.Next(0, nameListOr.Length)];
            }

            if (_rulesByName == null || !_rulesByName.ContainsKey(name))
            {
                var nameListAnd = name.Split(AND_OPERATOR);
                _rulesByName.Add(name, new CachedLineItem());
                foreach (var nameItem in nameListAnd)
                {
                    foreach (var item in Rules.Where(li => li.Name == nameItem.Remove("[", "]")).ToList())
                    {
                        _rulesByName[name].TotalWeight += item.Weight;
                        _rulesByName[name].Rules.Add(item);
                    }
                }
            }

            if (_rulesByName[name].TotalWeight > 0)
            {
                var sum = 0;
                var index = Convert.ToInt32((Random.Next()%_rulesByName[name].TotalWeight) + 1);
                while (_rulesByName[name].Rules.Count > 1 && index == _rulesByName[name].LastSelect)
                {
                    index = Convert.ToInt32((Random.Next()%_rulesByName[name].TotalWeight) + 1);
                }
                _rulesByName[name].LastSelect = index;
                foreach (var item in _rulesByName[name].Rules)
                {
                    sum += item.Weight;
                    if (sum >= index)
                    {
                        if (!_itemsEvaluatedByName.ContainsKey(name))
                        {
                            _itemsEvaluatedByName.Add(name, 0);
                        }
                        _itemsEvaluatedByName[name]++;
                        _itemsEvaluated++;
                        return item;
                    }
                }
            }
            return null;
        }

        private void Evaluate(LineItem item)
        {
            if (item != null && _name.Length < MaxLength)
            {
                if (!string.IsNullOrWhiteSpace(item.Expression))
                {
                    var parts = ParseExpression(item.Expression);

                    foreach (var part in parts)
                    {
                        if (part.Key == TokenType.String)
                        {
                            if (string.IsNullOrWhiteSpace(item.Variable))
                            {
                                _name += part.Value;
                            }
                            else
                            {
                                SetVariable(item.Variable, part.Value);
                            }
                        }
                        else if (part.Key == TokenType.Identifier)
                        {
                            var lineItem = GetItemByName(part.Value);
                            if (lineItem == null)
                            {
                                var parameterValue = GetValue(part.Value);
                                if (!string.IsNullOrWhiteSpace(parameterValue))
                                {
                                    lineItem = GetItemByName(parameterValue);
                                }
                            }
                            Evaluate(lineItem);
                        }
                        else if (part.Key == TokenType.Calc)
                        {
                            var expression = part.Value.Substring(1, part.Value.Length - 2);
                            if (string.IsNullOrWhiteSpace(item.Variable))
                            {
                                Evaluate(
                                    new LineItem(
                                        item.Name,
                                        string.Empty,
                                        (_calculator.Evaluate(expression)).ToString()));
                            }
                            else
                            {
                                SetVariable(item.Variable, _calculator.Evaluate(expression).ToString());
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(item.Next) &&
                    !item.Next.Equals(END_ITEM, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (item.Next.StartsWith(CALC_NOTIFICATION.ToString()))
                    {
                        Evaluate(GetItemByName(_calculator.Evaluate(item.Next.Substring(1)).ToString()));
                    }
                    else
                    {
                        Evaluate(GetItemByName(item.Next));
                    }
                }
            }
        }

        private void SetVariable(string variable, string value)
        {
            if (Variables.ContainsKey(variable))
            {
                Variables[variable] = value;
            }
            else
            {
                Variables.Add(variable, value);
            }
        }

        private List<KeyValuePair<TokenType, string>> ParseExpression(string expression)
        {
            var value = new List<KeyValuePair<TokenType, string>>();
            var type = default(TokenType);
            var current = string.Empty;
            var openBrackets = 0;

            for (var i = 0; i <= expression.Length - 1; i++)
            {
                var c = expression[i];
                switch (c)
                {
                    case CALC_NOTIFICATION:
                        if (type == TokenType.Identifier)
                        {
                            type = TokenType.Calc;
                        }
                        else
                        {
                            if (type == TokenType.None)
                            {
                                type = TokenType.String;
                            }
                            current += c.ToString();
                        }
                        break;
                    case IDENTIFIER_START:
                        openBrackets++;
                        if (openBrackets == 1)
                        {
                            if (!string.IsNullOrEmpty(current))
                            {
                                value.Add(new KeyValuePair<TokenType, string>(type, current));
                            }
                            type = TokenType.Identifier;
                            current = c.ToString();
                        }
                        else
                        {
                            current += c.ToString();
                        }
                        break;
                    case IDENTIFIER_END:
                        current += c.ToString();
                        openBrackets--;
                        if (type.In(TokenType.Identifier, TokenType.Calc) && openBrackets == 0)
                        {
                            if (!string.IsNullOrEmpty(current))
                            {
                                value.Add(new KeyValuePair<TokenType, string>(type, current));
                            }
                            type = TokenType.None;
                            current = string.Empty;
                        }
                        break;
                    default:
                        if (type == TokenType.None)
                        {
                            type = TokenType.String;
                        }
                        current += c.ToString();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(current))
            {
                value.Add(new KeyValuePair<TokenType, string>(type, current));
            }

            return value;
        }


        private string GetValue(string name)
        {
            var value =
                (name.StartsWith(IDENTIFIER_START.ToString()) &&
                 name.EndsWith(IDENTIFIER_END.ToString()))
                    ? name
                    : IDENTIFIER_START + name + Convert.ToString(IDENTIFIER_END);
            var parameter =
                Parameters.FirstOrDefault(p => IDENTIFIER_START + p.Name + Convert.ToString(IDENTIFIER_END) == value);
            return parameter != null ? parameter.Value : string.Empty;
        }

        private void EvaluateParameter(string name, ParameterArgs e)
        {
            var parameter = Parameters.FirstOrDefault(p => p.Name == name);
            if (parameter != null)
            {
                e.Result = parameter.Value;
            }
        }

        public void _calculator_EvaluateFunction(string name, FunctionArgs args)
        {
            switch (name)
            {
                case ITEMS_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        var itemName = Convert.ToString(args.Parameters[0].Evaluate());
                        if (_itemsEvaluatedByName.ContainsKey(itemName))
                        {
                            args.Result = _itemsEvaluatedByName[itemName];
                        }
                        else
                        {
                            args.Result = 0;
                        }
                    }
                    else if (!args.Parameters.Any())
                    {
                        args.Result = _itemsEvaluated;
                    }
                    else
                    {
                        throw (new EvaluationException(
                            "Invalid number of arguments for \'Generate\' function, 0 or 1 expected."));
                    }
                    break;
            }
        }

        public void _calculator_EvaluateParameter(string name, ParameterArgs args)
        {
            if (Variables.ContainsKey(name))
            {
                args.Result = get_Variable(name);
            }
            else
            {
                switch (name)
                {
                    case ITEMS_PARAMETER:
                        args.Result = _itemsEvaluated;
                        break;
                }
            }
        }

        private void _import_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        private void _rules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }


        private enum TokenType
        {
            None = 0,
            Identifier = 1,
            @String = 2,
            Calc = 3
        }


        private class CachedLineItem
        {
            public CachedLineItem()
            {
                TotalWeight = 0;
                LastSelect = 0;
                Rules = new List<LineItem>();
            }

            public CachedLineItem(int totalWeight, int lastSelect)
            {
                TotalWeight = totalWeight;
                LastSelect = lastSelect;
                Rules = new List<LineItem>();
            }

            public CachedLineItem(int totalWeight, int lastSelect, LineItem rule)
            {
                TotalWeight = totalWeight;
                LastSelect = lastSelect;
                Rules = new List<LineItem> {rule};
            }

            public CachedLineItem(int totalWeight, int lastSelect, List<LineItem> rules)
            {
                TotalWeight = totalWeight;
                LastSelect = lastSelect;
                Rules = rules;
            }

            public int TotalWeight { get; set; }
            public int LastSelect { get; set; }
            public List<LineItem> Rules { get; }
        }


        public class ItemFile : INotifyPropertyChanged, IDataErrorInfo
        {
            private string _fileName;
            private string _label;

            public string FileName
            {
                get { return _fileName; }
                set
                {
                    if (_fileName != value)
                    {
                        _fileName = value;
                        if (string.IsNullOrEmpty(Label))
                        {
                            Label = Path.GetFileNameWithoutExtension(value);
                        }
                        OnPropertyChanged();
                    }
                }
            }

            public string Label
            {
                get { return _label; }
                set
                {
                    if (_label != value)
                    {
                        _label = value;
                        OnPropertyChanged();
                    }
                }
            }

            public string Error
            {
                get { return string.Empty; }
            }

            public string this[string columnName]
            {
                get
                {
                    if (columnName == (HelperMethods.GetPropertyName<ItemFile, string>(i => i.FileName)))
                    {
                        if (!File.Exists(FileName))
                        {
                            return "File does not exist.";
                        }
                    }
                    else if (columnName == (HelperMethods.GetPropertyName<ItemFile, string>(i => i.Label)))
                    {
                        if (string.IsNullOrWhiteSpace(Label))
                        {
                            return "Label cannot be blank.";
                        }
                    }
                    return string.Empty;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private class SampleItem
        {
            public SampleItem(string previous, string next)
            {
                Previous = previous;
                Next = next;
            }

            public string Previous { get; }
            public string Next { get; }

            public int Weight { get; set; } = 1;
        }
    }
}