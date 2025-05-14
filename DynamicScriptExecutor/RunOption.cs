namespace DynamicScriptExecutor
{
    public enum ScriptLanguage
    {
        CSharp,
        VisualBasic
    }
    public class ExecOption
    {
        private object[] paramList;
        private ICollection<string> extraDllFolderList;
        private ICollection<string> extraDllFileList;
        private string methodName;
        private string className;
        private InstanceObject instanceObject;
        private ScriptLanguage scriptLanguage;
        private bool nonPublic;
        private bool isStatic;
        private bool addDefaultUsingWhenGeneratingClass;
        private bool addExtraUsingWhenGeneratingClass;
        private bool includeDllInBaseFolder;

        public ExecOption(object[] paramList = null
            , ICollection<string> extraDllFolderList = null
            , ICollection<string> extraDllFileList = null
            , string methodName = "Main"
            , string className = "Exec"
            , InstanceObject instanceObject = null
            , ScriptLanguage scriptLanguage = ScriptLanguage.CSharp
            , bool nonPublic = false
            , bool isStatic = false
            , bool addDefaultUsingWhenGeneratingClass = true
            , bool addExtraUsingWhenGeneratingClass = true
            , bool includeDllInBaseFolder = true)
        {
            this.paramList = paramList;
            this.extraDllFolderList = extraDllFolderList;
            this.extraDllFileList = extraDllFileList;
            this.methodName = methodName;
            this.className = className;
            this.instanceObject = instanceObject;
            this.scriptLanguage = scriptLanguage;
            this.nonPublic = nonPublic;
            this.isStatic = isStatic;
            this.addDefaultUsingWhenGeneratingClass = addDefaultUsingWhenGeneratingClass;
            this.addExtraUsingWhenGeneratingClass = addExtraUsingWhenGeneratingClass;
            this.includeDllInBaseFolder = includeDllInBaseFolder;
        }

        public object[] ParamList { get => paramList; set => paramList = value; }
        public ICollection<string> ExtraDllFolderList { get => extraDllFolderList; set => extraDllFolderList = value; }
        public ICollection<string> ExtraDllFileList { get => extraDllFileList; set => extraDllFileList = value; }
        public string MethodName { get => methodName; set => methodName = value; }
        public string ClassName { get => className; set => className = value; }
        public InstanceObject InstanceObject { get => instanceObject; set => instanceObject = value; }
        public ScriptLanguage ScriptLanguage { get => scriptLanguage; set => scriptLanguage = value; }
        public bool NonPublic { get => nonPublic; set => nonPublic = value; }
        public bool IsStatic { get => isStatic; set => isStatic = value; }
        public bool AddDefaultUsingWhenGeneratingClass { get => addDefaultUsingWhenGeneratingClass; set => addDefaultUsingWhenGeneratingClass = value; }
        public bool AddExtraUsingWhenGeneratingClass { get => addExtraUsingWhenGeneratingClass; set => addExtraUsingWhenGeneratingClass = value; }
        public bool IncludeDllInBaseFolder { get => includeDllInBaseFolder; set => includeDllInBaseFolder = value; }

        public ExecOption Copy()
        {
            return new ExecOption(this.ParamList, this.ExtraDllFolderList, this.ExtraDllFileList, this.MethodName, this.ClassName, this.InstanceObject, this.ScriptLanguage, this.NonPublic, this.IsStatic, this.addDefaultUsingWhenGeneratingClass, this.addExtraUsingWhenGeneratingClass, this.includeDllInBaseFolder);
        }
    }
}
