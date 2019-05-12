using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class DefaultViewResolver : IViewResolver
    {
        public Type GetViewType(Type viewModelType)
        {
            var name = viewModelType.FullName;

            if (name.EndsWith("Vm"))
            {
                var prefix = name.Substring(0, name.Length - 2);
                var fullName = $"{prefix}View";
                var assemblyQualifedName = $"{fullName}, {viewModelType.Assembly.FullName}";
                var res = Type.GetType(assemblyQualifedName);
                return res;
            }

            return null;
        }
    }
}
