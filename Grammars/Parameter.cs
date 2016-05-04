using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace Grammars
{
    public enum DataTypes
    {
        List,
        Text,
        CheckBox
    }

    public class Parameter : INotifyPropertyChanged
    {
        public enum ValueTypes
        {
            Value,
            Label
        }

        public Parameter()
        {
            Options = new ObservableCollection<Option>();
        }

        public Parameter(string name, string value) : this()
        {
            Name = name;
            Value = value;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("display")]
        public string Display { get; set; }

        [XmlText]
        public string Description { get; set; }

        [XmlAttribute("type")]
        public DataTypes Type { get; set; } = DataTypes.List;

        [XmlAttribute("valueType")]
        public ValueTypes ValueType { get; set; } = ValueTypes.Label;

        [XmlElement("option")]
        public ObservableCollection<Option> Options { get; set; }

        [XmlAttribute("default")]
        public string Default { get; set; }

        [XmlIgnore]
        [YamlIgnore]
        public string Value { get; set; }

        [XmlIgnore]
        [YamlIgnore]
        public DockPanel Control
        {
            get
            {
                Control value = null;
                var label = new Label();
                var panel = new DockPanel();
                switch (Type)
                {
                    case DataTypes.List:
                        var cboValue = new ComboBox();
                        foreach (var option in Options)
                        {
                            if (string.IsNullOrWhiteSpace(option.Display))
                            {
                                option.Display = option.Value;
                            }
                            var index = cboValue.Items.Add(option);
                            if (option.Value == Default)
                            {
                                cboValue.SelectedIndex = index;
                            }
                            if (cboValue.SelectedIndex < 0)
                            {
                                cboValue.SelectedIndex = 0;
                            }
                        }
                        cboValue.DisplayMemberPath = "Display";
                        value = cboValue;
                        break;
                    case DataTypes.Text:
                        var txtvalue = new TextBox();
                        txtvalue.Text = Default;
                        value = txtvalue;
                        break;
                    case DataTypes.CheckBox:
                        var chkValue = new CheckBox
                        {
                            IsChecked = Default != null && (bool.Parse(Default))
                        };
                        label.Visibility = Visibility.Collapsed;
                        chkValue.Content = Display;
                        value = chkValue;
                        break;
                }
                value.Tag = Name;
                value.ToolTip = Description;
                value.Margin = new Thickness(3, 3, 3, 3);
                label.Margin = new Thickness(3, -3, 3, 0);
                label.Content = Display;
                label.Height = value.Height;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.ToolTip = Description;
                DockPanel.SetDock(label, Dock.Left);
                DockPanel.SetDock(value, Dock.Right);
                panel.Children.Add(label);
                panel.Children.Add(value);
                panel.LastChildFill = true;
                panel.Margin = new Thickness(0, 3, 0, 3);
                DockPanel.SetDock(panel, Dock.Top);
                return panel;
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

        public class OptionClass
        {
            [XmlAttribute("display")]
            public string Display { get; set; }

            [XmlText]
            public string Value { get; set; }
        }
    }

    public class Option
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("display")]
        public string Display { get; set; }
    }
}