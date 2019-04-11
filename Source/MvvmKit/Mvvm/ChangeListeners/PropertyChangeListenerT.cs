using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class PropertyChangeListener<T> : PropertyChangeListener
    {
        public new void Observe(object owner, Action a)
        {
            base.Observe(owner, a);
        }

        public void Observe(object owner, Action<T> a)
        {
            base.Observe(owner, a);
        }

        public void Observe(object owner, Action<T, T> a)
        {
            base.Observe(owner, a);
        }

        public new void Unobserve(object owner)
        {
            base.Unobserve(owner);
        }

        public new void Unobserve(object owner, Action a)
        {
            base.Unobserve(owner, a);
        }

        public void Unobserve(object owner, Action<T> a)
        {
            base.Unobserve(owner, a);
        }

        public void Unobserve(object owner, Action<T, T> a)
        {
            base.Unobserve(owner, a);
        }
    }
}
