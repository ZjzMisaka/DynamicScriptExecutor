namespace RoslynScriptRunner
{
    public enum ScriptLanguage
    {
        CSharp,
        VisualBasic
    }
    public class RunOption
    {
        private object[] paramList;
        private ICollection<string> extraDllFolderList;
        private ICollection<string> extraDllFileList;
        private string methodName;
        private string className;
        private InstanceObject instanceObject;
        private ScriptLanguage scriptLanguage;

        public RunOption(object[] paramList = null, ICollection<string> extraDllFolderList = null, ICollection<string> extraDllFileList = null, string methodName = "Main", string className = "Run", InstanceObject instanceObject = null, ScriptLanguage scriptLanguage = ScriptLanguage.CSharp)
        {
            this.paramList = paramList;
            this.extraDllFolderList = extraDllFolderList;
            this.extraDllFileList = extraDllFileList;
            this.methodName = methodName;
            this.className = className;
            this.instanceObject = instanceObject;
            this.scriptLanguage = scriptLanguage;
        }

        public object[] ParamList { get => paramList; set => paramList = value; }
        public ICollection<string> ExtraDllFolderList { get => extraDllFolderList; set => extraDllFolderList = value; }
        public ICollection<string> ExtraDllFileList { get => extraDllFileList; set => extraDllFileList = value; }
        public string MethodName { get => methodName; set => methodName = value; }
        public string ClassName { get => className; set => className = value; }
        public InstanceObject InstanceObject { get => instanceObject; set => instanceObject = value; }
        public ScriptLanguage ScriptLanguage { get => scriptLanguage; set => scriptLanguage = value; }

        public RunOption Copy()
        { 
            return new RunOption(this.paramList, this.extraDllFolderList, this.extraDllFileList, this.methodName, this.className, this.instanceObject, this.scriptLanguage);
        }
    }
}
