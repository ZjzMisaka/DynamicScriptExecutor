using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynScriptRunner
{
    public class InstanceObject
    {
        private Type type;
        private object instance;
        private RunOption runOption;

        public InstanceObject(Type type, object instance, RunOption runOption)
        {
            this.Type = type;
            this.Instance = instance;
            this.RunOption = runOption;
        }

        public Type Type { get => type; set => type = value; }
        public object Instance { get => instance; set => instance = value; }
        public RunOption RunOption { get => runOption; set => runOption = value; }
    }
}
