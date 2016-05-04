using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace MRU
{
    public class MRUFileList : MenuItem
    {
        
        #region Classes
        public class FileSelectedEventArgs : EventArgs
        {
            private string _fileName;
            
            public FileSelectedEventArgs(string fileName)
            {
                _fileName = fileName;
            }

            public string FileName { get { return _fileName; } }
        }
        #endregion

        #region Constructors
        public MRUFileList(): base()
        {            
        }

        public MRUFileList(IStorage storage): base()
        {
            this.Storage = storage;
        }

        #endregion

        #region Events
        public event EventHandler<FileSelectedEventArgs> FileSelected;
        public event EventHandler MaxItemsChanged;
        #endregion

        #region Dependency Properties
        public static DependencyProperty StorageProperty = DependencyProperty.Register("Storage",
                                                                                       typeof(IStorage),
                                                                                       typeof(MRUFileList),
                                                                                       new PropertyMetadata(null, StoragePropertyChanged));
        #endregion

        #region Members
        private Int32 _maximumItems = 10;
        #endregion

        #region Properties
        public IStorage Storage { get; set; }
        public Int32 MaximumItems
        {
            get { return _maximumItems; }
            set 
            { 
                if (_maximumItems != value)
                {
                    _maximumItems = value;
                }
            }
        }
        #endregion
        
        #region Methods
        public void AddItem(string fileName)
        {
            MRUFileListItem item = GetItem(fileName);
            if (item != null) { Items.Remove(item); }
            item = new MRUFileListItem(fileName);
            Items.Insert(0, item);

            item.Click += mnuItem_Clicked;
            UpdateStorage();
        }

        public MRUFileListItem GetItem(string fileName)
        {
            Int32 index = -1;
            for (Int32 i = 0; i < Items.Count; i++)
            {
                MRUFileListItem current = (MRUFileListItem)Items[i];
                if (current.FileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    index = i;
                }
            }
            if (index >= 0 && index < Items.Count)
            {
                return (MRUFileListItem)Items[index];
            }
            return null;
        }

        public void RemoveItem(string fileName)
        {
            MRUFileListItem item = GetItem(fileName);
            if (item != null)
            {
                item.Click -= mnuItem_Clicked;
                Items.Remove(item);
            }
            UpdateStorage();
        }

        public void RemoveItem(Int32 index)
        {
            MRUFileListItem item = (MRUFileListItem)Items[index];
            item.Click -= mnuItem_Clicked;
            Items.Remove(item);
            UpdateStorage();
        }

        public void RemoveItem(MRUFileListItem item)
        {
            item.Click -= mnuItem_Clicked;
            Items.Remove(item);
            UpdateStorage();
        }
        
        protected virtual void OnFileSelected(string fileName)
        {
            FileSelected(this, new FileSelectedEventArgs(fileName));
        }

        protected virtual void OnMaxItemsChanged()
        {
            MaxItemsChanged(this, EventArgs.Empty);
            while (this.Items.Count > MaximumItems)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        }

        protected virtual void UpdateStorage()
        {
            List<string> fileList = new List<string>();
            foreach (MRUFileListItem item in Items)
            {
                fileList.Add(item.FileName);
            }
            Storage.WriteFileList(fileList);
        }
        #endregion  

        #region Event Handlers
        private static void StoragePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs  e)
        {
            MRUFileList instance = ((object)d) as MRUFileList;
            IEnumerable<string> fileList = instance.Storage.ReadFileList();
            instance.Items.Clear();
            foreach (string fileName in fileList)
            {
                instance.AddItem(fileName);
            }

        }

        private void mnuItem_Clicked(object sender, RoutedEventArgs e)
        {
            MRUFileListItem instance = (MRUFileListItem)sender;
            OnFileSelected(instance.FileName);
        }
        #endregion
        


    }
}
