using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceCollectionPropertyReadonly<T> : ServiceCollectionPropertyBase<T>
    {
        internal ServiceCollectionPropertyReadonly(ServiceCollectionField<T> field, ServiceBase owner)
            : base(field, owner)
        {

        }

        public static implicit operator ServiceCollectionPropertyReadonly<T>((ServiceCollectionField<T> field, ServiceBase service) data)
        {
            return data.field.PropertyGet(data.service);
        }

    }
}
