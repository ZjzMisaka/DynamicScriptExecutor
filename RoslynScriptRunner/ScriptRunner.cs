using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using System.Reflection;

namespace RoslynScriptRunner
{
    public class ScriptRunner
    {
        public static object Run(string code, RunOption runOption = null)
        {
            return Run(new string[] { code }, runOption);
        }

        public static async Task<object> RunAsync(string code, RunOption runOption = null)
        {
            return await Task.Run(() => Run(new string[] { code }, runOption));
        }

        public static object Run(ICollection<string> codeList, RunOption runOption = null)
        {
            RunOption runOptionCopied;
            if (runOption == null)
            {
                runOptionCopied = new RunOption();
            }
            else
            {
                runOptionCopied = runOption.Copy();
            }
            InstanceObject instanceObject;
            if (runOptionCopied.InstanceObject != null)
            {
                instanceObject = runOptionCopied.InstanceObject;
            }
            else
            {
                instanceObject = GetInstanceObject(codeList, runOptionCopied);
            }

            runOptionCopied.InstanceObject = instanceObject;

            return Run(runOptionCopied);
        }


        public static async Task<object> RunAsync(ICollection<string> codeList, RunOption runOption = null)
        {
            return await Task.Run(() => Run(codeList, runOption));
        }

        public static object Run(RunOption runOption)
        {
            InstanceObject instanceObject = runOption.InstanceObject;

            if (!runOption.NonPublic)
            {
                MethodInfo methodInfo = instanceObject.Type.GetMethod(runOption.MethodName);
                if (methodInfo == null)
                {
                    Exception e = new Exception($"Method not found: {runOption.MethodName}");
                    e.Data.Add("Type", "MethodNotFound");
                    e.Data.Add("Value", runOption.MethodName);
                    throw e;
                }
                return methodInfo.Invoke(instanceObject.Instance, runOption.ParamList);
            }
            else
            {
                BindingFlags bindingFlags;

                if (runOption.IsStatic)
                {
                    bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
                }
                else
                {
                    bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                }

                MethodInfo methodInfo = instanceObject.Type.GetMethod(runOption.MethodName, bindingFlags);
                if (methodInfo == null)
                {
                    Exception e = new Exception($"Method not found: {runOption.MethodName}");
                    e.Data.Add("Type", "MethodNotFound");
                    e.Data.Add("Value", runOption.MethodName);
                    throw e;
                }
                return methodInfo.Invoke(instanceObject.Instance, bindingFlags, null, runOption.ParamList, null);
            }
        }

        public static async Task<object> RunAsync(RunOption runOption)
        {
            return await Task.Run(() => Run(runOption));
        }

        public static Func<object[], object> GenerateFunc(string functionCode, RunOption runOption = null)
        {
            return GenerateFunc<object>(functionCode, runOption);
        }

        public static Func<object[], TResult> GenerateFunc<TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            InstanceObject functionWrapperInstanceObject = GetInstanceObject(code, runOption);
            object functionWrapperInstance = functionWrapperInstanceObject.Instance;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (object[] paramList) { return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList); };
            return (Func<object[], TResult>)functionDelegate;
        }

        private static TResult RunMethod<TResult>(MethodInfo methodInfo, InstanceObject instanceObject, object[] paramList)
        {
            return (TResult)methodInfo.Invoke(instanceObject.Instance, paramList);
        }


        public static InstanceObject GetInstanceObject(string code, RunOption runOption = null)
        {
            return GetInstanceObject(new string[] { code }, runOption, null);
        }

        public static InstanceObject GetInstanceObject(ICollection<string> codeList, RunOption runOption = null)
        {
            return GetInstanceObject(codeList, runOption, null);
        }

        private static InstanceObject GetInstanceObject(ICollection<string> codeList, RunOption runOption = null, List<string> needDelDll = null)
        {
            List<string> dlls = new List<string>();

            if (runOption == null)
            {
                runOption = new RunOption();
            }

            // Dll文件夹中的dll
            if (runOption.ExtraDllFolderList != null) 
            {
                foreach (string extraDllFolder in runOption.ExtraDllFolderList)
                {
                    FileSystemInfo[] dllInfos = FileHelper.GetDllInfos(extraDllFolder);
                    if (dllInfos != null && dllInfos.Count() != 0)
                    {
                        foreach (FileSystemInfo dllInfo in dllInfos)
                        {
                            dlls.Add(dllInfo.FullName);
                            _ = Assembly.LoadFrom(dllInfo.FullName);
                        }
                    }
                }
            }

            // 单独的dll
            if (runOption.ExtraDllFileList != null)
            {
                foreach (string extraDllFile in runOption.ExtraDllFileList)
                {
                    dlls.Add(extraDllFile);
                    _ = Assembly.LoadFrom(extraDllFile);
                }
            }

            // 根目录的Dll
            FileSystemInfo[] dllInfosBase = FileHelper.GetDllInfos(Environment.CurrentDirectory);
            foreach (FileSystemInfo dllInfo in dllInfosBase)
            {
                if (dllInfo.Extension == ".dll" && !dlls.Contains(dllInfo.Name))
                {
                    dlls.Add(dllInfo.Name);
                }
            }

            string assemblyLocation = typeof(object).Assembly.Location;
            FileSystemInfo[] dllInfosAssembly = FileHelper.GetDllInfos(Path.GetDirectoryName(assemblyLocation));
            foreach (FileSystemInfo dllInfo in dllInfosAssembly)
            {
                if (dllInfo.Extension == ".dll" && !dlls.Contains(dllInfo.Name) && !dllInfo.Name.StartsWith("api"))
                {
                    dlls.Add(dllInfo.Name);
                }
            }

            string errorStr = "";
            bool breakError = false;
            SyntaxTree[] syntaxTreeArray = new SyntaxTree[codeList.Count];
            int index = 0;
            foreach (string code in codeList)
            {
                SyntaxTree syntaxTree;
                if (runOption.ScriptLanguage == ScriptLanguage.VisualBasic)
                {
                    syntaxTree = VisualBasicSyntaxTree.ParseText(code);
                }
                else
                {
                    syntaxTree = CSharpSyntaxTree.ParseText(code);
                }
                
                syntaxTreeArray[index] = syntaxTree;

                IEnumerable<Diagnostic> diagnostics = syntaxTree.GetDiagnostics();

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    breakError = true;
                    errorStr = $"{errorStr}\n    {diagnostic.ToString()}";
                }
                if (breakError)
                {
                    Exception e = new Exception(errorStr);
                    e.Data.Add("Type", "SyntaxError");
                    e.Data.Add("Value", errorStr);
                    throw e;
                }

                ++index;
            }

            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            };

            if (needDelDll != null)
            {
                foreach (string errorDll in needDelDll)
                {
                    dlls.Remove(errorDll);
                }
            }

            // 循环遍历每个 DLL，并将其包含在编译中
            foreach (string dllName in dlls)
            {
                if (File.Exists(dllName))
                {
                    references = references.Append(MetadataReference.CreateFromFile(dllName)).ToArray();
                }
                else
                {
                    references = references.Append(MetadataReference.CreateFromFile(assemblyLocation.Replace("System.Private.CoreLib.dll", dllName))).ToArray();
                }
            }

            Compilation compilation;
            if (runOption.ScriptLanguage == ScriptLanguage.VisualBasic)
            {
                VisualBasicCompilationOptions options = new VisualBasicCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    platform: Platform.AnyCpu
                );
                compilation = VisualBasicCompilation.Create(
                    assemblyName,
                    syntaxTrees: syntaxTreeArray,
                    references: references,
                    options: options
                );
            }
            else
            {
                CSharpCompilationOptions options = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    platform: Platform.AnyCpu
                );
                compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: syntaxTreeArray,
                    references: references,
                    options: options
                );
            }

            List<string> errDllList = new List<string>();
            MemoryStream ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                errorStr = "";

                foreach (Diagnostic diagnostic in result.Diagnostics)
                {
                    if (diagnostic.Id == "CS0009" || diagnostic.Id == "BC31519")
                    {
                        string dll = diagnostic.GetMessage();
                        dll = dll.Substring(dll.IndexOf("'") + 1);
                        dll = dll.Substring(0, dll.IndexOf("'"));
                        errDllList.Add(Path.GetFileName(dll));
                    }
                    else
                    {
                        breakError = true;
                        errorStr = $"{errorStr}\n    {diagnostic.Id}: {diagnostic.GetMessage()}";
                    }
                }

                if (breakError)
                {
                    Exception e = new Exception(errorStr);
                    e.Data.Add("Type", "Error");
                    e.Data.Add("Value", errorStr);
                    throw e;
                }

                if (errDllList.Count > 0)
                {
                    return GetInstanceObject(codeList, runOption, errDllList);
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                Type type = assembly.GetType(runOption.ClassName);
                if (type == null) 
                {
                    Exception e = new Exception($"Class not found: {runOption.ClassName}");
                    e.Data.Add("Type", "ClassNotFound");
                    e.Data.Add("Value", runOption.ClassName);
                    throw e;
                }

                object obj = null;
                if (!runOption.IsStatic)
                {
                    obj = Activator.CreateInstance(type);
                }
                
                return new InstanceObject(type, obj, runOption);
            }

            return null;
        }
    }
}