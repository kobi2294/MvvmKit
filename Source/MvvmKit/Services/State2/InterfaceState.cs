using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class InterfaceState
    {
        public Type Interface { get; private set; }

        private Dictionary<PropertyInfo, object> _values;
        private Dictionary<string, PropertyInfo> _props;
        private HashSet<PropertyInfo> _collectionProps;
        private Dictionary<MethodInfo, PropertyInfo> _getters;
        private Dictionary<MethodInfo, PropertyInfo> _setters;

        public InterfaceState(Type @interface)
        {
            if (!@interface.IsInterface) throw new ArgumentException("Expecting interface type", nameof(@interface));

            Interface = @interface;
            var props = Interface.GetAllProperties();
            _values = props.ToDictionary(p => p, p => p.PropertyType.DefaultValue());
            _props = props.ToDictionary(p => p.Name);

            _getters = props.ToDictionary(p => p.GetMethod);
            _setters = props.ToDictionary(p => p.SetMethod);

            _collectionProps = props
                .Where(p => p.PropertyType.IsGenericOf(typeof(IStateCollection<>)))
                .ToHashSet();

            // find properties that are of type IStateCollection<T> and default them to List<T>
            foreach (var prop in _collectionProps)
            {
                var collectionType = prop.PropertyType;
                var itemType = collectionType.GenericTypeArguments[0];
                var listType = typeof(List<>).MakeGenericType(itemType);
                var list = Activator.CreateInstance(listType);
                _values[prop] = list;
            }
        }

        public (PropertyInfo prop, PropertyMethodRole role) PropertyOf(MethodInfo method)
        {
            if (_getters.ContainsKey(method))
                return (_getters[method], PropertyMethodRole.Getter);

            if (_setters.ContainsKey(method))
                return (_setters[method], PropertyMethodRole.Setter);

            return (null, PropertyMethodRole.None);
        }

        public object this[PropertyInfo prop]
        {
            get => _values[prop];
            set => _values[prop] = value;
        }

        public object this[string propName]
        {
            get => this[_props[propName]];
            set => this[_props[propName]] = value;
        }

        public IEnumerable<PropertyInfo> Properties
        {
            get => _values.Keys;
        }

        public IEnumerable<PropertyInfo> CollectionProperties
        {
            get => _collectionProps.ToArray();
        }

        public bool IsCollection(PropertyInfo prop)
        {
            return _collectionProps.Contains(prop);
        }
    }
}
