using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRU
{
    public interface IStorage
    {
        IEnumerable<string> ReadFileList();
        void WriteFileList(IEnumerable<string> fileList);
        
    }
}
