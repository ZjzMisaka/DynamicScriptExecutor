﻿string codeHelloWorld = @"
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
    public void Main(int mid)
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
RoslynScriptRunner.Runner.Run(codeHelloWorld);
RoslynScriptRunner.RunOption runOption = new RoslynScriptRunner.RunOption(paramList);
RoslynScriptRunner.Runner.Run(codeDraw, runOption);