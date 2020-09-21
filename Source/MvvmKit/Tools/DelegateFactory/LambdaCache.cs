using MvvmKit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class LambdaCache
    {
        public static readonly ConcurrentDictionary<(MethodBase method, Type delegateType), object> _openDelegates;

        public static readonly ConcurrentDictionary<(MemberInfo member, Type delegateType), object> _memberGetters;

        public static readonly ConcurrentDictionary<(MemberInfo member, Type delegateType), object> _memberSetters;

        public static readonly ConcurrentDictionary<(MethodBase method, Type delegateType, PropertyInfo prop), object> _copyCtors;

        static LambdaCache()
        {
            _openDelegates = new ConcurrentDictionary<(MethodBase method, Type delegateType), object>();
            _memberGetters = new ConcurrentDictionary<(MemberInfo member, Type delegateType), object>();
            _memberSetters = new ConcurrentDictionary<(MemberInfo member, Type delegateType), object>();
            _copyCtors = new ConcurrentDictionary<(MethodBase method, Type delegateType, PropertyInfo prop), object>();
        }

        public static DelegateType AsDelegate<DelegateType>(this MethodBase mb)
            where DelegateType : Delegate
        {
            return _openDelegates.GetOrAdd((mb, typeof(DelegateType)), 
                pair => mb.CompileTo<DelegateType>()) as DelegateType;
        }

        public static Func<TEntity, TValue> AsGetter<TEntity, TValue>(this MemberInfo mi)
        {
            return _memberGetters.GetOrAdd((mi, typeof(Func<TEntity, TValue>)), 
                pair => mi.CompileGetter<TEntity, TValue>()) as Func<TEntity, TValue>;
        }

        public static Action<TEntity, TValue> AsSetter<TEntity, TValue>(this MemberInfo mi)
        {
            return _memberSetters.GetOrAdd((mi, typeof(Action<TEntity, TValue>)),
                pair => mi.CompileSetter<TEntity, TValue>()) as Action<TEntity, TValue>;
        }

        public static Func<TEntity, TProp> AsGetter<TEntity, TProp>(this Expression<Func<TEntity, TProp>> prop)
        {
            return prop.GetProperty().AsGetter<TEntity, TProp>();
        }

        public static Action<TEntity, TProp> AsSetter<TEntity, TProp>(this Expression<Func<TEntity, TProp>> prop)
        {
            return prop.GetProperty().AsSetter<TEntity, TProp>();
        }

        public static Func<TEntity, TProp, TEntity> AsCopyConstructor<TEntity, TProp>(this MethodBase mb, PropertyInfo prop)
        {
            return _copyCtors.GetOrAdd((mb, typeof(Func<TEntity, TProp, TEntity>), prop),
                tpl => mb.CompileCopyConstructor<TEntity, TProp>(prop)) as Func<TEntity, TProp, TEntity>;
        }

        public static Func<TEntity, TProp, TEntity> AsCopyConstructor<TEntity, TProp>(this MethodBase mb, Expression<Func<TEntity, TProp>> prop)
        {
            return mb.AsCopyConstructor<TEntity, TProp>(prop.GetProperty());
        }


        #region AsAction...
        public static Action AsAction(this MethodBase mb)
        {
            return mb.AsDelegate<Action>();
        }

        public static Action<T> AsAction<T>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T>>();
        }

        public static Action<T1, T2> AsAction<T1, T2>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2>>();
        }

        public static Action<T1, T2, T3> AsAction<T1, T2, T3>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2, T3>>();
        }

        public static Action<T1, T2, T3, T4> AsAction<T1, T2, T3, T4>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2, T3, T4>>();
        }

        public static Action<T1, T2, T3, T4, T5> AsAction<T1, T2, T3, T4, T5>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2, T3, T4, T5>>();
        }

        public static Action<T1, T2, T3, T4, T5, T6> AsAction<T1, T2, T3, T4, T5, T6>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2, T3, T4, T5, T6>>();
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7> AsAction<T1, T2, T3, T4, T5, T6, T7>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2, T3, T4, T5, T6, T7>>();
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> AsAction<T1, T2, T3, T4, T5, T6, T7, T8>(this MethodBase mb)
        {
            return mb.AsDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8>>();
        }

        #endregion

        #region AsFunc...

        public static Func<TRes> AsFunc<TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<TRes>>();
        }

        public static Func<T1, TRes> AsFunc<T1, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, TRes>>();
        }

        public static Func<T1, T2, TRes> AsFunc<T1, T2, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, TRes>>();
        }

        public static Func<T1, T2, T3, TRes> AsFunc<T1, T2, T3, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, T3, TRes>>();
        }

        public static Func<T1, T2, T3, T4, TRes> AsFunc<T1, T2, T3, T4, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, T3, T4, TRes>>();
        }

        public static Func<T1, T2, T3, T4, T5, TRes> AsFunc<T1, T2, T3, T4, T5, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, T3, T4, T5, TRes>>();
        }

        public static Func<T1, T2, T3, T4, T5, T6, TRes> AsFunc<T1, T2, T3, T4, T5, T6, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, T3, T4, T5, T6, TRes>>();
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TRes> AsFunc<T1, T2, T3, T4, T5, T6, T7, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, T3, T4, T5, T6, T7, TRes>>();
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TRes> AsFunc<T1, T2, T3, T4, T5, T6, T7, T8, TRes>(this MethodBase mb)
        {
            return mb.AsDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, TRes>>();
        }

        #endregion

        

    }
}
