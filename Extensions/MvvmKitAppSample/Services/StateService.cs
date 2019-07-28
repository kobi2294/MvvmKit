using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Services
{
    public class StateService : BackgroundServiceBase
    {
        public interface State
        {
            bool IsMuted { get; set; }

            IStateList<string> Alarms { get; set; }
        }

        private ServiceStore<State> _store = new ServiceStore<State>(data =>
        {
            data.IsMuted = true;
            data.Alarms.Reset("Alarm 1", "Alarm 2");
        });

        public IStateProperty<bool> IsMuted { get; }
        public IStateCollectionReader<string> Alarms { get; }

        public AsyncEvent<bool> IsMutedChanged { get; }


        public StateService()
        {
            IsMuted = _store.CreateWriter(this, x => x.IsMuted);
            Alarms = _store.CreateReader(this, x => x.Alarms);
            IsMutedChanged = _store.Observe(x => x.IsMuted);
        }


        protected override async Task OnInit()
        {
            await base.OnInit();

            // init logic here
        }

        [InjectionMethod]
        public void Inject()
        {
            // store dependencies here
        }

        public Task Method()
        {
            return Run(async () =>
            {
                await _store.Modify(data =>
                {
                    data.IsMuted = !data.IsMuted;
                    data.Alarms.AddRange("Alarm 1", "Alarm 2");
                });
            });
        }
    }
}
