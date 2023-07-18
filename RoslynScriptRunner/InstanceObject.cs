using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace RoslynScriptRunner
{
    public class InstanceObject
    {
        private Type type;
        private object instance;

        public InstanceObject(Type type, object instance)
        {
            this.Type = type;
            this.Instance = instance;
        }

        public Type Type { get => type; set => type = value; }
        public object Instance { get => instance; set => instance = value; }

        public static InstanceObject Get(string code, RunOption runOption = null)
        {
            return Get(new string[] { code }, runOption, null);
        }

        public static InstanceObject Get(ICollection<string> codeList, RunOption runOption = null)
        {
            return Get(codeList, runOption, null);
        }

        private static InstanceObject Get(ICollection<string> codeList, RunOption runOption = null, List<string> needDelDll = null)
        {
            List<string> dlls = new List<string>();

            if (runOption == null)
            {
                runOption = new RunOption();
            }

            DllHelper.GetExtraDllsAndAssemblies(runOption, dlls, null);

            // 根目录的Dll
            FileSystemInfo[] dllInfosBase = DllHelper.GetDllInfos(Environment.CurrentDirectory);
            foreach (FileSystemInfo dllInfo in dllInfosBase)
            {
                if (dllInfo.Extension == ".dll" && !dlls.Contains(dllInfo.Name))
                {
                    dlls.Add(dllInfo.Name);
                }
            }

            string assemblyLocation = typeof(object).Assembly.Location;
            FileSystemInfo[] dllInfosAssembly = DllHelper.GetDllInfos(Path.GetDirectoryName(assemblyLocation));
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
                    SyntaxErrorException e = new SyntaxErrorException(errorStr);
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
                        errorStr = $"{errorStr}\n    {diagnostic.ToString()}";
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
                    return Get(codeList, runOption, errDllList);
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                Type type = assembly.GetType(runOption.ClassName);
                if (type == null)
                {
                    TypeLoadException e = new TypeLoadException($"Unable to load type: {runOption.ClassName}");
                    throw e;
                }

                object obj = null;
                if (!runOption.IsStatic)
                {
                    obj = Activator.CreateInstance(type);
                }

                return new InstanceObject(type, obj);
            }

            return null;
        }
    }
}
