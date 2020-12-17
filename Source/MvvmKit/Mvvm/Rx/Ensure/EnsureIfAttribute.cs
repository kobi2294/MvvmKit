using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EnsureIfAttribute: Attribute
    {
        public string MethodName { get; set; }

        public bool Value { get; set; }

        public EnsureIfAttribute(string methodName, bool value = true)
        {
            MethodName = methodName;
            Value = value;
        }
    }
}
