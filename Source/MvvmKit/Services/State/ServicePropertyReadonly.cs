using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServicePropertyReadonly<T> : ServicePropertyBase<T>
    {
        internal ServicePropertyReadonly(ServiceField<T> field, ServiceBase owner)
            : base(field, owner) { }

        public static implicit operator ServicePropertyReadonly<T>((ServiceField<T> field, ServiceBase service) data)
        {
            return data.field.PropertyGet(data.service);
        }

    }
}
