using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualBasic.FileIO;
using NCalc;
using YamlDotNet.Serialization;

namespace Grammars.Table
{
    public enum Actions
    {
        Random,
        @Loop,
        @Select
    }

    public class TableGrammarTable
    {
        private const string COMMENT_TOKEN = "#";
        private readonly Calculator _ncalc = new Calculator();

        private EventHandler<EvaluateTableFunctionArgs> EvaluateFunctionEvent;


        private EventHandler<EvaluateTableParameterArgs> EvaluateParameterEvent;

        [XmlAttribute("action")]
        public Actions Action { get; set; } = Actions.Random;

        [XmlAttribute("column")]
        public string Column { get; set; }

        [XmlAttribute("randomModifier")]
        public string RandomModifier { get; set; }

        [XmlAttribute("loopId")]
        public string LoopId { get; set; }

        [XmlAttribute("repeat")]
        public string Repeat { get; set; }

        [XmlAttribute("repeatJoin")]
        public string RepeatJoin { get; set; }

        [XmlAttribute("selectValue")]
        public string SelectValue { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("delimiter")]
        public string Delimiter { get; set; }

        [XmlAttribute("skipTable")]
        public string SkipTable { get; set; }

        [XmlText]
        public string Value { get; set; }

        [XmlIgnore]
        [YamlIgnore]
        public DataTable Table { get; private set; }

        public Dictionary<string, object> ProcessTable()
        {
            if (!string.IsNullOrWhiteSpace(SkipTable) && Convert.ToBoolean(_ncalc.Evaluate(SkipTable)))
            {
                return new Dictionary<string, object>();
            }
            switch (Action)
            {
                case Actions.Random:
                    return ProcessRandom();
                case Actions.Loop:
                    return ProcessLoop();
                case Actions.Select:
                    return ProcessSelect();
            }
            return new Dictionary<string, object>();
        }

        private Dictionary<string, object> ProcessSelect()
        {
            var value = SelectValue;
            var result = new Dictionary<string, object>();

            if (value.StartsWith("="))
            {
                value = _ncalc.Evaluate(value.Substring(1)).ToString();
            }

            ParseTable();

            var row = Table.AsEnumerable().FirstOrDefault(r => r[Column].Equals(value));

            ProcessRow(result, row);

            return result;
        }

        private Dictionary<string, object> ProcessLoop()
        {
            var result = new Dictionary<string, object>();
            var count = GetRepeat();

            ParseTable();

            for (var i = 1; i <= count; i++)
            {
                foreach (DataRow row in Table.Rows)
                {
                    var id = row[LoopId].ToString();
                    foreach (DataColumn column in Table.Columns)
                    {
                        var key = id + "." + column.ColumnName;
                        var expression = row[column.ColumnName].ToString();
                        object value;
                        if (expression.StartsWith("="))
                        {
                            value = _ncalc.Evaluate(expression.Substring(1));
                        }
                        else
                        {
                            value = expression;
                        }
                        if (result.ContainsKey(key))
                        {
                            result[key] = result[key] + RepeatJoin + value;
                        }
                        else
                        {
                            result.Add(key, value);
                        }
                    }
                }
            }

            return result;
        }

        private Dictionary<string, object> ProcessRandom()
        {
            if (Table == null)
            {
                ParseTable();
            }

            var max = Table.AsEnumerable().Max(r => Convert.ToInt32(r[Column]));
            var result = new Dictionary<string, object>();
            var modifier = 0;
            var count = GetRepeat();

            for (var i = 1; i <= count; i++)
            {
                DataRow selectedRow = null;
                var index = 0;

                if (!string.IsNullOrWhiteSpace(RandomModifier))
                {
                    if (RandomModifier.StartsWith("="))
                    {
                        modifier = Convert.ToInt32(_ncalc.Evaluate(RandomModifier.Substring(1)));
                    }
                    else
                    {
                        modifier = Convert.ToInt32(_ncalc.Evaluate(RandomModifier));
                    }
                }

                var value = BaseGrammar.Random.Next(Math.Abs(max + modifier))*(max + modifier < 0 ? -1 : 1);
                while (index < Table.Rows.Count && selectedRow == null)
                {
                    var row = Table.Rows[index];
                    if (value < Convert.ToInt32(row[Column]))
                    {
                        selectedRow = row;
                    }
                    index++;
                }

                if (selectedRow == null)
                {
                    selectedRow = Table.Rows[Table.Rows.Count - 1];
                }

                ProcessRow(result, selectedRow);
            }
            return result;
        }

        private void ProcessRow(Dictionary<string, object> result, DataRow row)
        {
            if (row != null)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    object thisValue;
                    if (row[column].ToString().StartsWith("="))
                    {
                        thisValue = _ncalc.Evaluate(row[column].ToString().Substring(1));
                    }
                    else
                    {
                        thisValue = row[column];
                    }

                    if (result.ContainsKey(column.ColumnName))
                    {
                        result[column.ColumnName] = result[column.ColumnName] + RepeatJoin + thisValue;
                    }
                    else
                    {
                        result.Add(column.ColumnName, thisValue);
                    }
                }
            }
        }

        private int GetRepeat()
        {
            if (string.IsNullOrWhiteSpace(Repeat))
            {
                return 1;
            }
            if (Repeat.StartsWith("="))
            {
                return Convert.ToInt32(_ncalc.Evaluate(Repeat.Substring(1)));
            }
            return int.Parse(Repeat);
        }

        private void ParseTable()
        {
            if (Table == null)
            {
                var reader = new StringReader(Value);
                var parser = new TextFieldParser(reader)
                {
                    Delimiters = new[] {Delimiter},
                    TextFieldType = FieldType.Delimited,
                    CommentTokens = new[] {COMMENT_TOKEN},
                    HasFieldsEnclosedInQuotes = false,
                    TrimWhiteSpace = true
                };
                Table = new DataTable {TableName = Name};

                // Read headers
                var headers = parser.ReadFields();
                foreach (var header in headers)
                {
                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        Table.Columns.Add(header);
                    }
                }

                // Read data
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    Table.Rows.Add(fields);
                }
            }
        }

        private void _ncalc_EvaluateFunction(string name, FunctionArgs args)
        {
            if (EvaluateFunctionEvent != null)
            {
                EvaluateFunctionEvent(this, new EvaluateTableFunctionArgs(name, args));
            }
        }

        private void _ncalc_EvaluateParameter(string name, ParameterArgs args)
        {
            if (EvaluateParameterEvent != null)
            {
                EvaluateParameterEvent(this, new EvaluateTableParameterArgs(name, args));
            }
        }

        public event EventHandler<EvaluateTableParameterArgs> EvaluateParameter
        {
            add
            {
                EvaluateParameterEvent =
                    (EventHandler<EvaluateTableParameterArgs>) Delegate.Combine(EvaluateParameterEvent, value);
            }
            remove
            {
                EvaluateParameterEvent =
                    (EventHandler<EvaluateTableParameterArgs>) Delegate.Remove(EvaluateParameterEvent, value);
            }
        }

        public event EventHandler<EvaluateTableFunctionArgs> EvaluateFunction
        {
            add
            {
                EvaluateFunctionEvent =
                    (EventHandler<EvaluateTableFunctionArgs>) Delegate.Combine(EvaluateFunctionEvent, value);
            }
            remove
            {
                EvaluateFunctionEvent =
                    (EventHandler<EvaluateTableFunctionArgs>) Delegate.Remove(EvaluateFunctionEvent, value);
            }
        }


        public class EvaluateTableParameterArgs : EventArgs
        {
            public EvaluateTableParameterArgs(string name, ParameterArgs args)
            {
                Name = name;
                Args = args;
            }

            public string Name { get; }

            public ParameterArgs Args { get; }
        }

        public class EvaluateTableFunctionArgs : EventArgs
        {
            public EvaluateTableFunctionArgs(string name, FunctionArgs args)
            {
                Name = name;
                Args = args;
            }

            public string Name { get; }

            public FunctionArgs Args { get; }
        }
    }
}