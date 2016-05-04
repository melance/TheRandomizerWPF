using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Grammars.Assignment
{
    [XmlType(AnonymousType = true)]
    [XmlRoot("library", Namespace = null)]
    public class Library
    {
        [XmlElement("item")]
        public ObservableCollection<LineItem> Rules { get; } = new ObservableCollection<LineItem>();


        public override string ToString()
        {
            var deserializer = new XmlSerializer(typeof (Library));
            using (var stream = new MemoryStream())
            {
                deserializer.Serialize(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }


        public static Library FromFile(string fileName)
        {
            var serializer = new XmlSerializer(typeof (Library));
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                return ((Library) (serializer.Deserialize(stream)));
            }
        }

        public static Library ItemListConverter(
            IEnumerable<AssignmentGrammar.ItemFile> listFiles,
            int weightAdjustment,
            bool removeDuplicates,
            bool caseSensitive)
        {
            var value = new Library();
            var rules = AssignmentGrammar.CreateRules(
                listFiles,
                weightAdjustment,
                removeDuplicates,
                caseSensitive);

            foreach (var rule in rules)
            {
                value.Rules.Add(rule);
            }

            return value;
        }
    }
}