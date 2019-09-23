using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MvvmKit
{
    public class ServiceCollectionAdapter<T, TVM>
    {
        private IResolver _resolver;

        public Func<T, TVM, Task> Modifier { get; private set; }

        public Action<CollectionChanges<T>> Logger { get; private set; }

        public ObservableCollection<TVM> Target { get; private set; }

        public IStateCollectionReader<T> Source { get; private set; }

        private BufferBlock<CollectionChanges<T>> _handleQueue;
        private CancellationTokenSource _handleCancelSource;
        private Task _handleLoopTask;


        public ServiceCollectionAdapter(IResolver resolver)
        {
            _resolver = resolver;
        }

        public ServiceCollectionAdapter<T, TVM> From(IStateCollectionReader<T> source)
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

        public ServiceCollectionAdapter<T, TVM> LogWith(Action<CollectionChanges<T>> logger)
        {
            Logger = logger;
            return this;
        }

        public async Task<ServiceCollectionAdapter<T, TVM>> Start()
        {
            _handleQueue = new BufferBlock<CollectionChanges<T>>();
            _handleCancelSource = new CancellationTokenSource();
            _handleLoopTask = _handleLoop();

            await Source.Changed.Subscribe(this, _onEvent);
            return this;
        }

        private Task _onEvent(CollectionChanges<T> arg)
        {
            _handleQueue.Post(arg);
            return Task.CompletedTask;
        }

        private async Task _handleLoop()
        {
            CollectionChanges<T> current = null;

            try
            {
                while(true)
                {
                    current = await _handleQueue.ReceiveAsync(_handleCancelSource.Token);
                    await _handleSingleEvent(current);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async Task Stop()
        {
            await Source.Changed.Unsubscribe(this);
            _handleCancelSource.Cancel();
            await _handleLoopTask;
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
            Target.Insert(index, vm);
            await _readModel(source, vm);
            return vm;
        }

        private async Task _handleSingleEvent(CollectionChanges<T> arg)
        {
            Logger?.Invoke(arg);

            foreach (var change in arg)
            {
                switch (change)
                {
                    case IItemAdded<T> c:
                        await _addAt(c.Item, c.Index);
                        break;
                    case IItemRemoved<T> c:
                        Target.RemoveAt(c.Index);
                        break;
                    case ICleared<T> c:
                        Target.Clear();
                        break;
                    case IReset<T> c:
                        Target.Clear();
                        foreach (var item in c.Items)
                        {
                            await _addAt(item);
                        }
                        break;
                    case IItemReplaced<T> c:
                        var vm = Target[c.Index];
                        await _readModel(c.ToItem, vm);
                        break;
                    case IItemMoved<T> c:
                        Target.Move(c.FromIndex, c.ToIndex);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
