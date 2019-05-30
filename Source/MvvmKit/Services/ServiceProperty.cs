using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ServicePropertyBase<T>
    {
        internal ServiceField<T> State { get; }
        internal ServiceRunner Runner { get; }
        internal ServiceBase Owner { get; }

        internal ServicePropertyBase(ServiceField<T> state, ServiceBase owner)
        {
            State = state;
            Owner = owner;
            Runner = owner.GetRunner();
        }

        public AsyncEvent<T> Changed => State.Changed;

        public Task<T> Get()
        {
            return Runner.Run(() =>
            {
                return State.Value;
            });
        }
    }

    public class ServicePropertyReadonly<T> : ServicePropertyBase<T>
    {
        internal ServicePropertyReadonly(ServiceField<T> field, ServiceBase owner)
            :base(field, owner) {}

        public static implicit operator ServicePropertyReadonly<T>((ServiceField<T> field, ServiceBase service) data)
        {
            return data.field.PropertyGet(data.service);
        }

    }

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
                return State.Set(value);
            }, true);
        }

        public static implicit operator ServiceProperty<T>((ServiceField<T> field, ServiceBase service) data)
        {
            return data.field.PropertyGetSet(data.service);
        }

    }
}
