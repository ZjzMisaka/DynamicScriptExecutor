using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Dynamic;
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

        public static Func<object[], object> GenerateFunc(string code, RunOption runOption = null)
        {
            return GenerateFunc<object>(code, runOption);
        }

        public static Func<object[], object> GenerateFunc(RunOption runOption)
        {
            return GenerateFunc<object>(runOption);
        }

        public static Func<object[], TResult> GenerateFunc<TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            InstanceObject functionWrapperInstanceObject = GetInstanceObject(code, runOption);
            newRunOption.InstanceObject = functionWrapperInstanceObject;

            return GenerateFunc<TResult>(newRunOption);
        }

        public static Func<object[], TResult> GenerateFunc<TResult>(RunOption runOption)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);

            Delegate functionDelegate = delegate (object[] paramList) { return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList); };
            return (Func<object[], TResult>)functionDelegate;
        }

        public static Func<T1, TResult> GenerateFunc<T1, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, TResult>(newRunOption);
        }

        public static Func<T1, TResult> GenerateFunc<T1, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1)
            {
                object[] paramList = new object[1] { p1 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, TResult>)functionDelegate;
        }

        public static Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, TResult>(newRunOption);
        }

        public static Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2)
            {
                object[] paramList = new object[2] { p1, p2 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3)
            {
                object[] paramList = new object[3] { p1, p2, p3 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4)
            {
                object[] paramList = new object[4] { p1, p2, p3, p4 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, T5, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
            {
                object[] paramList = new object[5] { p1, p2, p3, p4, p5 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
            {
                object[] paramList = new object[6] { p1, p2, p3, p4, p5, p6 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
            {
                object[] paramList = new object[7] { p1, p2, p3, p4, p5, p6, p7 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
            {
                object[] paramList = new object[8] { p1, p2, p3, p4, p5, p6, p7, p8 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
            {
                object[] paramList = new object[9] { p1, p2, p3, p4, p5, p6, p7, p8, p9 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            RunOption newRunOption = runOption.Copy();

            newRunOption.InstanceObject = GetInstanceObject(code, runOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(newRunOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(RunOption runOption)
        {
            InstanceObject functionWrapperInstanceObject = runOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(runOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10)
            {
                object[] paramList = new object[10] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
                return RunMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)functionDelegate;
        }

        private static TResult RunMethod<TResult>(MethodInfo methodInfo, InstanceObject instanceObject, object[] paramList)
        {
            return (TResult)methodInfo.Invoke(instanceObject.Instance, paramList);
        }

        public static string GenerateClassWithFunction(string code, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            if (runOption.AddExtraUsingWhenGeneratingClass)
            {
                return GenerateClassWithFunction(code, GetExtraDllNamespaces(runOption), runOption);
            }
            else
            {
                return GenerateClassWithFunction(code, new HashSet<string>(), runOption);
            }
        }

        public static string GenerateClassWithFunction(string code, ICollection<string> extraDllNamespaces, RunOption runOption = null)
        {
            if (runOption == null)
            {
                runOption = new RunOption();
            }

            string extraUsings = "";
            if (runOption.AddExtraUsingWhenGeneratingClass)
            {
                foreach (string nameSpace in extraDllNamespaces)
                {
                    extraUsings = $"{extraUsings}using {nameSpace};\n";
                }
            }

            string defaultUsings;
            if (runOption.AddDefaultUsingWhenGeneratingClass)
            {
                defaultUsings = @"
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Principal;
using System.IO.Compression;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
";
            }
            else
            {
                defaultUsings = "";
            }

            return @$"
using System;
{defaultUsings}
{extraUsings}
public class Run
{{
    {code}
}}
";
        }

        public static ICollection<string> GetExtraDllNamespaces(RunOption runOption)
        {
            if (!runOption.AddExtraUsingWhenGeneratingClass)
            { 
                return new HashSet<string>();
            }

            List<Assembly> extraAssemblies = new List<Assembly>();
            GetExtraDlls(runOption, null, extraAssemblies);
            HashSet<string> result = new HashSet<string>();
            foreach (Assembly assembly in extraAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!result.Contains(type.Namespace))
                    {
                        result.Add(type.Namespace);
                    }
                }
            }
            return result;
        }

        private static void GetExtraDlls(RunOption runOption, List<string> dlls, List<Assembly> extraAssemblies)
        {
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
                            if (dlls != null)
                            {
                                dlls.Add(dllInfo.FullName);
                            }
                            Assembly assembly = Assembly.LoadFrom(dllInfo.FullName);
                            if (extraAssemblies != null)
                            {
                                extraAssemblies.Add(assembly);
                            }
                        }
                    }
                }
            }

            // 单独的dll
            if (runOption.ExtraDllFileList != null)
            {
                foreach (string extraDllFile in runOption.ExtraDllFileList)
                {
                    if (dlls != null)
                    {
                        dlls.Add(extraDllFile);
                    }
                    Assembly assembly = Assembly.LoadFrom(extraDllFile);
                    if (extraAssemblies != null)
                    {
                        extraAssemblies.Add(assembly);
                    }
                }
            }
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

            GetExtraDlls(runOption, dlls, null);

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
                
                return new InstanceObject(type, obj);
            }

            return null;
        }
    }
}