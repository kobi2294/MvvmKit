using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmKit
{
    public class EnsureContext: IImmutable
    {
        private enum CurrentType
        {
            Null,
            Object, 
            List 
        }

        private EnsureContext _parent;
        private object _entity;
        private PropertyInfo _property; // for property
        private int _index; // for list item
        private CurrentType _type;
            

        public EnsureContext(object root)
        {
            _parent = null;
            _entity = root;
            _property = null;
            _index = -1;
            _findTypeOfCurrent();
        }

        public EnsureContext(EnsureContext parent, int index)
        {
            if (parent._type != CurrentType.List) throw new InvalidOperationException();
            _parent = parent;
            _index = index;
            _property = null;

            var source = _parent._entity as IList;
            _entity = source[index];
            _findTypeOfCurrent();
        }

        public EnsureContext(EnsureContext parent, PropertyInfo property)
        {
            if (parent._type != CurrentType.Object) throw new InvalidOperationException();
            _parent = parent;
            _index = -1;
            _property = property;
            _entity = property.GetValue(_parent._entity);
            _findTypeOfCurrent();
        }

        public EnsureContext To(int index)
        {
            return new EnsureContext(this, index);
        }

        public EnsureContext To(PropertyInfo property)
        {
            return new EnsureContext(this, property);
        }

        private void _findTypeOfCurrent()
        {
            if (_entity == null)
            {
                _type = CurrentType.Null;
            }
            else if (_entity is IImmutable)
            {
                _type = CurrentType.Object;
            }
            else if (_entity.GetType().IsImmutableListOfImmutables())
            {
                _type = CurrentType.List;
            }
            else
                throw new InvalidOperationException();
        }

        public static bool CanCreateContextForType(Type type)
        {
            return type.IsImmutableType() || type.IsImmutableListOfImmutables();
        }

        public object Entity => _entity;

        public bool IsList => _type == CurrentType.List;

        public bool IsObject => _type == CurrentType.Object;

        public bool IsNull => _type == CurrentType.Null;

        private IEnumerable<object> _getPathToRoot()
        {
            var current = this;

            while (current != null)
            {
                yield return current._entity;
                current = current._parent;
            }
        }

        public IEnumerable<object> Path => _getPathToRoot().Reverse();

        /// <summary>
        /// Generates a list of EnsureContext objects, including self, and recursively going dowards through all properties 
        /// and all members of immutable lists
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EnsureContext> FlattenSubContexts()
        {
            if (_type == CurrentType.Null)
            {
                return Enumerable.Empty<EnsureContext>();
            }

            var self = this.Yield();

            if (_type == CurrentType.List)
            {
                var list = _entity as IList;
                var descendants = Enumerable.Range(0, list.Count)
                    .Select(index => To(index))
                    .SelectMany(ctxt => ctxt.FlattenSubContexts());

                return self.Concat(descendants);
            } else if (_type == CurrentType.Object)
            {
                var entityType = _entity.GetType();
                var descendants = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                      .Where(prop => CanCreateContextForType(prop.PropertyType))
                                      .Select(prop => To(prop))
                                      .SelectMany(ctxt => ctxt.FlattenSubContexts());
                return self.Concat(descendants);
            }

            return self;
        }

        public object EntityOfType(Type type)
        {
            return _getPathToRoot()
                .FirstOrDefault(entity => (entity != null)  && entity.GetType().IsInheritedFrom(type));
        }

        /// <summary>
        /// Hierarchically applies the new value all the way upwards to the root context
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public object ApplyNewValue(object newValue)
        {
            // if this is the root object, then the new value simply replaces the current value
            if (_parent == null)
            {
                return newValue;
            }

            // if the index is > -1, and the parent is a list, we need to create a new list where the item in the index is the newValue
            // and then apply the result on the parent
            else if (_parent._type == CurrentType.List)
            {
                var list = _parent._entity;
                var type = list.GetType();
                var replaceMethod = type.GetMethod("SetItem");
                var newList = replaceMethod.Invoke(list, new[] { _index, newValue });
                return _parent.ApplyNewValue(newList);
            }

            // if the property is not null, and the parent is an object, we need to create a new parent object, by replacing the value of the property
            // and then apply the result on the parent
            else if (_parent._type == CurrentType.Object)
            {
                var entity = _parent._entity as IImmutable;
                var newEntity = entity.With(_property, newValue);
                return _parent.ApplyNewValue(newEntity);
            }

            else throw new InvalidOperationException();
        }


        public override string ToString()
        {
            var type = _type.ToString();

            var source = "ROOT";
            if (_index >= 0) source = $"[{_index}]";
            if (_property != null) source = $".{_property.Name}";

            var entity = "";
            if (_type == CurrentType.Object) entity = _entity.GetType().Name;
            if (_type == CurrentType.List) entity = _entity.GetType().GetGenericArguments()[0].Name;

            var parent = _parent?.ToString() ?? "";

            return $"{parent}{source} -> {type} {entity}";
        }
    }
}
