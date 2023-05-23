using RoslynScriptRunner;

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
object[] paramList = new object[1];
paramList[0] = 30;
ScriptRunner.Run(codeHelloWorld);
RunOption runOption = new RunOption(paramList);
string codeDrawRes = (string)ScriptRunner.Run(codeDraw, runOption);
Console.WriteLine(codeDrawRes);

RunOption runOption1 = new RunOption();
runOption1.InstanceObject = ScriptRunner.GetInstanceObject(codeHelloWorld);
ScriptRunner.Run(runOption1);



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