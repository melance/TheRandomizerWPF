using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using NCalc;

namespace Grammars.Dice
{
    [XmlRoot("DiceRoller", IsNullable = false)]
    public class DiceRoll : BaseGrammar
    {
        private const string ASSIGN_OPERATOR = ":=";
        private const string ROLL_FUNCTION_PARAMETER = "RollFunction";
        private const string COMMENT_CHARACTER = "#";

        private Calculator _calculator;

        private Dictionary<int, string> _results = new Dictionary<int, string>();

        public DiceRoll()
        {
            _calculator = new Calculator(this);
            _calculator.EvaluateParameter += _calculator_EvaluateParameter;
            RollFunctions = new ObservableCollection<RollFunction>();
        }

        [XmlElement("function")]
        public ObservableCollection<RollFunction> RollFunctions { get; set; }

        [XmlElement("supportsMaxLength")]
        public override bool SupportsMaxLength
        {
            get { return false; }
            set { }
        }

        public override string Analyze()
        {
            return string.Empty;
        }

        protected override string GenerateName()
        {
            var lines = new List<string>(Strings.Split(GetRollFunction(), "\n"));
            var lineNumber = 1;

            _results.Clear();

            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(COMMENT_CHARACTER))
                {
                    var parts = line.Split(new[] {ASSIGN_OPERATOR}, StringSplitOptions.None);
                    if (parts.Length == 1)
                    {
                        _results.Add(lineNumber, _calculator.Evaluate(line).ToString());
                    }
                    else
                    {
                        if (GetParameter(parts[0]) != null)
                        {
                            GetParameter(parts[0]).Value = parts[1];
                        }
                        else
                        {
                            if (!Variables.ContainsKey(parts[0]))
                            {
                                Variables.Add(parts[0], 0);
                            }
                            Variables[parts[0]] = _calculator.Evaluate(parts[1]);
                        }
                    }
                }
                else
                {
                    _results.Add(lineNumber, string.Empty);
                }
                lineNumber++;
            }

            return string.Join(Environment.NewLine, _results.Values.ToArray());
        }

        private string GetRollFunction()
        {
            if (RollFunctions.Count == 1)
            {
                return RollFunctions[0].Value;
            }
            var rollFunctionParameter = GetParameter(ROLL_FUNCTION_PARAMETER);
            if (rollFunctionParameter != null)
            {
                var name = rollFunctionParameter.Value;
                if (name == null)
                {
                    return RollFunctions[0].Value;
                }
                return RollFunctions.First(rf => rf.Name == (name).ToString()).Value;
            }
            return string.Empty;
        }

        private string GetRollResults(object[] @params)
        {
            var delimiter = " ";
            if (@params.Length > 0)
            {
                delimiter = @params[0].ToString();
            }
            return string.Join(delimiter, Calculator.LastRollResult.IndividualRolls.ToArray());
        }

        private void _calculator_EvaluateParameter(string name, ParameterArgs args)
        {
            if (Variables.ContainsKey(name))
            {
                args.Result = Variables[name];
            }
        }

        public class RollFunction : INotifyPropertyChanged, IDataErrorInfo
        {
            private string _name;
            private string _value;

            [XmlAttribute("name")]
            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged();
                    }
                }
            }

            [XmlText]
            public string Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;
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
                    switch (columnName)
                    {
                        case "Name":
                            if (string.IsNullOrWhiteSpace(Name))
                            {
                                return "Name is required.";
                            }
                            break;
                        case "Value":
                            if (string.IsNullOrWhiteSpace(Value))
                            {
                                return "Function is required.";
                            }
                            break;
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
    }
}