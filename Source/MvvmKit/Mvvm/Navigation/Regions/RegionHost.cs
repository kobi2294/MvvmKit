using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmKit
{
    public static class RegionHost
    {
        #region Region of Host property

        public static Region GetRegion(ContentControl obj)
        {
            return (Region)obj.GetValue(RegionProperty);
        }

        public static void SetRegion(ContentControl obj, Region value)
        {
            obj.SetValue(RegionProperty, value);
        }

        // Using a DependencyProperty as the backing store for Region.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionProperty =
            DependencyProperty.RegisterAttached("Region", typeof(Region), typeof(RegionHost),
                new PropertyMetadata(null, OnRegionChanged));

        private static void OnRegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as ContentControl;
            if (cc == null) return;

            var oldRegion = e.OldValue as Region;
            var newRegion = e.NewValue as Region;

            oldRegion?.Owner?[oldRegion].RemoveHost(cc);

            newRegion?.Owner?[newRegion].AddHost(cc);
        }

        #endregion

        #region Content Property Property

        public static string GetContentProperty(ContentControl obj)
        {
            return (string)obj.GetValue(ContentPropertyProperty);
        }

        public static void SetContentProperty(ContentControl obj, string value)
        {
            obj.SetValue(ContentPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentPropertyProperty =
            DependencyProperty.RegisterAttached("ContentProperty", typeof(string), typeof(RegionHost),
                new PropertyMetadata("Content", OnContentPropertyChanged));

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as ContentControl;
            if (cc == null) return;

            var region = GetRegion(cc);
            if (region == null) return;

            var oldVal = e.OldValue as string;
            var newVal = e.NewValue as string;

            region.Owner[region].ChangeHostContentProperty(cc, oldVal, newVal);

        }



        #endregion



        public static RegionHostBehavior GetBehavior(ContentControl obj)
        {
            return (RegionHostBehavior)obj.GetValue(BehaviorProperty);
        }

        public static void SetBehavior(ContentControl obj, RegionHostBehavior value)
        {
            obj.SetValue(BehaviorProperty, value);
        }

        // Using a DependencyProperty as the backing store for Behavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("Behavior", typeof(RegionHostBehavior), typeof(RegionHost),
                new PropertyMetadata(null, OnBehaviorChanged));

        private static void OnBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as ContentControl;
            if (cc == null) return;

            var val = e.NewValue as RegionHostBehavior;
            var allowedType = val.HostType;

            if (!allowedType.IsAssignableFrom(cc.GetType()))
                throw new InvalidOperationException($"Behavior of type {val.GetType().Name} can only be assigned to controls of type {allowedType.Name}");
        }
    }
}
