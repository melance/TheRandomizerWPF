using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using Utility;
using YamlDotNet.Serialization;
using Color = System.Windows.Media.Color;

namespace Grammars
{
    [XmlRoot("Grammar", Namespace = "")]
    public abstract class BaseGrammar : INotifyPropertyChanged, IDataErrorInfo
    {
        private const string HTML_COLOR_FORMAT = "#{0:X2}{1:X2}{2:X2}";

        private const string XML_EXTENSION = ".xml";
        private const string YAML_EXTENSION = ".yaml";
        private const string YML_EXTENSION = ".yml";


        private static List<Type> _knownTypes;


        private static Random _random;
        private string _author;
        private bool _cancel;
        private string _category;
        private string _description;
        private string _genre;
        private string _name;
        private ObservableCollection<Parameter> _parameters = new ObservableCollection<Parameter>();
        private bool _skipValidation;
        private bool _supportsMaxLength;
        private string _system;
        private BindingList<string> _tags = new BindingList<string>();
        private string _url;
        private double? _version;
        private Color DEFAULT_DIVIDER_COLOR = Colors.Black;
        private Color DEFAULT_EVEN_COLOR = Color.FromArgb(255, 239, 239, 239);


        private Color DEFAULT_ODD_COLOR = Colors.White;
        private Font DEFAULT_RESULT_FONT = new Font("Consolas", 12);


        private EventHandler<ProgressUpdateEventArgs> ProgressUpdateEvent;


        protected BaseGrammar()
        {
            OddResultColor = DEFAULT_ODD_COLOR;
            EvenResultColor = DEFAULT_EVEN_COLOR;
            DividerColor = DEFAULT_DIVIDER_COLOR;
            ResultFont = DEFAULT_RESULT_FONT;
            Variables = new Dictionary<string, object>();
        }


        public static Type[] KnownTypes
        {
            get
            {
                if (_knownTypes == null)
                {
                    _knownTypes =
                        new List<Type>(
                            Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => typeof (BaseGrammar).IsAssignableFrom(t))
                                    .ToArray());
                }
                return _knownTypes.ToArray();
            }
        }


        [YamlIgnore]
        [XmlIgnore]
        public string FilePath { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public string FileDirectory
        {
            get { return Path.GetDirectoryName(FilePath); }
        }

        [YamlIgnore]
        [XmlIgnore]
        public string FileName
        {
            get { return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(FilePath)); }
        }

        [YamlIgnore]
        [XmlIgnore]
        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    VBMath.Randomize();
                    _random = new Random();
                }
                return _random;
            }
        }

        [XmlElement("name")]
        [YamlMember(Alias = "name")]
        public virtual string Name
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

        [XmlElement("version", IsNullable = true)]
        [YamlMember(Alias = "version")]
        public virtual double? Version
        {
            get
            {
                if (_version == null)
                {
                    return 1;
                }
                return _version;
            }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        [XmlElement("author")]
        [YamlMember(Alias = "author")]
        public virtual string Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlElement("description")]
        [YamlMember(Alias = "description")]
        public virtual string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        [YamlIgnore]
        [XmlElement("category")]
        public virtual string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    Tags.Add(_category);
                    OnPropertyChanged();
                    OnPropertyChanged("Genre");
                    OnPropertyChanged("System");
                    OnPropertyChanged("Tags");
                }
            }
        }

        [YamlIgnore]
        [XmlElement("genre")]
        public virtual string Genre
        {
            get { return _genre; }
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    Tags.Add(_genre);
                    OnPropertyChanged();
                    OnPropertyChanged("Category");
                    OnPropertyChanged("System");
                    OnPropertyChanged("Tags");
                }
            }
        }

        [YamlIgnore]
        [XmlElement("system")]
        public virtual string System
        {
            get { return _system; }
            set
            {
                if (_system != value)
                {
                    _system = value;
                    Tags.Add(_system);
                    OnPropertyChanged();
                    OnPropertyChanged("Genre");
                    OnPropertyChanged("Category");
                    OnPropertyChanged("Tags");
                }
            }
        }

        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        public virtual BindingList<string> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        [YamlIgnore]
        [XmlIgnore]
        public string TagList
        {
            get { return string.Join(", ", Tags.ToArray()); }
        }

        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        [YamlMember(Alias = "parameters")]
        public virtual ObservableCollection<Parameter> Parameters
        {
            get { return _parameters; }
            set
            {
                if (!_parameters.Equals(value))
                {
                    _parameters = value;
                    _parameters.CollectionChanged += _parameters_CollectionChanged;
                    OnPropertyChanged();
                }
            }
        }

        [XmlElement("supportsMaxLength")]
        [YamlMember(Alias = "supportsMaxLength")]
        public virtual bool SupportsMaxLength
        {
            get { return _supportsMaxLength; }
            set
            {
                if (_supportsMaxLength != value)
                {
                    _supportsMaxLength = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlElement("url")]
        [YamlMember(Alias = "url")]
        public virtual string URL
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual bool Visible { get; set; } = true;

        [YamlIgnore]
        [XmlIgnore]
        public virtual Color OddResultColor { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public virtual Color EvenResultColor { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public virtual Color DividerColor { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public virtual Font ResultFont { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public virtual string CSS { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public virtual bool OddResultColorSpecified
        {
            get { return OddResultColor != DEFAULT_ODD_COLOR; }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual bool EvenResultColorSpecified
        {
            get { return EvenResultColor != DEFAULT_EVEN_COLOR; }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual bool DividerColorSpecified
        {
            get { return DividerColor != DEFAULT_DIVIDER_COLOR; }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual bool ResultFontSpecified
        {
            get { return !ResultFont.Equals(DEFAULT_RESULT_FONT); }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual bool CSSSpecified
        {
            get { return !string.IsNullOrWhiteSpace(CSS); }
        }

        [YamlIgnore]
        [XmlIgnore]
        public bool IsDirty { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public virtual Dictionary<string, object> Variables { get; set; }

        [YamlIgnore]
        [XmlIgnore]
        public bool CategorySpecified
        {
            get { return Category != null; }
        }

        [YamlIgnore]
        [XmlIgnore]
        public bool GenreSpecified
        {
            get { return Genre != null; }
        }

        [YamlIgnore]
        [XmlIgnore]
        public bool SystemSpecified
        {
            get { return System != null; }
        }

        protected int MaxLength { get; set; } = int.MaxValue;

        [YamlIgnore]
        [XmlIgnore]
        private bool FilterComponentsInError
        {
            get
            {
                return string.IsNullOrWhiteSpace(Genre) &&
                       string.IsNullOrWhiteSpace(System) &&
                       string.IsNullOrWhiteSpace(Category);
            }
        }

        [YamlIgnore]
        [XmlIgnore]
        public bool SkipValidation
        {
            get { return _skipValidation; }
            set
            {
                if (_skipValidation != value)
                {
                    _skipValidation = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Name");
                    OnPropertyChanged("Description");
                    OnPropertyChanged("Genre");
                    OnPropertyChanged("System");
                    OnPropertyChanged("Category");
                }
            }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual string Error
        {
            get { return string.Empty; }
        }

        [YamlIgnore]
        [XmlIgnore]
        public virtual string this[string columnName]
        {
            get
            {
                if (SkipValidation)
                {
                    return string.Empty;
                }
                if (columnName == (HelperMethods.GetPropertyName<BaseGrammar, string>(bg => bg.Name)))
                {
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        return "Name is required.";
                    }
                }
                else if (columnName == (HelperMethods.GetPropertyName<BaseGrammar, string>(bg => bg.Description)))
                {
                    if (string.IsNullOrWhiteSpace(Description))
                    {
                        return "Description is required.";
                    }
                }
                else if (
                    columnName == (HelperMethods.GetPropertyName<BaseGrammar, string>(bg => bg.Genre))
                    || columnName == (HelperMethods.GetPropertyName<BaseGrammar, string>(bg => bg.System))
                    || columnName == (HelperMethods.GetPropertyName<BaseGrammar, string>(bg => bg.Category)))
                {
                    if (FilterComponentsInError)
                    {
                        return "At least one of Genre, System or Category must exist.";
                    }
                }
                return string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private static string ColorToHTML(Color color)
        {
            return string.Format(HTML_COLOR_FORMAT, color.R, color.G, color.B);
        }


        private void _parameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate
        {
            add
            {
                ProgressUpdateEvent =
                    (EventHandler<ProgressUpdateEventArgs>) Delegate.Combine(ProgressUpdateEvent, value);
            }
            remove
            {
                ProgressUpdateEvent =
                    (EventHandler<ProgressUpdateEventArgs>) Delegate.Remove(ProgressUpdateEvent, value);
            }
        }


        public static BaseGrammar Open(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case XML_EXTENSION:
                    return OpenXML(fileName);
                case YAML_EXTENSION:
                case YML_EXTENSION:
                    return OpenYAML(fileName);
                default:
                    throw new NotSupportedException();
            }
        }

        public static BaseGrammar OpenXML(string fileName)
        {
            var deserializer = new XmlSerializer(typeof (BaseGrammar), KnownTypes);
            using (var reader = File.OpenText(fileName))
            {
                var grammar = (BaseGrammar) (deserializer.Deserialize(reader));
                grammar.FilePath = fileName;
                grammar.IsDirty = false;
                return grammar;
            }
        }

        public static BaseGrammar OpenYAML(string fileName)
        {
            var deserializer = new Deserializer(null, null);
            using (TextReader reader = File.OpenText(fileName))
            {
                var grammar = deserializer.Deserialize<BaseGrammar>(reader);
                grammar.FilePath = fileName;
                grammar.IsDirty = false;
                return grammar;
            }
        }

        public static void ConvertXMLToYAML(string sourcefileName, string targetFileName)
        {
            var grammar = OpenXML(sourcefileName);
            var serializer = new Serializer(SerializationOptions.Roundtrip, null);
            using (TextWriter output = File.CreateText(targetFileName))
            {
                serializer.Serialize(output, grammar);
            }
        }

        public dynamic get_Variable(string name)
        {
            if (!Variables.ContainsKey(name))
            {
                Variables.Add(name, string.Empty);
            }
            return Variables[name];
        }

        public void set_Variable(string name, object value)
        {
            if (!Variables.ContainsKey(name))
            {
                Variables.Add(name, value);
            }
            else
            {
                Variables[name] = value;
            }
        }

        public dynamic get_Parameter(string name)
        {
            object value = Parameters.FirstOrDefault(p => p.Name.Equals(name)).Value;
            return value;
        }


        public string GenerateNames(int count, int maxLength, Dictionary<string, object> parameters)
        {
            var value = new List<string>();
            MaxLength = maxLength;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var param = Parameters.FirstOrDefault(p => p.Name.Equals(parameter.Key));
                    param.Value = (parameter.Value).ToString();
                }
            }
            for (var i = 0; i <= count - 1; i++)
            {
                if (_cancel)
                {
                    _cancel = false;
                    if (value != null && value.Count > 0)
                    {
                        return FormatResults(value);
                    }
                    return string.Empty;
                }
                value.Add(GenerateName());
                if (ProgressUpdateEvent != null)
                {
                    ProgressUpdateEvent(this, new ProgressUpdateEventArgs(i + 1));
                }
            }
            return FormatResults(value);
        }

        public void Cancel()
        {
            _cancel = true;
        }

        public virtual void Serialize(string fileName)
        {
            var knownTypes =
                Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => typeof (BaseGrammar).IsAssignableFrom(t))
                        .ToArray();
            var serializer = new XmlSerializer(GetType(), knownTypes);
            using (var file = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(file, this);
            }
        }

        public override string ToString()
        {
            var knownTypes =
                Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => typeof (BaseGrammar).IsAssignableFrom(t))
                        .ToArray();
            var serializer = new XmlSerializer(typeof (BaseGrammar), knownTypes);
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public virtual Parameter GetParameter(string name)
        {
            return Parameters.FirstOrDefault(p => p.Name.Equals(name));
        }

        public virtual bool ParameterExists(string name)
        {
            return Parameters.FirstOrDefault(p => p.Name.Equals(name)) != null;
        }

        public abstract string Analyze();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            IsDirty = true;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected abstract string GenerateName();

        protected virtual void Initialize()
        {
        }

        protected virtual string FormatResults(List<string> values)
        {
            const string DIV_OPEN = "<div class=\'result {0}\'>";
            const string NEW_LINE = "<br />";
            const string DIV_CLOSE = "</div>";
            const string BODY_PARAMETER = "{Body}";
            const string FONT_FAMILY_PARAMETER = "{FontFamily}";
            const string FONT_SIZE_PARAMETER = "{FontSize}";
            const string ODD_COLOR_PARAMETER = "{OddColor}";
            const string EVEN_COLOR_PARAMETER = "{EvenColor}";
            const string DIVIDER_COLOR_PARAMETER = "{DividerColor}";
            const string CSS_PARAMETER = "{CSS}";
            const string HTML_DIV_ODD = "odd";
            const string HTML_DIV_EVEN = "even";

            var results = new StringBuilder();
            var odd = true;

            foreach (var value in values)
            {
                results.AppendFormat(DIV_OPEN, odd ? HTML_DIV_ODD : HTML_DIV_EVEN);
                results.Append(value.Replace(Environment.NewLine, NEW_LINE));
                if (value.Contains(Environment.NewLine))
                {
                    results.AppendLine(NEW_LINE);
                }
                results.Append(DIV_CLOSE);
                odd = !odd;
            }

            var html = Resources.ResultsTemplate.Replace(BODY_PARAMETER, results.ToString());
            html = html.Replace(CSS_PARAMETER, CSS);
            html = html.Replace(ODD_COLOR_PARAMETER, ColorToHTML(OddResultColor));
            html = html.Replace(EVEN_COLOR_PARAMETER, ColorToHTML(EvenResultColor));
            html = html.Replace(DIVIDER_COLOR_PARAMETER, ColorToHTML(DividerColor));
            html = html.Replace(FONT_FAMILY_PARAMETER, ResultFont.FontFamily.Name);
            html = html.Replace(FONT_SIZE_PARAMETER, ResultFont.Size.ToString(CultureInfo.InvariantCulture));

            return html;
        }


        public class ProgressUpdateEventArgs : EventArgs
        {
            public ProgressUpdateEventArgs(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}