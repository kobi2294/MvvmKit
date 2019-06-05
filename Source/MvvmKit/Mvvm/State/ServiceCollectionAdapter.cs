using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceCollectionAdapter<T, TVM>
    {
        private IResolver _resolver;

        public Func<T, TVM, Task> Modifier { get; private set; }

        public ObservableCollection<TVM> Target { get; private set; }

        public ServiceCollectionPropertyBase<T> Source { get; private set; }

        public ServiceCollectionAdapter(IResolver resolver)
        {
            _resolver = resolver;
        }


        public ServiceCollectionAdapter<T, TVM> From(ServiceCollectionPropertyBase<T> source)
        {
            Source = source;
            return this;
        }

        public ServiceCollectionAdapter<T, TVM> To(ObservableCollection<TVM> target)
        {
            Target = target;
            return this;
        }

        public ServiceCollectionAdapter<T, TVM> ModifyWith(Func<T, TVM, Task> modifier)
        {
            Modifier = modifier;
            return this;
        }

        public async Task<ServiceCollectionAdapter<T, TVM>> Start()
        {
            await Source.Changed.Subscribe(this, OnSourceChanged);
            return this;
        }

        public async Task Stop()
        {
            await Source.Changed.Unsubscribe(this);
        }

        private Task<TVM> _generateNewItem()
        {
            var res = _resolver.Resolve<TVM>();
            return Task.FromResult(res);
        }

        private async Task<TVM> _readModel(T source, TVM target)
        {
            await Modifier(source, target);
            return target;
        }

        private async Task<TVM> _addAt(T source, int index = -1)
        {
            if (index < 0) index = Target.Count;
            var vm = await _generateNewItem();
            await _readModel(source, vm);
            Target.Insert(index, vm);
            return vm;
        }

        private async Task OnSourceChanged(CollectionChanges<T> arg)
        {
            foreach (var change in arg)
            {
                switch (change)
                {
                    case ItemAdded<T> c:
                        await _addAt(c.Item, c.Index);
                        break;
                    case ItemRemoved<T> c:
                        Target.RemoveAt(c.Index);
                        break;
                    case Cleared<T> c:
                        Target.Clear();
                        break;
                    case Reset<T> c:
                        Target.Clear();
                        foreach (var item in c.Items)
                        {
                            await _addAt(item);
                        }
                        break;
                    case ItemReplaced<T> c:
                        var vm = Target[c.Index];
                        await _readModel(c.ToItem, vm);
                        break;
                    case ItemMoved<T> c:
                        Target.Move(c.FromIndex, c.ToIndex);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
