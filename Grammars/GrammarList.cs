using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Grammars
{
    public class GrammarList : BindingList<GrammarListItem>, INotifyPropertyChanged
    {
        private readonly List<string> _paths;

        private readonly string[] GENERATOR_FILE_PATTERN =
        {
            "*.rnd.xml",
            "*.rnd.yaml"
        };

        private GrammarList _filteredList;
        private List<string> _selectedTags = new List<string>();

        private EventHandler<ProgressChangedEventArgs> ReportProgressEvent;

        public GrammarList()
        {
        }

        public GrammarList(List<string> paths)
        {
            _paths = paths;
        }


        public List<string> SelectedTags
        {
            get { return _selectedTags; }
            set
            {
                _selectedTags = value;
                FilterList();
                OnPropertyChanged("SelectedTags");
            }
        }

        public GrammarList FilteredList
        {
            get { return _filteredList; }
            set
            {
                _filteredList = value;
                OnPropertyChanged("FilteredList");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ProgressChangedEventArgs> ReportProgress
        {
            add
            {
                ReportProgressEvent =
                    (EventHandler<ProgressChangedEventArgs>) Delegate.Combine(ReportProgressEvent, value);
            }
            remove
            {
                ReportProgressEvent =
                    (EventHandler<ProgressChangedEventArgs>) Delegate.Remove(ReportProgressEvent, value);
            }
        }

        public Dictionary<string, Exception> Load()
        {
            var errorList = new Dictionary<string, Exception>();
            var count = 0;
            var done = 0;
            var fileList = new List<string>();

            foreach (var path in _paths)
            {
                foreach (var pattern in GENERATOR_FILE_PATTERN)
                {
                    if (Directory.Exists(path))
                    {
                        fileList.AddRange(Directory.GetFiles(path, pattern));
                    }
                }
            }

            count = fileList.Count;

            foreach (var filename in fileList)
            {
                try
                {
                    done++;
                    var grammar = BaseGrammar.Open(filename);
                    if (ReportProgressEvent != null)
                    {
                        ReportProgressEvent(
                            this,
                            new ProgressChangedEventArgs((int) (((double) done/count)*100), grammar.Name));
                    }
                    if (grammar.Visible)
                    {
                        var info = new GrammarListItem(
                            grammar.Name,
                            grammar.Description,
                            filename,
                            grammar.Tags);
                        Add(info);
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(Path.GetFileName(filename), ex);
                }
            }

            return errorList;
        }

        private void FilterList()
        {
            var result = new GrammarList();
            var sortedList = new GrammarList();
            result.AddRange(Items.Where(g => g.Tags.Any(SelectedTags.Contains)));
            sortedList.AddRange(result.OrderBy(g => g.Name));
            FilteredList = sortedList;
        }

        public void AddRange(IEnumerable<GrammarListItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}