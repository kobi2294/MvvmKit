using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IViewResolver
    {
        Type GetViewType(Type viewModelType);
    }
}
