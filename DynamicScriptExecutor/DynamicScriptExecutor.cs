using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace DynamicScriptExecutor
{
    public class ScriptExecutor
    {
        public static object Exec(string code, ExecOption execOption = null)
        {
            return Exec(new string[] { code }, execOption);
        }

        public static async Task<object> ExecAsync(string code, ExecOption execOption = null)
        {
            return await Task.Run(() => Exec(new string[] { code }, execOption));
        }

        public static object Exec(ICollection<string> codeList, ExecOption execOption = null)
        {
            ExecOption execOptionCopied;
            if (execOption == null)
            {
                execOptionCopied = new ExecOption();
            }
            else
            {
                execOptionCopied = execOption.Copy();
            }
            InstanceObject instanceObject;
            if (execOptionCopied.InstanceObject != null)
            {
                instanceObject = execOptionCopied.InstanceObject;
            }
            else
            {
                instanceObject = new InstanceObject(codeList, execOptionCopied);
            }

            execOptionCopied.InstanceObject = instanceObject;

            return Exec(execOptionCopied);
        }


        public static async Task<object> ExecAsync(ICollection<string> codeList, ExecOption execOption = null)
        {
            return await Task.Run(() => Exec(codeList, execOption));
        }

        public static object Exec(ExecOption execOption)
        {
            InstanceObject instanceObject = execOption.InstanceObject;
            BindingFlags bindingFlags = execOption.NonPublic ? BindingFlags.NonPublic : BindingFlags.Public;

            bindingFlags |= execOption.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            MethodInfo methodInfo = instanceObject.Type.GetMethod(execOption.MethodName, bindingFlags);

            if (methodInfo == null)
            {
                throw new MissingMethodException(instanceObject.Type.FullName, execOption.MethodName);
            }

            try
            {
                return methodInfo.Invoke(instanceObject.Instance, execOption.ParamList);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Method invocation failed.", e);
            }
        }

        public static async Task<object> ExecAsync(ExecOption execOption)
        {
            return await Task.Run(() => Exec(execOption));
        }

        public static Func<object[], object> GenerateFunc(string code, ExecOption execOption = null)
        {
            return GenerateFunc<object>(code, execOption);
        }

        public static Func<object[], object> GenerateFunc(ExecOption execOption)
        {
            return GenerateFunc<object>(execOption);
        }

        public static Func<object[], TResult> GenerateFunc<TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            InstanceObject functionWrapperInstanceObject = new InstanceObject(code, execOption);
            newExecOption.InstanceObject = functionWrapperInstanceObject;

            return GenerateFunc<TResult>(newExecOption);
        }

        public static Func<object[], TResult> GenerateFunc<TResult>(ExecOption execOption)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);

            Delegate functionDelegate = delegate (object[] paramList) { return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList); };
            return (Func<object[], TResult>)functionDelegate;
        }

        public static Func<T1, TResult> GenerateFunc<T1, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, TResult>(newExecOption);
        }

        public static Func<T1, TResult> GenerateFunc<T1, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1)
            {
                object[] paramList = new object[1] { p1 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, TResult>)functionDelegate;
        }

        public static Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, TResult>(newExecOption);
        }

        public static Func<T1, T2, TResult> GenerateFunc<T1, T2, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2)
            {
                object[] paramList = new object[2] { p1, p2 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, TResult> GenerateFunc<T1, T2, T3, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3)
            {
                object[] paramList = new object[3] { p1, p2, p3 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, TResult> GenerateFunc<T1, T2, T3, T4, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4)
            {
                object[] paramList = new object[4] { p1, p2, p3, p4 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, T5, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, T5, TResult> GenerateFunc<T1, T2, T3, T4, T5, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
            {
                object[] paramList = new object[5] { p1, p2, p3, p4, p5 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
            {
                object[] paramList = new object[6] { p1, p2, p3, p4, p5, p6 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
            {
                object[] paramList = new object[7] { p1, p2, p3, p4, p5, p6, p7 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
            {
                object[] paramList = new object[8] { p1, p2, p3, p4, p5, p6, p7, p8 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
            {
                object[] paramList = new object[9] { p1, p2, p3, p4, p5, p6, p7, p8, p9 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)functionDelegate;
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            ExecOption newExecOption = execOption.Copy();

            newExecOption.InstanceObject = new InstanceObject(code, execOption);
            return GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(newExecOption);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> GenerateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ExecOption execOption)
        {
            InstanceObject functionWrapperInstanceObject = execOption.InstanceObject;
            MethodInfo createDelegateMethod = functionWrapperInstanceObject.Type.GetMethod(execOption.MethodName);


            Delegate functionDelegate = delegate (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10)
            {
                object[] paramList = new object[10] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
                return ExecMethod<TResult>(createDelegateMethod, functionWrapperInstanceObject, paramList);
            };
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)functionDelegate;
        }

        private static TResult ExecMethod<TResult>(MethodInfo methodInfo, InstanceObject instanceObject, object[] paramList)
        {
            return (TResult)methodInfo.Invoke(instanceObject.Instance, paramList);
        }

        public static string GenerateClassWithFunction(string code, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            if (execOption.AddExtraUsingWhenGeneratingClass)
            {
                return GenerateClassWithFunction(code, DllHelper.GetExtraDllNamespaces(execOption), execOption);
            }
            else
            {
                return GenerateClassWithFunction(code, new HashSet<string>(), execOption);
            }
        }

        public static string GenerateClassWithFunction(string code, ICollection<string> extraDllNamespaces, ExecOption execOption = null)
        {
            if (execOption == null)
            {
                execOption = new ExecOption();
            }

            string extraUsings = "";
            if (execOption.AddExtraUsingWhenGeneratingClass)
            {
                foreach (string nameSpace in extraDllNamespaces)
                {
                    if (string.IsNullOrWhiteSpace(nameSpace))
                    {
                        continue;
                    }
                    extraUsings = $"{extraUsings}using {nameSpace};\n";
                }
            }

            string defaultUsings;
            if (execOption.AddDefaultUsingWhenGeneratingClass)
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
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
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
public class Exec
{{
    {code}
}}
";
        }

        

        

        
    }
}