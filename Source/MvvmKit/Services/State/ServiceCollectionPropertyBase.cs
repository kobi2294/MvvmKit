using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ServiceCollectionPropertyBase<T>
    {
        internal ServiceCollectionField<T> Field { get; }
        internal ServiceBase.Runner Runner { get; }
        internal ServiceBase Owner { get; }

        internal ServiceCollectionPropertyBase(ServiceCollectionField<T> field, ServiceBase owner)
        {
            Field = field;
            Owner = owner;
            Runner = owner.GetRunner();
        }

        public AsyncEvent<CollectionChanges<T>> Changed => Field.Changed;

        public Task<T> Get(int index)
        {
            return Runner.Run(() => Field[index]);
        }

        public Task<int> Count()
        {
            return Runner.Run(() => Field.Count);
        }

        public Task<bool> Contains(T item)
        {
            return Runner.Run(() => Field.Contains(item));
        }

        public Task<T> Find(Predicate<T> predicate)
        {
            return Runner.Run(() => Field.Find(predicate));
        }

        public Task<List<T>> FindAll(Predicate<T> predicate)
        {
            return Runner.Run(() => Field.FindAll(predicate));
        }

        public Task<int> FindIndex(Predicate<T> predicate)
        {
            return Runner.Run(() => Field.FindIndex(predicate));
        }

        public Task<List<T>> GetRange(int index, int count)
        {
            return Runner.Run(() => Field.GetRange(index, count));
        }

        public Task<List<T>> Items()
        {
            return Runner.Run(() => Field.Items);
        }

        public Task<int> IndexOf(T value)
        {
            return Runner.Run(() => Field.IndexOf(value));
        }
    }
}
