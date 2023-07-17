# RoslynScriptRunner
<img src="https://www.nuget.org/Content/gallery/img/logo-header.svg?sanitize=true" height="30px">
  
RoslynScriptRunner is a versatile C#/VB.NET script execution library based on the Roslyn compiler, enabling dynamic runtime execution of C#/VB.NET code snippets without the need for pre-compilation.

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
runOptionStatic.InstanceObject = ScriptRunner.GetInstanceObject(codeStatic, runOptionStatic);
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
public int GenerateClassWithFunctionTestFunc()
{
    List<DiffRes> res = DiffTool.Diff(
        new List<string>() { ""11111111"", ""2222222"",  ""3333333"",  ""4444444"",  ""555"", ""666"", ""777"", ""888"", """", ""999"", ""99"", ""88"", ""77"" }, 
        new List<string>() { ""11111111"", ""22222222"", ""33333333"", ""44444444"",                             """", ""666"", ""99"", ""88"", ""77"" });
    List<GroupedDiffRes> groupedDiffRes = DiffTool.GetGroupedResult(res);
    return groupedDiffRes.Count;
}
";
RunOption generateClassWithFunctionOption = new RunOption();
generateClassWithFunctionOption.ExtraDllFileList = new List<string> { "Diff.dll" };
generateClassWithFunctionOption.MethodName = "GenerateClassWithFunctionTestFunc";
string codeGeneratedClassWithFunction = ScriptRunner.GenerateClassWithFunction(codeGenerateClassWithFunction, generateClassWithFunctionOption);
Console.WriteLine(ScriptRunner.Run(codeGeneratedClassWithFunction, generateClassWithFunctionOption)); // 7
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
``` csharp
void GetExtraDllsAndAssemblies(RunOption runOption, List<string> dlls, List<Assembly> extraAssemblies)
```
**DllHelper**
``` csharp
InstanceObject GetInstanceObject(string code, RunOption runOption = null)
```
``` csharp
InstanceObject GetInstanceObject(ICollection<string> codeList, RunOption runOption = null)
```

#### Options
**RunOption**
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
