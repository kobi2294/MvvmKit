using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ServicePropertyBase<T>
    {
        internal ServiceField<T> Field { get; }
        internal ServiceRunner Runner { get; }
        internal ServiceBase Owner { get; }

        internal ServicePropertyBase(ServiceField<T> field, ServiceBase owner)
        {
            Field = field;
            Owner = owner;
            Runner = owner.GetRunner();
        }

        public AsyncEvent<T> Changed => Field.Changed;

        public Task<T> Get()
        {
            return Runner.Run(() =>
            {
                return Field.Value;
            });
        }
    }
}
