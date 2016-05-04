using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualBasic;
using NCalc;
using NLua;
using Utility;

namespace Grammars
{
    internal class Calculator
    {
        // Function Names
        private const string GENERATE_FUNCTION = "Generate";
        private const string ROLL_FUNCTION = "Roll";
        private const string TARGET_ROLL_FUNCTION = "TargetRoll";
        private const string RANDOM_FUNCTION = "Rnd";
        private const string TO_TEXT_FUNCTION = "ToText";
        private const string TO_ORDINAL_FUNCTION = "ToOrdinal";
        private const string UCASE_FUNCTION = "UCase";
        private const string LCASE_FUNCTION = "LCase";
        private const string TCASE_FUNCTION = "TCase";
        private const string FORMAT_FUNCTION = "Format";
        private const string ROLL_RESULTS_FUNCTION = "RollResults";
        private const string PICK_FUNCTION = "Pick";
        private const string ROLLED_ONES_FUNCTION = "RolledOnes";

        // Special Dice
        private const string DIE_FATE = "F";
        private const string DIE_PERCENTILE = "%";

        // Exception Messages
        private const string PARAMETER_COUNT_EXCEPTION_MESSAGE =
            "Invalid number of parameters for the {0} function received {1} and was expecting {2}.";

        // Dice Roll Constants
        private const string DROP_LOWEST = "DL";
        private const string DROP_HIGHEST = "DH";
        private const string EXPLODE_DIE = "EX";
        private const string COMPOUND_EXPLODE = "CX";
        private const string REROLL_BELOW = "RB";
        private const string TARGET_GREATER = "GT";
        private const string TARGET_LESS = "LT";
        private const string TARGET_RULE_OF_ONE = "R1";
        private const string ROLL_SUCCESS = "<span style=\'color:Green\'>{0}</span>";
        private const string ROLL_ONE = "<span style=\'color:Red;\'>{0}</span>";
        private readonly BaseGrammar _grammar;


        private Expression _calc;


        private EvaluateFunctionHandler EvaluateFunctionEvent;

        private EvaluateParameterHandler EvaluateParameterEvent;


        public Calculator()
        {
        }

        public Calculator(BaseGrammar grammar)
        {
            _grammar = grammar;
        }


        public static DieRollResult LastRollResult { get; private set; }

        public event EvaluateFunctionHandler EvaluateFunction
        {
            add { EvaluateFunctionEvent = (EvaluateFunctionHandler) Delegate.Combine(EvaluateFunctionEvent, value); }
            remove { EvaluateFunctionEvent = (EvaluateFunctionHandler) Delegate.Remove(EvaluateFunctionEvent, value); }
        }

        public event EvaluateParameterHandler EvaluateParameter
        {
            add { EvaluateParameterEvent = (EvaluateParameterHandler) Delegate.Combine(EvaluateParameterEvent, value); }
            remove
            {
                EvaluateParameterEvent = (EvaluateParameterHandler) Delegate.Remove(EvaluateParameterEvent, value);
            }
        }


        public static int DiceRoll(object[] @params)
        {
            var count = Convert.ToInt32(@params[0]);
            var min = 1;
            var die = (@params[1]).ToString();
            var dropHigh = 0;
            var dropLow = 0;
            var explode = false;
            var compoundExplode = false;
            var rerollBelow = 0;
            var results = new List<int>();

            if (die.Equals(DIE_FATE, StringComparison.CurrentCultureIgnoreCase))
            {
                die = "1";
                min = -1;
            }
            else if (die.Equals(DIE_PERCENTILE))
            {
                die = "100";
            }

            LastRollResult = new DieRollResult();

            for (var i = 2; i <= @params.Length - 1; i++)
            {
                switch (@params[i].ToString())
                {
                    case DROP_HIGHEST:
                        if (i < @params.GetUpperBound(0) && @params[i + 1] is int)
                        {
                            dropHigh = Convert.ToInt32(@params[i + 1]);
                        }
                        else
                        {
                            dropHigh = 1;
                        }
                        break;
                    case DROP_LOWEST:
                        if (i < @params.GetUpperBound(0) && @params[i + 1] is int)
                        {
                            dropLow = Convert.ToInt32(@params[i + 1]);
                        }
                        else
                        {
                            dropLow = 1;
                        }
                        break;
                    case REROLL_BELOW:
                        if (i < @params.GetUpperBound(0) && @params[i + 1] is int)
                        {
                            rerollBelow = Convert.ToInt32(@params[i + 1]);
                        }
                        else
                        {
                            rerollBelow = 1;
                        }
                        break;
                    case EXPLODE_DIE:
                        explode = true;
                        break;
                    case COMPOUND_EXPLODE:
                        compoundExplode = true;
                        break;
                }
            }

            if (dropHigh + dropLow > count)
            {
                throw (new EvaluationException(
                    "Dice count to drop " + Convert.ToString(dropHigh + dropLow) +
                    " is greater than the number of dice rolled " + Convert.ToString(count)));
            }

            for (var i = 1; i <= count; i++)
            {
                var roll = RollDie(min, die.Val(), explode, compoundExplode);
                while (roll < rerollBelow)
                {
                    roll = RollDie(min, die.Val(), explode, compoundExplode);
                }
                LastRollResult.IndividualRolls.Add(roll.ToString());
                results.Add(roll);
            }

            for (var i = 1; i <= dropHigh; i++)
            {
                var index = results.IndexOf(results.Max());
                results.RemoveAt(index);
            }

            for (var i = 1; i <= dropLow; i++)
            {
                var index = results.IndexOf(results.Min());
                results.RemoveAt(index);
            }

            LastRollResult.Value = results.Sum();

            return LastRollResult.Value;
        }

        public static int TargetRoll(object[] @params)
        {
            var count = Convert.ToInt32(@params[0]);
            var die = Convert.ToInt32(@params[1]);
            var target = Convert.ToInt32(@params[2]);
            var ruleOfOne = false;
            var explode = false;
            var compoundExplode = false;
            var greaterThan = true;

            LastRollResult = new DieRollResult();

            for (var i = 3; i <= @params.Length - 1; i++)
            {
                switch (@params[i].ToString())
                {
                    case TARGET_RULE_OF_ONE:
                        ruleOfOne = true;
                        break;
                    case EXPLODE_DIE:
                        explode = true;
                        break;
                    case COMPOUND_EXPLODE:
                        compoundExplode = true;
                        break;
                    case TARGET_GREATER:
                        greaterThan = true;
                        break;
                    case TARGET_LESS:
                        greaterThan = false;
                        break;
                }
            }

            for (var i = 1; i <= count; i++)
            {
                DieRollResult roll;
                if (greaterThan)
                {
                    roll = RollTargetOverDie(
                        die,
                        target,
                        ruleOfOne,
                        explode,
                        compoundExplode);
                }
                else
                {
                    roll = RollTargetUnderDie(
                        die,
                        target,
                        explode,
                        compoundExplode);
                }
                LastRollResult += roll;
            }
            return LastRollResult.Value;
        }


        public dynamic Evaluate(string expression)
        {
            if (_calc != null)
            {
                _calc.EvaluateFunction -= _calc_EvaluateFunction;
                _calc.EvaluateParameter -= _calc_EvaluateParameter;
            }
            _calc = new Expression(expression, EvaluateOptions.NoCache);
            _calc.EvaluateFunction += _calc_EvaluateFunction;
            _calc.EvaluateParameter += _calc_EvaluateParameter;
            try
            {
                return _calc.Evaluate();
            }
            catch (EvaluationException ex)
            {
                ex.Data.Add("Expression", expression);
            }

            return null;
        }

        public string Generate(string grammarName, int maxLength)
        {
            return Generate(grammarName, maxLength, new Dictionary<string, string>());
        }

        public string Generate(string grammarName, int maxLength, LuaTable parameters)
        {
            return Generate(grammarName, maxLength, parameters.ToDictionary<string, string>());
        }

        public string Generate(string grammarName, int maxLength, Dictionary<string, string> parameters)
        {
            var name = Convert.ToString((Path.HasExtension(grammarName)) ? grammarName : grammarName + ".rnd.xml");
            var path = string.Empty;
            if (!Path.IsPathRooted(name))
            {
                var i = 0;

                if (!string.IsNullOrWhiteSpace(_grammar.FilePath))
                {
                    path = Path.Combine(Path.GetDirectoryName(_grammar.FilePath), name);
                }

                while (!File.Exists(path) && i < Utility.GrammarFilePaths.Count)
                {
                    path = Path.Combine(Convert.ToString(Utility.GrammarFilePaths[i]), name);
                }
            }

            var grammar = BaseGrammar.Open(path);

            foreach (var item in parameters)
            {
                grammar.Parameters.Add(new Parameter(Convert.ToString(item.Key), Convert.ToString(item.Value)));
            }

            var value = grammar.GenerateNames(1, maxLength, null);

            foreach (var variable in grammar.Variables)
            {
                _grammar.set_Variable(variable.Key, variable.Value);
            }

            return ExtractBody(value);
        }


        private static string ExtractBody(string html)
        {
            var document = new XmlDocument();
            document.LoadXml(html);

            var body = (XmlElement) (document.SelectSingleNode("/html/body/div"));

            return body.InnerXml;
        }

        private static int RollDie(int low, int high, bool explode, bool compoundExplode)
        {
            var result = BaseGrammar.Random.Next(low, high + 1);
            if (result == high)
            {
                if (explode)
                {
                    result += RollDie(low, high, false, false);
                }
                else if (compoundExplode)
                {
                    result += RollDie(low, high, false, true);
                }
            }
            return result;
        }

        private static DieRollResult RollTargetOverDie(
            int die,
            int target,
            bool ruleOfOne,
            bool explode,
            bool compoundExplode)
        {
            var result = new DieRollResult();
            var roll = RollDie(1, die, false, false);
            if (roll >= target)
            {
                result.IndividualRolls.Add(string.Format(ROLL_SUCCESS, roll));
                result.Value++;
                if (roll == die && explode)
                {
                    result += RollTargetOverDie(die, target, false, false, false);
                }
                else if (roll == die && compoundExplode)
                {
                    result += RollTargetOverDie(die, target, false, false, true);
                }
            }
            else if (roll == 1 && ruleOfOne)
            {
                result.IndividualRolls.Add(string.Format(ROLL_ONE, roll));
                result.Value--;
            }
            else
            {
                result.IndividualRolls.Add(roll.ToString());
            }
            return result;
        }

        private static DieRollResult RollTargetUnderDie(int die, int target, bool explode, bool compoundExplode)
        {
            var result = new DieRollResult();
            var roll = RollDie(1, die, false, false);
            if (roll <= target)
            {
                result.IndividualRolls.Add(string.Format(ROLL_SUCCESS, roll));
                result.Value++;
                if (roll == die && explode)
                {
                    result += RollTargetUnderDie(die, target, false, false);
                }
                else if (roll == die && compoundExplode)
                {
                    result += RollTargetUnderDie(die, target, false, true);
                }
            }
            else
            {
                result.IndividualRolls.Add(roll.ToString());
            }
            return result;
        }

        private static string GetRollResults(object[] @params)
        {
            var delimiter = " ";
            if (@params.Length > 0)
            {
                delimiter = @params[0].ToString();
            }
            return string.Join(delimiter, LastRollResult.IndividualRolls.ToArray());
        }

        private static int CountOnes()
        {
            if (LastRollResult != null)
            {
                return LastRollResult.IndividualRolls.Sum(r => r.Val() == 1 ? 1 : 0);
            }
            throw (new EvaluationException("RolledOnes can only be called after a Roll or TargetRoll call"));
        }


        private void _calc_EvaluateFunction(string name, FunctionArgs args)
        {
            switch (name)
            {
                case ROLL_FUNCTION:
                    if (args.Parameters.Length >= 2)
                    {
                        args.Result = DiceRoll(args.EvaluateParameters());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                ROLL_FUNCTION,
                                args.Parameters.Length,
                                "2 or more")));
                    }
                    break;
                case TARGET_ROLL_FUNCTION:
                    if (args.Parameters.Length >= 3)
                    {
                        args.Result = TargetRoll(args.EvaluateParameters());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                TARGET_ROLL_FUNCTION,
                                args.Parameters.Length,
                                "3 or more")));
                    }
                    break;
                case ROLL_RESULTS_FUNCTION:
                    if (args.Parameters.Length <= 1)
                    {
                        args.Result = GetRollResults(args.EvaluateParameters());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                ROLL_RESULTS_FUNCTION,
                                args.Parameters.Length,
                                "1 or less")));
                    }
                    break;
                case ROLLED_ONES_FUNCTION:
                    if (args.Parameters.Length == 0)
                    {
                        args.Result = CountOnes();
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                ROLL_RESULTS_FUNCTION,
                                args.Parameters.Length,
                                "0")));
                    }
                    break;
                case RANDOM_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        args.Result = BaseGrammar.Random.Next(Convert.ToInt32(args.Parameters[0].Evaluate())) + 1;
                    }
                    else if (args.Parameters.Length == 2)
                    {
                        args.Result = BaseGrammar.Random.Next(
                            Convert.ToInt32(args.Parameters[0].Evaluate()),
                            (Convert.ToInt32(args.Parameters[1].Evaluate())) + 1);
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                RANDOM_FUNCTION,
                                args.Parameters.Length,
                                "1 or 2")));
                    }
                    break;
                case GENERATE_FUNCTION:
                    if (_grammar != null)
                    {
                        if (args.Parameters.Length == 1)
                        {
                            args.Result = Generate((args.Parameters[0].Evaluate()).ToString(), int.MaxValue);
                        }
                        else if (args.Parameters.Length == 2)
                        {
                            args.Result = Generate(
                                (args.Parameters[0].Evaluate()).ToString(),
                                Convert.ToInt32(args.Parameters[1].Evaluate()));
                        }
                        else if (args.Parameters.Length >= 3)
                        {
                            var parameters = new Dictionary<string, string>();
                            for (var i = 2; i <= args.Parameters.Length - 1; i += 2)
                            {
                                parameters.Add(
                                    (args.Parameters[i].Evaluate()).ToString(),
                                    (args.Parameters[i + 1].Evaluate()).ToString());
                            }
                            args.Result = Generate(
                                (args.Parameters[0].Evaluate()).ToString(),
                                Convert.ToInt32(args.Parameters[1].Evaluate()),
                                parameters);
                        }
                        else
                        {
                            throw (new EvaluationException(
                                string.Format(
                                    PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                    GENERATE_FUNCTION,
                                    args.Parameters.Length,
                                    2)));
                        }
                    }
                    break;
                case TO_TEXT_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        args.Result = (Convert.ToInt32(args.Parameters[0].Evaluate())).ToText();
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                TO_TEXT_FUNCTION,
                                args.Parameters.Length,
                                1)));
                    }
                    break;
                case TO_ORDINAL_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        args.Result = (Convert.ToInt32(args.Parameters[0].Evaluate())).ToOrdinal();
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                TO_ORDINAL_FUNCTION,
                                args.Parameters.Length,
                                1)));
                    }
                    break;
                case UCASE_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        args.Result =
                            CultureInfo.CurrentCulture.TextInfo.ToUpper((args.Parameters[0].Evaluate()).ToString());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                UCASE_FUNCTION,
                                args.Parameters.Length,
                                1)));
                    }
                    break;
                case LCASE_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        args.Result =
                            CultureInfo.CurrentCulture.TextInfo.ToLower((args.Parameters[0].Evaluate()).ToString());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                LCASE_FUNCTION,
                                args.Parameters.Length,
                                1)));
                    }
                    break;
                case TCASE_FUNCTION:
                    if (args.Parameters.Length == 1)
                    {
                        args.Result =
                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase((args.Parameters[0].Evaluate()).ToString());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                TCASE_FUNCTION,
                                args.Parameters.Length,
                                1)));
                    }
                    break;
                case FORMAT_FUNCTION:
                    if (args.Parameters.Length == 2)
                    {
                        args.Result = Strings.Format(
                            args.Parameters[0].Evaluate(),
                            (args.Parameters[1].Evaluate()).ToString());
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                FORMAT_FUNCTION,
                                args.Parameters.Length,
                                2)));
                    }
                    break;
                case PICK_FUNCTION:
                    if (args.Parameters.Length >= 1)
                    {
                        args.Result = args.Parameters[BaseGrammar.Random.Next(0, args.Parameters.Length)].Evaluate();
                    }
                    else
                    {
                        throw (new EvaluationException(
                            string.Format(
                                PARAMETER_COUNT_EXCEPTION_MESSAGE,
                                FORMAT_FUNCTION,
                                args.Parameters.Length,
                                1)));
                    }
                    break;
                default:
                    if (EvaluateFunctionEvent != null)
                    {
                        EvaluateFunctionEvent(name, args);
                    }
                    break;
            }
        }

        private void _calc_EvaluateParameter(string name, ParameterArgs args)
        {
            if (_grammar != null && _grammar.ParameterExists(name))
            {
                args.Result = _grammar.get_Parameter(name);
            }
            else if (_grammar != null && _grammar.Variables.ContainsKey(name))
            {
                args.Result = _grammar.get_Variable(name);
            }
            else
            {
                switch (name)
                {
                    default:
                        if (EvaluateParameterEvent != null)
                        {
                            EvaluateParameterEvent(name, args);
                        }
                        break;
                }
            }
        }


        public class DieRollResult
        {
            public DieRollResult()
            {
                IndividualRolls = new List<string>();
            }

            public List<string> IndividualRolls { get; set; }
            public int Value { get; set; }

            public static DieRollResult operator +(DieRollResult l, DieRollResult r)
            {
                var result = l;
                result.IndividualRolls.AddRange(r.IndividualRolls);
                result.Value += r.Value;
                return result;
            }
        }
    }
}