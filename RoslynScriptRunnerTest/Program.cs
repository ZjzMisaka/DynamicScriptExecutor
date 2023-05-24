using RoslynScriptRunner;

/**
 * HelloWorld test
 */
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
ScriptRunner.Run(codeHelloWorld);

/**
 * Param test
 */
string codeDraw = @"
using System;
using System.Text;
class Run
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
string codeDrawRes = (string)ScriptRunner.Run(codeDraw, new RunOption(paramList));
Console.WriteLine(codeDrawRes);


/**
 * InstanceObject test
 */
RunOption runOptionHelloWorld = new RunOption();
runOptionHelloWorld.InstanceObject = ScriptRunner.GetInstanceObject(codeHelloWorld);
ScriptRunner.Run(runOptionHelloWorld);


/**
 * Async test
 */
string codeCostTime = @"
using System;
using System.Threading;
class Run
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
Task<object> task = ScriptRunner.RunAsync(codeCostTime);
Console.WriteLine("output after RunAsync");
Thread.Sleep(1500);
Console.WriteLine("output when sleeping");
int result = (int)task.GetAwaiter().GetResult();
Console.WriteLine("output after GetResult, result is: " + result);

/**
 * Dependency test
 */
string codeMain = @"
using System;
class Run
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
public class ExtraClass
{
    public static void ExtraOutput()
    {
        Console.WriteLine(""Hello World In Extra"");
    }
}
";
ScriptRunner.Run(new string[] { codeMain, codeExtra });

/**
 * Dependency + InstanceObject test
 */
RunOption runOptionMainExtra = new RunOption();
runOptionMainExtra.InstanceObject = ScriptRunner.GetInstanceObject(new List<string> { codeMain, codeExtra });
ScriptRunner.Run(runOptionMainExtra);