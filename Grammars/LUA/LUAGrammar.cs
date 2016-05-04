using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using NLua;
using NLua.Exceptions;

namespace Grammars.LUA
{
    public class LUAGrammar : BaseGrammar
    {
        private StringBuilder _output;

        private string _script;
        private string _scriptPath;

        [XmlElement("script")]
        public string Script
        {
            get { return _script; }
            set
            {
                if (value != _script)
                {
                    _script = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlElement("scriptPath")]
        public string ScriptPath
        {
            get
            {
                if (Path.IsPathRooted(_scriptPath))
                {
                    return _scriptPath;
                }
                return Path.Combine(
                    Path.GetDirectoryName(Assembly.GetCallingAssembly().Location),
                    "DataFiles",
                    _scriptPath);
            }
            set { _scriptPath = value; }
        }

        public override string this[string columnName]
        {
            get
            {
                if (columnName == "Script" && string.IsNullOrWhiteSpace(Script))
                {
                    return "Script is required.";
                }
                return base[columnName];
            }
        }

        public override string Analyze()
        {
            return "Not Supported";
        }

        protected override string GenerateName()
        {
            var l = new Lua();
            var modulePath = string.Empty;

            try
            {
                _output = null;
                foreach (var path in Utility.GrammarFilePaths)
                {
                    modulePath += ";" + Path.Combine(path, "lua");
                    modulePath += "\\?.lua";
                }

                modulePath = modulePath.Replace("\\", "\\\\");

                foreach (var parameter in Parameters)
                {
                    l[parameter.Name] = parameter.Value;
                }

                l["Grammar"] = this;
                l["Me"] = l;
                l.RegisterFunction("print", this, GetType().GetMethod("Print"));
                l.RegisterFunction("printif", this, GetType().GetMethod("PrintIf"));
                l.DoString("luanet.load_assembly(\'Grammars\')");
                l.DoString("CalcClass=luanet.import_type(\'Grammars.LUA.Calculator\')");
                l.DoString("calc=CalcClass(Grammar,Me)");
                l.DoString("package.path = package.path .. \'" + modulePath + "\'");
                l.DoString("import = function() end");

                if (string.IsNullOrWhiteSpace(Script))
                {
                    l.DoFile(ScriptPath);
                }
                else
                {
                    l.DoString(Script);
                }

                if (_output == null)
                {
                    return string.Empty;
                }
                return _output.ToString();
            }
            catch (LuaScriptException ex)
            {
                if (_output != null)
                {
                    return _output + "<br /><br /><span style=\'color:red;\'>" + ex + "</span>";
                }
                return "<span style=\'color:red;\'>" + ex + "</span>";
            }
        }

        public void Print(object value)
        {
            if (_output == null)
            {
                _output = new StringBuilder();
            }
            _output.AppendLine(value.ToString());
        }

        public void PrintIf(bool condition, object value)
        {
            if (condition)
            {
                Print(value);
            }
        }
    }
}