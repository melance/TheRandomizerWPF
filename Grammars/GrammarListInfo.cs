using System.ComponentModel;

namespace Grammars
{
    public class GrammarListItem
    {
        public GrammarListItem(string name, string description, string filepath, BindingList<string> tags)
        {
            Name = name;
            Description = description;
            FilePath = filepath;
            Tags = tags;
        }

        public string Name { get; }

        public string Description { get; }

        public string FilePath { get; }

        public BindingList<string> Tags { get; }

        public BaseGrammar Grammar()
        {
            return BaseGrammar.Open(FilePath);
        }
    }
}