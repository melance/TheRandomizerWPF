using System;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using NLua;

namespace Grammars.LUA
{
    internal class Calculator
    {
        private readonly Grammars.Calculator _calculator;
        private readonly BaseGrammar _grammar;
        private readonly Lua _lua;

        public Calculator(LUAGrammar grammar, Lua lua)
        {
            _grammar = grammar;
            _lua = lua;
            _calculator = new Grammars.Calculator(_grammar);
        }

        public Grammars.Calculator.DieRollResult LastRollResult
        {
            get { return Grammars.Calculator.LastRollResult; }
        }

        public string ExpandPath(string path)
        {
            var value = Environment.ExpandEnvironmentVariables(path);
            if (!Path.IsPathRooted(value))
            {
                value = Path.Combine(Path.GetDirectoryName(_grammar.FilePath), value);
            }
            return value;
        }

        public int Roll(params object[] @params)
        {
            return Grammars.Calculator.DiceRoll(@params);
        }

        public int Rnd(int min, int max)
        {
            return BaseGrammar.Random.Next(min, max + 1);
        }

        public int TargetRoll(params object[] @params)
        {
            return Grammars.Calculator.TargetRoll(@params);
        }

        public string Generate(string grammarName, int maxLength)
        {
            return _calculator.Generate(grammarName, maxLength);
        }

        public string Generate(string grammarName, int maxLength, LuaTable parameters)
        {
            return _calculator.Generate(grammarName, maxLength, parameters);
        }

        public dynamic Evaluate(string expression)
        {
            return _calculator.Evaluate(expression);
        }

        public dynamic SelectFromTable(LuaTable items)
        {
            var maxRoll = items.Keys
                               .Cast<int>()
                               .Concat(new[] {0})
                               .Max();

            var index = BaseGrammar.Random.Next(1, maxRoll + 1);
            foreach (int item in items.Keys)
            {
                if (index <= item)
                {
                    return items[Convert.ToString(item)];
                }
            }

            return null;
        }

        public string FileToString(string filePath)
        {
            var path = filePath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Path.GetDirectoryName(_grammar.FilePath), filePath);
            }
            return File.ReadAllText(filePath);
        }

        public string[] FileToTable(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public LuaTable XMLFileToTable(string filePath)
        {
            var document = new XmlDocument();
            document.Load(filePath);

            var value = ProcessXMLNode(document.DocumentElement);

            return value;
        }

        private LuaTable ProcessXMLNode(XmlNode parent)
        {
            var value = CreateTable();
            var indexed = false;
            var index = 1;

            if (parent.Attributes["indexed"] != null)
            {
                indexed = bool.Parse(parent.Attributes["indexed"].Value);
            }

            foreach (XmlNode child in parent.ChildNodes)
            {
                if (child.HasChildNodes && child.FirstChild.NodeType == XmlNodeType.Element)
                {
                    if (indexed)
                    {
                        value[index] = ProcessXMLNode(child);
                        index++;
                    }
                    else
                    {
                        value[child.Name] = ProcessXMLNode(child);
                    }
                }
                else if (child.NodeType != XmlNodeType.Comment)
                {
                    if (indexed)
                    {
                        value[index] = child.InnerText;
                        index++;
                    }
                    else
                    {
                        value[child.Name] = child.InnerText;
                    }
                }
            }
            return value;
        }

        private LuaTable CreateTable()
        {
            return ((LuaTable) (_lua.DoString("return {}")[0]));
        }

        public LuaTable CSVToTable(string csvPath)
        {
            var parser = new TextFieldParser(csvPath)
            {
                TextFieldType = FieldType.Delimited,
                HasFieldsEnclosedInQuotes = true
            };
            parser.SetDelimiters(",");

            var header = parser.ReadFields();

            var value = CreateTable();
            for (var index = 1; !parser.EndOfData; index++)
            {
                var line = parser.ReadFields();
                var record = CreateTable();
                for (var i = 0; i <= line.Length - 1; i++)
                {
                    record[header[i]] = line[i];
                }
                value[index] = record;
            }

            return value;
        }
    }
}