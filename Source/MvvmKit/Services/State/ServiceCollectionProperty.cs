using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceCollectionProperty<T> : ServiceCollectionPropertyBase<T>
    {
        public ServiceCollectionProperty(ServiceCollectionField<T> field, ServiceBase owner)
            :base(field, owner)
        {
        }

        public Task SetAt(int index, T item)
        {
            return Runner.Run(() => Field.SetAt(index, item));
        }

        public Task SetWhere(Predicate<T> predicate, T item)
        {
            return Runner.Run(() => Field.SetWhere(predicate, item));
        }

        public Task Add(T value)
        {
            return Runner.Run(() => Field.Add(value));
        }

        public Task AddRange(IEnumerable<T> values)
        {
            return Runner.Run(() => Field.AddRange(values));
        }

        public Task Clear()
        {
            return Runner.Run(() => Field.Clear());
        }

        public Task Insert(int index, T item)
        {
            return Runner.Run(() => Field.Insert(index, item));
        }

        public Task InsertRange(int index, IEnumerable<T> items)
        {
            return Runner.Run(() => Field.InsertRange(index, items));
        }

        public Task MoveAt(int oldIndex, int newIndex)
        {
            return Runner.Run(() => Field.MoveAt(oldIndex, newIndex));
        }

        public Task MoveItem(T item, int newIndex)
        {
            return Runner.Run(() => Field.MoveItem(item, newIndex));
        }

        public Task MoveWhere(Predicate<T> predicate, int newIndex)
        {
            return Runner.Run(() => Field.MoveWhere(predicate, newIndex));
        }

        public Task Remove(T item)
        {
            return Runner.Run(() => Field.Remove(item));
        }

        public Task RemoveAt(int index)
        {
            return Runner.Run(() => Field.RemoveAt(index));
        }

        public Task Reset(IEnumerable<T> values)
        {
            return Runner.Run(() => Field.Reset(values));
        }

        public static implicit operator ServiceCollectionProperty<T>((ServiceCollectionField<T> field, ServiceBase service) data)
        {
            return data.field.PropertyGetSet(data.service);
        }
    }
}
