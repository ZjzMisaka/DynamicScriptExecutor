# DynamicScriptExecutor
<img src="https://www.nuget.org/Content/gallery/img/logo-header.svg?sanitize=true" height="30px">

[English ReadMe](README.md)
  
运行时执行C#/VB.NET脚本而无需预编译。支持Func委托生成、DLLs、灵活的运行选项和异步功能等功能。

### 使用
DynamicScriptExecutor 可以通过 [Nuget 包](https://www.nuget.org/packages/ZjzMisaka.DynamicScriptExecutor/) 下载.

### 入门
**Hello World**
``` csharp
string codeHelloWorld = @"
using System;
class Exec
{
    public void Main()
    {
        Console.WriteLine(""Hello World"");
    }
}
";
DynamicScriptExecutor.ScriptExecutor.Exec(codeHelloWorld); // Hello World
```
**如果需要保持InstanceObject**
``` csharp
string codeStatic = @"
using System;
public static class Exec
{
    public static int count = 1;
    public static void Main()
    {
        Console.WriteLine(""Hello World Static: "" + count++);
    }
}
";
ExecOption execOptionStatic = new ExecOption() { IsStatic = true };
execOptionStatic.InstanceObject = new InstanceObject(codeStatic, execOptionStatic);
ScriptExecutor.Exec(execOptionStatic); // Hello World Static: 1
ScriptExecutor.Exec(execOptionStatic); // Hello World Static: 2
```
**如果需要创建委托**
``` csharp
string codeDelegateHelloWorld = @"
using System;
public class Exec
{
    public string DelegateHelloWorldFunc()
    {
        return ""Delegate Hello World"";
    }
}
";
var DelegateHelloWorldFunc = DynamicScriptExecutor.ScriptExecutor.GenerateFunc<string>(codeDelegateHelloWorld, new ExecOption() { MethodName = "DelegateHelloWorldFunc" });
Console.WriteLine(DelegateHelloWorldFunc(null)); // Delegate Hello World
```
**如果想单独写一个函数并且懒得写using语句**
``` csharp
string codeGenerateClassWithFunction = @"
public ExternalResultClass DoSth()
{
    return ExternalClass.DoSth();
}
";
ExecOption generateClassWithFunctionOption = new ExecOption();
generateClassWithFunctionOption.ExtraDllFileList = new List<string> { "ExternalDll.dll" };
generateClassWithFunctionOption.MethodName = "DoSth";
string codeGeneratedClassWithFunction = ScriptExecutor.GenerateClassWithFunction(codeGenerateClassWithFunction, generateClassWithFunctionOption);
```

#### API
**ScriptExecutor**
``` csharp
object Exec(string code, ExecOption execOption = null)
```
``` csharp
Task<object> ExecAsync(string code, ExecOption execOption = null)
```
``` csharp
object Exec(ICollection<string> codeList, ExecOption execOption = null)
```
``` csharp
Task<object> ExecAsync(ICollection<string> codeList, ExecOption execOption = null)
```
``` csharp
object Exec(ExecOption execOption)
```
``` csharp
Task<object> ExecAsync(ExecOption execOption)
```
``` csharp
Func<object[], object> GenerateFunc(string code, ExecOption execOption = null)
```
``` csharp
Func<object[], object> GenerateFunc(ExecOption execOption)
```
``` csharp
Func<object[], TResult> GenerateFunc<TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<object[], TResult> GenerateFunc<TResult>(ExecOption execOption)
```
``` csharp
Func<T1, TResult> GenerateFunc<T1, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, TResult> GenerateFunc<T1, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ExecOption execOption)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string code, ExecOption execOption = null)
```
``` csharp
Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ExecOption execOption)
```
``` csharp
string GenerateClassWithFunction(string code, ExecOption execOption = null)
```
``` csharp
string GenerateClassWithFunction(string code, ICollection<string> extraDllNamespaces, ExecOption execOption = null)
```
**DllHelper**
``` csharp
FileSystemInfo[] GetDllInfos(string path)
```
``` csharp
ICollection<string> GetExtraDllNamespaces(ExecOption execOption)
```

#### 实例对象
**InstanceObject**
``` csharp
InstanceObject(string code, ExecOption execOption = null)
```
``` csharp
InstanceObject(ICollection<string> codeList, ExecOption execOption = null)
```

#### 选项
**ExecOption**
``` csharp
ExecOption(object[] paramList = null
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
bool includeDllInBaseFolder;
```
**ScriptLanguage**
- CSharp
- VisualBasic

**示例**
``` csharp
DynamicScriptExecutor.ExecOption execOption = new DynamicScriptExecutor.ExecOption(...);
DynamicScriptExecutor.ScriptExecutor.Exec(code, execOption);
```
