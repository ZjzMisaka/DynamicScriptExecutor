using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicScriptExecutor
{
    public class DllHelper
    {
        public static FileSystemInfo[] GetDllInfos(string path)
        {
            string folderPath = path;
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileSystemInfo[] dllInfos = null;
            if (dir.Exists)
            {
                DirectoryInfo dirD = dir as DirectoryInfo;
                dllInfos = dirD.GetFileSystemInfos();
            }

            return dllInfos;
        }

        public static ICollection<string> GetExtraDllNamespaces(ExecOption execOption)
        {
            if (!execOption.AddExtraUsingWhenGeneratingClass)
            {
                return new HashSet<string>();
            }

            List<Assembly> extraAssemblies = new List<Assembly>();
            GetExtraDllsAndAssemblies(execOption, null, extraAssemblies);
            HashSet<string> result = new HashSet<string>();
            foreach (Assembly assembly in extraAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!result.Contains(type.Namespace))
                    {
                        result.Add(type.Namespace);
                    }
                }
            }
            return result;
        }

        internal static void GetExtraDllsAndAssemblies(ExecOption execOption, List<string> dlls, List<Assembly> extraAssemblies)
        {
            // Dll文件夹中的dll
            if (execOption.ExtraDllFolderList != null)
            {
                foreach (string extraDllFolder in execOption.ExtraDllFolderList)
                {
                    FileSystemInfo[] dllInfos = GetDllInfos(extraDllFolder);
                    if (dllInfos != null && dllInfos.Count() != 0)
                    {
                        foreach (FileSystemInfo dllInfo in dllInfos)
                        {
                            Assembly assembly = Assembly.LoadFrom(dllInfo.FullName);

                            if (dlls != null)
                            {
                                dlls.Add(dllInfo.FullName);
                            }
                            
                            if (extraAssemblies != null)
                            {
                                extraAssemblies.Add(assembly);
                            }
                        }
                    }
                }
            }

            // 单独的dll
            if (execOption.ExtraDllFileList != null)
            {
                foreach (string extraDllFile in execOption.ExtraDllFileList)
                {
                    if (dlls != null)
                    {
                        dlls.Add(extraDllFile);
                    }
                    
                    if (extraAssemblies != null)
                    {
                        Assembly assembly = Assembly.LoadFrom(extraDllFile);
                        extraAssemblies.Add(assembly);
                    }
                }
            }
        }

        internal static string ExtractPath(string text)
        {
            string pattern = "[\"'“”‘’]([^\"'“”‘’]+)[\"'“”‘’]";

            var match = Regex.Match(text, pattern);
            if (match.Success)
            {
                foreach (Group group in match.Groups)
                {
                    if (group.Value.Contains("'") || group.Value.Contains("\"") || group.Value.Contains("“"))
                    {
                        continue;
                    }
                    else
                    {
                        return group.Value;
                    }
                }
            }
            return null;
        }
    }
}
