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
RoslynScriptRunner.ScriptRunner.Run(codeHelloWorld);
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
ScriptRunner.Run(runOptionStatic);
ScriptRunner.Run(runOptionStatic);
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
Console.WriteLine(DelegateHelloWorldFunc(null));
```

#### API
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
Func<object[], object> GenerateFunc(string functionCode, RunOption runOption = null)
```
``` csharp
Func<object[], TResult> GenerateFunc<TResult>(string code, RunOption runOption = null)
```
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
```
**ScriptLanguage**
- CSharp
- VisualBasic

**Useage**
``` csharp
RoslynScriptRunner.RunOption runOption = new RoslynScriptRunner.RunOption(...);
RoslynScriptRunner.ScriptRunner.Run(code, runOption);
```
