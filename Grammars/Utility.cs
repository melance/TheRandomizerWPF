using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.VisualBasic.ApplicationServices;

namespace Grammars
{
    public static class Utility
    {
        private static List<string> _grammarFilePaths;
        private static string _tempPath;

        internal static string TempPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tempPath))
                {
                    _tempPath = Path.Combine(Path.GetTempPath(), (new ConsoleApplicationBase()).Info.AssemblyName);
                }
                return Environment.ExpandEnvironmentVariables(_tempPath);
            }
            set { _tempPath = value; }
        }

        public static List<string> GrammarFilePaths
        {
            get
            {
                if (_grammarFilePaths == null)
                {
                    _grammarFilePaths = new List<string>();
                }
                return _grammarFilePaths;
            }
        }

        public static PropertyDescriptor GetPropertyDescriptor(Type type, string name)
        {
            return TypeDescriptor.GetProperties(type)[name];
        }
    }
}