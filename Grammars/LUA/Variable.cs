using System.Xml.Serialization;

namespace Grammars.LUA
{
    public class Variable
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}