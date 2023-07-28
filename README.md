# RoslynScriptRunner
<img src="https://www.nuget.org/Content/gallery/img/logo-header.svg?sanitize=true" height="30px">

[中文 ReadMe](README_zh-CN.md)
  
Enables runtime execution of C#/VB.NET scripts without pre-compilation. Supports Func delegate generation, DLLs, flexible run options, and async capabilities.

### Download
RoslynScriptRunner is available as [Nuget Package](https://www.nuget.org/packages/ZjzMisaka.RoslynScriptRunner/) now.

### Getting started
**Hello World**
``` csharp
string codeHelloWorld = @"
using System;
class Run
{
    public void Main()
    {
        Console.WriteLine(""Hello World"");
    }
}
";
RoslynScriptRunner.ScriptRunner.Run(codeHelloWorld); // Hello World
```
**If you want to hold an InstanceObject**
``` csharp
string codeStatic = @"
using System;
public static class Run
{
    public static int count = 1;
    public static void Main()
    {
        Console.WriteLine(""Hello World Static: "" + count++);
    }
}
";
RunOption runOptionStatic = new RunOption() { IsStatic = true };
runOptionStatic.InstanceObject = new InstanceObject(codeStatic, runOptionStatic);
ScriptRunner.Run(runOptionStatic); // Hello World Static: 1
ScriptRunner.Run(runOptionStatic); // Hello World Static: 2
```
**If you want to create delegate**
``` csharp
string codeDelegateHelloWorld = @"
using System;
public class Run
{
    public string DelegateHelloWorldFunc()
    {
        return ""Delegate Hello World"";
    }
}
";
var DelegateHelloWorldFunc = RoslynScriptRunner.ScriptRunner.GenerateFunc<string>(codeDelegateHelloWorld, new RunOption() { MethodName = "DelegateHelloWorldFunc" });
Console.WriteLine(DelegateHelloWorldFunc(null)); // Delegate Hello World
```
**If you only want to write functions and don't want to write using statement**
``` csharp
string codeGenerateClassWithFunction = @"
public class ExternalResultClass
{
    return ExternalClass.DoSth();
}
";
RunOption generateClassWithFunctionOption = new RunOption();
generateClassWithFunctionOption.ExtraDllFileList = new List<string> { "ExternalDll.dll" };
generateClassWithFunctionOption.MethodName = "DoSth";
string codeGeneratedClassWithFunction = ScriptRunner.GenerateClassWithFunction(codeGenerateClassWithFunction, generateClassWithFunctionOption);
```

#### API
**ScriptRunner**
``` csharp
object Run(string code, RunOption runOption = null)
```
``` csharp
Task<object> RunAsync(string code, RunOption runOption = null)
```
``` csharp
object Run(ICollection<string> codeList, RunOption runOption = null)
```
``` csharp
Task<object> RunAsync(ICollection<string> codeList, RunOption runOption = null)
```
``` csharp
object Run(RunOption runOption)
```
``` csharp
Task<object> RunAsync(RunOption runOption)
```
``` csharp
Func<object[], object> GenerateFunc(string code, RunOption runOption = null)
```
``` csharp
Func<object[], object> GenerateFunc(RunOption runOption)
```
``` csharp
Func<object[], TResult> GenerateFunc<TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<object[], TResult> GenerateFunc<TResult>(RunOption runOption)
```
``` csharp
Func<T1, TResult> GenerateFunc<T1, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, TResult> GenerateFunc<T1, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(RunOption runOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string code, RunOption runOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(RunOption runOption)
```
``` csharp
string GenerateClassWithFunction(string code, RunOption runOption = null)
```
``` csharp
string GenerateClassWithFunction(string code, ICollection<string> extraDllNamespaces, RunOption runOption = null)
```
**DllHelper**
``` csharp
FileSystemInfo[] GetDllInfos(string path)
```
``` csharp
ICollection<string> GetExtraDllNamespaces(RunOption runOption)
```

#### InstanceObject
**InstanceObject**
``` csharp
InstanceObject(string code, RunOption runOption = null)
```
``` csharp
InstanceObject(ICollection<string> codeList, RunOption runOption = null)
```

#### Options
**RunOption**
``` csharp
RunOption(object[] paramList = null
    , ICollection<string> extraDllFolderList = null
    , ICollection<string> extraDllFileList = null
    , string methodName = "Main"
    , string className = "Run"
    , InstanceObject instanceObject = null
    , ScriptLanguage scriptLanguage = ScriptLanguage.CSharp
    , bool nonPublic = false
    , bool isStatic = false
    , bool addDefaultUsingWhenGeneratingClass = true
    , bool addExtraUsingWhenGeneratingClass = true)
```
``` csharp
object[] paramList;
ICollection<string> extraDllFolderList;
ICollection<string> extraDllFileList;
string methodName;
string className;
InstanceObject instanceObject;
ScriptLanguage scriptLanguage;
bool nonPublic;
bool isStatic;
bool addDefaultUsingWhenGeneratingClass;
bool addExtraUsingWhenGeneratingClass;
```
**ScriptLanguage**
- CSharp
- VisualBasic

**Useage**
``` csharp
RoslynScriptRunner.RunOption runOption = new RoslynScriptRunner.RunOption(...);
RoslynScriptRunner.ScriptRunner.Run(code, runOption);
```
