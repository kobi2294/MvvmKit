using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceProperty<T> : ServicePropertyBase<T>
    {
        public ServiceProperty(ServiceField<T> field, ServiceBase owner)
            :base(field, owner)
        {
        }

        public Task Set(T value)
        {
            return Runner.Run(() =>
            {
                return Field.Set(value);
            });
        }

        public static implicit operator ServiceProperty<T>((ServiceField<T> field, ServiceBase service) data)
        {
            return data.field.PropertyGetSet(data.service);
        }

    }
}
