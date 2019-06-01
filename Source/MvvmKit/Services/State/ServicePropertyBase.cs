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
}
