using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace Grammars.Assignment
{
    public class MarkovChainGenerator
    {
        private readonly IEnumerable<string> _samples;
        private readonly int _syllableLength;
        private readonly int _weightLimit;

        private readonly string START_LABEL = "Start";
        private AssignmentGrammar _grammar;

        public MarkovChainGenerator(IEnumerable<string> samples, int syllableLength, int weightLimit)
        {
            _samples = samples;
            _syllableLength = syllableLength;
            _weightLimit = weightLimit;
        }

        public AssignmentGrammar BuildGrammar()
        {
            var list = new List<KeyValuePair<string, string>>();
            _grammar = new AssignmentGrammar();

            foreach (var s in _samples)
            {
                var sample = s;

                var i = 0;
                var diff = sample.Length%_syllableLength;
                var previous = START_LABEL;

                if (diff != 0)
                {
                    sample += Strings.Space(diff);
                }
                while (i < sample.Length)
                {
                    var current = sample.Substring(i, _syllableLength).Trim();
                    list.Add(new KeyValuePair<string, string>(previous, current));
                    if (i == sample.Length - _syllableLength)
                    {
                        list.Add(new KeyValuePair<string, string>(current, string.Empty));
                    }
                    previous = current.Trim();
                    i += _syllableLength;
                }
            }

            foreach (var pair in list)
            {
                var rule = _grammar.Rules.FirstOrDefault(
                    r =>
                    {
                        if (r.Name == ((string.IsNullOrWhiteSpace(pair.Key)) ? "Start" : pair.Key) &&
                            ((string.IsNullOrEmpty(r.Next))
                                ? (r.Expression == string.Format("[{0}]", pair.Value))
                                : r.Next == pair.Value))
                        {
                            return true;
                        }
                        return false;
                    });
                if (rule != null)
                {
                    if (_weightLimit == 0 || rule.Weight < _weightLimit)
                    {
                        rule.Weight++;
                    }
                }
                else
                {
                    rule = new LineItem(
                        pair.Key,
                        pair.Key == START_LABEL ? string.Empty : pair.Value,
                        pair.Key == START_LABEL ? string.Format("[{0}]", pair.Value) : pair.Key);
                    rule.Weight = 1;
                    _grammar.Rules.Add(rule);
                }
            }

            return _grammar;
        }
    }
}