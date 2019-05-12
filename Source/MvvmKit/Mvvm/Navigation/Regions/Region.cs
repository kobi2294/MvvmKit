using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Region
    {
        public string Name { get; private set; }

        public List<RegionBehavior> Behaviors { get; } = new List<RegionBehavior>();

        internal RegionsService Owner { get; set; }

        public Region()
        {
        }

        public Region WithName(string name)
        {
            Name = name;
            return this;
        }

        public Region WithBehavior(RegionBehavior behavior)
        {
            Behaviors.Add(behavior);
            return this;
        }

        internal async Task InvokeBehaviorsBeforeNavigation()
        {
            await _invokeOnAllBehaviors(b => b.BeforeNavigation);
        }

        internal async Task InvokeBehaviorsAfterNavigation()
        {
            await _invokeOnAllBehaviors(b => b.AfterNavigation);
        }

        private async Task _invokeOnAllBehaviors(Func<RegionBehavior, Func<RegionManager, Task>> func)
        {
            var manager = Owner[this];
            var tasks = Behaviors.Select(behavior => func(behavior).Invoke(manager));

            await Task.WhenAll(tasks);

        }

        public override string ToString()
        {
            return $"Region: {Name}";
        }
    }
}
