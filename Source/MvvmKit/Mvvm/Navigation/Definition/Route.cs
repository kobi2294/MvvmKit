﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Route
    {
        public Type ViewModelType { get; private set; }

        public object Parameter { get; private set; }

        public Type ParameterType { get; private set; }

        public string Name { get; private set; }

        public object Key { get; private set; }

        public RouteParameterMode ParameterMode { get; private set; }

        public Region Region { get; internal set; }

        private Route()
        {

        }

        public static Route To<TVM>(object key, string name = null)
            where TVM: ComponentBase
        {
            if (string.IsNullOrWhiteSpace(name))
                name = key.ToString();

            var res = new Route
            {
                Key = key,
                Name = name,
                ViewModelType = typeof(TVM)
            };

            return res.WithoutParam();
        }

        public Route WithParam<T>(T param)
        {
            ParameterMode = RouteParameterMode.Fixed;
            Parameter = param;
            ParameterType = typeof(T);
            return this;
        }

        public Route WithParam<T>()
        {
            ParameterMode = RouteParameterMode.Variant;
            Parameter = null;
            ParameterType = typeof(T);
            return this;

        }

        public Route WithoutParam()
        {
            ParameterMode = RouteParameterMode.None;
            Parameter = null;
            ParameterType = null;
            return this;
        }




        public static bool operator ==(Route r1, Route r2)
        {
            var isnull1 = ReferenceEquals(r1, null);
            var isnull2 = ReferenceEquals(r2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return Equals(r1.ViewModelType, r2.ViewModelType)
                && Equals(r1.Key, r2.Key)
                && Equals(r1.Parameter, r2.Parameter)
                && Equals(r1.ParameterMode, r2.ParameterMode)
                && Equals(r1.ParameterType, r2.ParameterType)
                && Equals(r1.Region, r2.Region)
                ;
        }

        public static bool operator !=(Route r1, Route r2)
        {
            return !(r1 == r2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Route)) return false;
            return this == (Route)obj;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(ViewModelType, Key, Parameter, ParameterType, ParameterMode, Region);
        }

        public override string ToString()
        {
            string prm = "";
            if (ParameterMode == RouteParameterMode.Fixed) prm = $"{Parameter} ({ParameterType})";
            if (ParameterMode == RouteParameterMode.Variant) prm = $"({ParameterType})";

            return $"Route ({Name}) To {ViewModelType}, Parameter: {ParameterMode} {prm}";
        }
    }
}
