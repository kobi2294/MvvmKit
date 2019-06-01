using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceField<T>
    {
        private T _value;
        private ServicePropertyBase<T> _prop;

        internal AsyncEvent<T> Changed { get; }



        public T Value => _value;


        public ServiceField(T value)
        {
            _value = value;
            Changed = new AsyncEvent<T>(_value);
        }

        public async Task Set(T value)
        {
            if (!Equals(value, _value))
            {
                _value = value;
                await Changed.Invoke(_value);
            }
        }

        public ServicePropertyReadonly<T> PropertyGet(ServiceBase service)
        {
            var res = _prop as ServicePropertyReadonly<T>;

            if ((res == null) || (res.Owner != service)) 
            {
                res = new ServicePropertyReadonly<T>(this, service);
                _prop = res;
            }
            return res;                
        }

        public ServiceProperty<T> PropertyGetSet(ServiceBase service)
        {
            var res = _prop as ServiceProperty<T>;

            if ((res == null) || (res.Owner != service))
            {
                res = new ServiceProperty<T>(this, service);
                _prop = res;
            }
            return res;
        }

        public static implicit operator ServiceField<T>(T value)
        {
            return new ServiceField<T>(value);
        }

        public static implicit operator T(ServiceField<T> state)
        {
            return state.Value;
        }
    }
}
