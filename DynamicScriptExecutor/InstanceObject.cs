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
using System.Dynamic;
using System.IO;

namespace DynamicScriptExecutor
{
    public class InstanceObject
    {
        private Type type;
        private object instance;

        public Type Type { get => type; set => type = value; }
        public object Instance { get => instance; set => instance = value; }

        public InstanceObject(string code, ExecOption execOption = null)
        {
            SetInstanceObject(new string[] { code }, execOption, null);
        }

        public InstanceObject(ICollection<string> codeList, ExecOption execOption = null)
        {
            SetInstanceObject(codeList, execOption, null);
        }

        private void SetInstanceObject(ICollection<string> codeList, ExecOption execOption = null, List<string> needDelDll = null)
        {
            List<string> dlls = new List<string>();

            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            DllHelper.GetExtraDllsAndAssemblies(execOption, dlls, null);

            // 根目录的Dll
            if (execOption.IncludeDllInBaseFolder)
            {
                FileSystemInfo[] dllInfosBase = DllHelper.GetDllInfos(Environment.CurrentDirectory);
                foreach (FileSystemInfo dllInfo in dllInfosBase)
                {
                    if (dllInfo.Extension == ".dll" && !dlls.Contains(dllInfo.Name))
                    {
                        dlls.Add(dllInfo.Name);
                    }
                }
            }

            string assemblyLocation = typeof(object).Assembly.Location;
            dlls.Add("System.dll");
            dlls.Add("System.Console.dll");
            dlls.Add("System.Linq.dll");
            dlls.Add("System.Net.dll");
            dlls.Add("System.Net.Http.dll");
            dlls.Add("System.Threading.dll");
            dlls.Add("System.IO.dll");
            dlls.Add("System.Drawing.dll");
            dlls.Add("System.Xml.dll");
            dlls.Add("System.Xml.Linq.dll");
            dlls.Add("System.Security.dll");
            dlls.Add("System.Security.Claims.dll");
            dlls.Add("System.Data.dll");
            dlls.Add("System.Runtime.dll");
            dlls.Add("System.ComponentModel.dll");
            dlls.Add("System.ComponentModel.DataAnnotations.dll");

            string errorStr = "";
            bool breakError = false;
            SyntaxTree[] syntaxTreeArray = new SyntaxTree[codeList.Count];
            int index = 0;
            foreach (string code in codeList)
            {
                SyntaxTree syntaxTree;
                if (execOption.ScriptLanguage == ScriptLanguage.VisualBasic)
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
            if (execOption.ScriptLanguage == ScriptLanguage.VisualBasic)
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
                        dll = DllHelper.ExtractPath(dll);
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
                else 
                {
                    SetInstanceObject(codeList, execOption, errDllList);
                    return;
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                Type type = assembly.GetType(execOption.ClassName);
                if (type == null)
                {
                    TypeLoadException e = new TypeLoadException($"Unable to load type: {execOption.ClassName}");
                    throw e;
                }

                object obj = null;
                if (!execOption.IsStatic)
                {
                    obj = Activator.CreateInstance(type);
                }

                this.Type = type;
                this.Instance = obj;
            }
        }
    }
}
