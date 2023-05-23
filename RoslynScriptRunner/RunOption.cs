namespace RoslynScriptRunner
{
    public class RunOption
    {
        private object[] paramList;
        private List<string> extraDllFolderList;
        private List<string> extraDllFileList;
        private string methodName;
        private string className;
        private InstanceObject instanceObject;

        public RunOption(object[] paramList = null, List<string> extraDllFolderList = null, List<string> extraDllFileList = null, string methodName = "Main", string className = "Run", InstanceObject instanceObject = null)
        {
            this.paramList = paramList;
            this.extraDllFolderList = extraDllFolderList;
            this.extraDllFileList = extraDllFileList;
            this.methodName = methodName;
            this.className = className;
            this.instanceObject = instanceObject;
        }

        public object[] ParamList { get => paramList; set => paramList = value; }
        public List<string> ExtraDllFolderList { get => extraDllFolderList; set => extraDllFolderList = value; }
        public List<string> ExtraDllFileList { get => extraDllFileList; set => extraDllFileList = value; }
        public string MethodName { get => methodName; set => methodName = value; }
        public string ClassName { get => className; set => className = value; }
        public InstanceObject InstanceObject { get => instanceObject; set => instanceObject = value; }
    }
}
