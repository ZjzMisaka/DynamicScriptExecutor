# RoslynScriptRunner
Roslyn script runner

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
RoslynScriptRunner.Runner.Run(codeHelloWorld);
```
#### Options
**RoslynScriptRunner.RunOption**
``` csharp
object[] paramList;
List<string> extraDllFolderList;
List<string> extraDllFileList;
string methodName;
string className;
InstanceObject instanceObject;
```
**Useage**
``` csharp
RoslynScriptRunner.RunOption runOption = new RoslynScriptRunner.RunOption(...);
RoslynScriptRunner.Runner.Run(code, runOption);
```