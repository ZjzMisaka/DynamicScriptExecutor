using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Dynamic;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace RoslynScriptRunner
{
    public class ScriptRunner
    {
        public static object Run(string code, RunOption runOption = null)
        {
            if (runOption == null) 
            {
                runOption = new RunOption();
            }
            InstanceObject instanceObject;
            if (runOption.InstanceObject != null)
            {
                instanceObject = runOption.InstanceObject;
            }
            else
            {
                instanceObject = GetInstanceObject(code, runOption);
            }
            
            MethodInfo methodInfo = instanceObject.Type.GetMethod(runOption.MethodName);
            if (methodInfo == null)
            {
                throw new Exception($"Method not found: {runOption.MethodName}");
            }
            return methodInfo.Invoke(instanceObject.Instance, runOption.ParamList);
        }

        public static InstanceObject GetInstanceObject(string code, RunOption runOption, List<string> needDelDll = null)
        {
            List<string> dlls = new List<string>();

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

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            IEnumerable<Diagnostic> diagnostics = syntaxTree.GetDiagnostics();

            bool breakError = false;
            string errorStr = "";
            foreach (Diagnostic diagnostic in diagnostics)
            {
                breakError = true;
                errorStr = $"{errorStr}\n    {diagnostic.ToString()}";
            }
            if (breakError)
            {
                throw new Exception(errorStr);
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

            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                platform: Platform.AnyCpu
                //languageVersion: LanguageVersion.CSharp10,
                //runtimeMetadataVersion: "6.0"
                );
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: options);


            List<string> errDll = new List<string>();
            MemoryStream ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                errorStr = "";

                foreach (Diagnostic diagnostic in result.Diagnostics)
                {
                    if (diagnostic.Id == "CS0009")
                    {
                        string dll = diagnostic.GetMessage();
                        dll = dll.Substring(dll.IndexOf("'") + 1);
                        dll = dll.Substring(0, dll.IndexOf("'"));
                        errDll.Add(Path.GetFileName(dll));
                    }
                    else
                    {
                        breakError = true;
                        errorStr = $"{errorStr}\n    {diagnostic.Id}: {diagnostic.GetMessage()}";
                    }
                }

                if (breakError)
                {
                    throw new Exception(errorStr);
                }

                if (errDll.Count > 0)
                {
                    return GetInstanceObject(code, runOption, errDll);
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                Type type = assembly.GetType(runOption.ClassName);
                if (type == null) 
                {
                    throw new Exception($"Class not found: {runOption.ClassName}");
                }
                object obj = Activator.CreateInstance(type);
                return new InstanceObject(type, obj);
            }

            return null;
        }
    }
}