using DynamicScriptExecutor;

/**
 * HelloWorld test
 */
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
ScriptExecutor.Exec(codeHelloWorld);

/**
 * Param test
 */
string codeDraw = @"
using System;
using System.Text;
class Exec
{
    public string Main(int mid)
    {
        for (int i = 0; true; ++i)
        {
            int x = Math.Abs(mid - i);
            int y = mid * 2 - x;
            OutputLog(x, y, 2 * mid + 1);
            if (x == y && i > 0)
            {
                break;
            }
        }
        return ""codeDraw end"";
    }
    
    private void OutputLog(int x, int y,  int length)
    {
        StringBuilder stringBuilder = new (new string(' ', length));
        stringBuilder[x] = '*';
        stringBuilder[y] = '*';
        string value = stringBuilder.ToString();
        Console.WriteLine(value);
    }
}
";
object[] paramList = new object[1] { 30 };
string codeDrawRes = (string)ScriptExecutor.Exec(codeDraw, new ExecOption(paramList));
Console.WriteLine(codeDrawRes);


/**
 * InstanceObject test
 */
ExecOption execOptionHelloWorld = new ExecOption();
execOptionHelloWorld.InstanceObject = new InstanceObject(codeHelloWorld);
ScriptExecutor.Exec(execOptionHelloWorld);


/**
 * Async test
 */
string codeCostTime = @"
using System;
using System.Threading;
class Exec
{
    public int Main()
    {
        Console.WriteLine(""Start sleep"");
        Thread.Sleep(3000);
        Console.WriteLine(""Sleeped 3s"");
        return 1024;
    }
}
";
Task<object> task = ScriptExecutor.ExecAsync(codeCostTime);
Console.WriteLine("output after ExecAsync");
Thread.Sleep(1500);
Console.WriteLine("output when sleeping");
int result = (int)task.GetAwaiter().GetResult();
Console.WriteLine("output after GetResult, result is: " + result);

/**
 * Dependency test
 */
string codeMain = @"
using System;
class Exec
{
    public void Main()
    {
        Console.WriteLine(""Hello World In Main"");
        ExtraClass.ExtraOutput();
    }
}
";
string codeExtra = @"
using System;
public static class ExtraClass
{
    public static void ExtraOutput()
    {
        Console.WriteLine(""Hello World In Extra"");
    }
}
";
ScriptExecutor.Exec(new string[] { codeMain, codeExtra });

/**
 * Dependency + InstanceObject test
 */
ExecOption execOptionMainExtra = new ExecOption();
execOptionMainExtra.InstanceObject = new InstanceObject(new List<string> { codeMain, codeExtra });
ScriptExecutor.Exec(execOptionMainExtra);

/**
 * VB.NET HelloWorld test
 */
string codeHelloWorldVB = @"
Imports System
Public Class Exec
    Public Sub Main()
        Console.WriteLine(""Hello World VB.NET"")
    End Sub
End Class
";
ExecOption execOptionVB = new ExecOption() { ScriptLanguage = ScriptLanguage.VisualBasic };
ScriptExecutor.Exec(codeHelloWorldVB, execOptionVB);

/**
 * Private test
 */
string codeHelloWorldPrivate = @"
using System;
class Exec
{
    private void Main()
    {
        Console.WriteLine(""Hello World Private"");
    }
}
";
ExecOption execOptionHelloWorldPrivate = new ExecOption();
execOptionHelloWorldPrivate.NonPublic = true;
ScriptExecutor.Exec(codeHelloWorldPrivate, execOptionHelloWorldPrivate);

/**
 * Static test
 */
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
ScriptExecutor.Exec(execOptionStatic);
ScriptExecutor.Exec(execOptionStatic);

/**
 * Private Static test
 */
string codePrivateStatic = @"
using System;
static class Exec
{
    private static void Main()
    {
        Console.WriteLine(""Hello World Private & Static"");
    }
}
";
ExecOption execOptionPrivateStatic = new ExecOption() { IsStatic = true, NonPublic = true };
ScriptExecutor.Exec(codePrivateStatic, execOptionPrivateStatic);

/**
 * Delegate HelloWorld
 */
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
var DelegateHelloWorldFunc = ScriptExecutor.GenerateFunc<string>(codeDelegateHelloWorld, new ExecOption() { MethodName = "DelegateHelloWorldFunc" });
Console.WriteLine(DelegateHelloWorldFunc(null));

/**
 * Delegate test
 */
string codeDelegate = @"
using System;
using System.Text;
public class Exec
{
    public string DelegateTestFunc(int length, char c, int textIndex)
    {
        StringBuilder stringBuilder = new (new string('-', length));
        stringBuilder[textIndex] = c;
        return stringBuilder.ToString();
    }
}
";
var DelegateTestFunc = ScriptExecutor.GenerateFunc<string>(codeDelegate, new ExecOption() { MethodName = "DelegateTestFunc" });
Console.WriteLine(DelegateTestFunc(new object[] { 10, 't', 3 }));

/**
 * Delegate no generic test
 */
string codeDelegateNoGeneric = @"
using System;
using System.Text;
public class Exec
{
    public string DelegateNoGenericTestFunc()
    {
        return ""Delegate no generic test"";
    }
}
";
var DelegateNoGenericTestFunc = ScriptExecutor.GenerateFunc(codeDelegateNoGeneric, new ExecOption() { MethodName = "DelegateNoGenericTestFunc" });
Console.WriteLine(DelegateNoGenericTestFunc(null));

/**
 * Delegate test
 */
string codeDelegateMultipleParams = @"
using System;
using System.Text;
public class Exec
{
    public string CodeDelegateMultipleParamsTestFunc(int length, char c, int textIndex)
    {
        StringBuilder stringBuilder = new (new string('-', length));
        stringBuilder[textIndex] = c;
        return stringBuilder.ToString();
    }
}
";
var CodeDelegateMultipleParamsTestFunc = ScriptExecutor.GenerateFunc<int, char, int, string>(codeDelegateMultipleParams, new ExecOption() { MethodName = "CodeDelegateMultipleParamsTestFunc" });
Console.WriteLine(CodeDelegateMultipleParamsTestFunc(10, 't', 6));

/**
 * Delegate Static test
 */
string codeDelegateStatic = @"
using System;
public static class Exec
{
    public static int count = 1;
    public static void DelegateStaticFunc()
    {
        Console.WriteLine(""Hello World Delegate Static: "" + count++);
    }
}
";
ExecOption execOptionDelegateStatic = new ExecOption() { IsStatic = true };
execOptionStatic.InstanceObject = new InstanceObject(codeDelegateStatic, execOptionDelegateStatic);
execOptionStatic.MethodName = "DelegateStaticFunc";
var DelegateStaticFunc = ScriptExecutor.GenerateFunc<object>(execOptionStatic);
DelegateStaticFunc(null);
DelegateStaticFunc(null);

/**
 * GenerateClassWithFunction test
 */
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
ExecOption generateClassWithFunctionOption = new ExecOption();
generateClassWithFunctionOption.ExtraDllFileList = new List<string> { "Diff.dll" };
generateClassWithFunctionOption.MethodName = "GenerateClassWithFunctionTestFunc";
try
{
    string codeGeneratedClassWithFunction = ScriptExecutor.GenerateClassWithFunction(codeGenerateClassWithFunction, generateClassWithFunctionOption);
    Console.WriteLine(ScriptExecutor.Exec(codeGeneratedClassWithFunction, generateClassWithFunctionOption));
}
catch
{
    Console.WriteLine("DLL NOT FOUND");
}