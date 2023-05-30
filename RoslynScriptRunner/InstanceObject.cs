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

        public InstanceObject(Type type, object instance)
        {
            this.Type = type;
            this.Instance = instance;
        }

        public Type Type { get => type; set => type = value; }
        public object Instance { get => instance; set => instance = value; }
    }
}
