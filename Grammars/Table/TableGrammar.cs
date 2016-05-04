using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using NCalc;
using Utility;

namespace Grammars.Table
{
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class TableGrammar : BaseGrammar
    {
        private const string CALCULATION_REGEX = "\\[=.*?\\]";
        private const char VARIABLE_OPEN = '[';
        private const char VARIABLE_CLOSE = ']';
        private const string CALCULATION_OPEN = "[=";

        private Calculator _calculator;


        [XmlArray("tables")]
        [XmlArrayItem("table", IsNullable = false)]
        private ObservableCollection<TableGrammarTable> _Tables =
            new ObservableCollection<TableGrammarTable>();


        private Dictionary<string, object> _valueList = new Dictionary<string, object>();

        public TableGrammar()
        {
            _calculator = new Calculator(this);
            _calculator.EvaluateParameter += _calculator_EvaluateParameter;
        }

        private ObservableCollection<TableGrammarTable> Tables
        {
            get { return _Tables; }
            set { _Tables = value; }
        }

        [XmlElement("output")]
        public string Output { get; set; }


        public override string Analyze()
        {
            return string.Empty;
        }


        protected override string GenerateName()
        {
            _valueList = new Dictionary<string, object>();
            foreach (var table in Tables)
            {
                table.EvaluateParameter += Tables_EvaluateParameter;
                table.EvaluateFunction += Tables_EvaluteFunction;
                var newResults = table.ProcessTable();
                foreach (var result in newResults)
                {
                    _valueList.Add(table.Name + "." + result.Key, result.Value);
                }
                table.EvaluateParameter -= Tables_EvaluateParameter;
                table.EvaluateFunction -= Tables_EvaluteFunction;
            }
            return FillOutput();
        }


        private string FillOutput()
        {
            var result = Output;

            foreach (var pair in _valueList)
            {
                result = result.Replace(
                    VARIABLE_OPEN + pair.Key + Convert.ToString(VARIABLE_CLOSE),
                    pair.Value.ToString());
            }

            var index = result.IndexOf(CALCULATION_OPEN);

            while (index >= 0)
            {
                var length = 2;
                var expression = string.Empty;
                if (index >= 0)
                {
                    var parenthesis = 1;
                    while (parenthesis > 0)
                    {
                        var res = result.Substring(index + length, 1);

                        if (res == VARIABLE_OPEN.ToString())
                        {
                            parenthesis++;
                        }
                        else if (res == VARIABLE_CLOSE.ToString())
                        {
                            parenthesis--;
                        }
                        if (parenthesis != 0)
                        {
                            length++;
                        }
                    }
                    expression = result.Substring(index + 2, length - 2);
                }
                result = result.Stuff(index, length + 1, (string) (_calculator.Evaluate(expression)).ToString());
                index = result.IndexOf(CALCULATION_OPEN);
            }

            return result;
        }


        private void Tables_EvaluateParameter(object sender, TableGrammarTable.EvaluateTableParameterArgs args)
        {
            var key = args.Name;

            if (!key.Contains("."))
            {
                var tableName = ((TableGrammarTable) sender).Name;
                key = tableName + "." + args.Name;
            }

            if (_valueList.ContainsKey(key))
            {
                args.Args.Result = _valueList[key];
            }
            else if (ParameterExists(args.Name))
            {
                args.Args.Result = get_Parameter(args.Name);
            }
        }

        private void Tables_EvaluteFunction(object sender, TableGrammarTable.EvaluateTableFunctionArgs e)
        {
        }

        private void _calculator_EvaluateParameter(string name, ParameterArgs args)
        {
            if (_valueList.ContainsKey(name))
            {
                args.Result = _valueList[name];
            }
        }
    }
}