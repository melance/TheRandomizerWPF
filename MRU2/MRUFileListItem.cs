using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace MRU
{
    public class MRUFileListItem : MenuItem
    {

        public MRUFileListItem(string fileName)
        {
            this.FileName = fileName;
        }

        private Int32 _maxLength = 50;
        private string _fileName;

        public Int32 MaxLength { get { return _maxLength; } }
        public string FileName 
        { 
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    Header = ShortenPath(_fileName, MaxLength);
                }
            }
        }
        
        public static string ShortenPath(string rawString, int maxLength = 30, char delimiter = '\\')
        {
            maxLength -= 3; //account for delimiter spacing

            if (rawString.Length <= maxLength)
            {
                return rawString;
            }

            string final = rawString;
            List<string> parts;

            int loops = 0;
            while (loops++ < 100)
            {
                parts = rawString.Split(delimiter).ToList();
                parts.RemoveRange(parts.Count - 1 - loops, loops);
                if (parts.Count == 1)
                {
                    return parts.Last();
                }

                parts.Insert(parts.Count - 1, "...");
                final = string.Join(delimiter.ToString(), parts);
                if (final.Length < maxLength)
                {
                    return final;
                }
            }

            return rawString.Split(delimiter).ToList().Last();
        }
    }
}
