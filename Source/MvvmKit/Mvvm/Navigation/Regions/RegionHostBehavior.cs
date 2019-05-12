using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmKit
{
    public abstract class RegionHostBehavior : DependencyObject
    {
        internal abstract Task BeforeNavigation(RegionManager manager, ContentControl host);

        internal abstract Task AfterNavigation(RegionManager manager, ContentControl host);

        internal abstract Type HostType
        {
            get;
        }
    }

    // we inherit from DependencyObject so that it gets the DataContext of the associated object
    public abstract class RegionHostBehavior<T> : RegionHostBehavior
        where T : ContentControl
    {
        internal override Type HostType => typeof(T);

        internal override async Task BeforeNavigation(RegionManager manager, ContentControl host)
        {
            await BeforeNavigationOverride(manager, host as T);
        }

        internal override async Task AfterNavigation(RegionManager manager, ContentControl host)
        {
            await AfterNavigationOverride(manager, host as T);
        }

        protected virtual Task BeforeNavigationOverride(RegionManager manager, T host)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterNavigationOverride(RegionManager manager, T host)
        {
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        protected override bool ShouldSerializeProperty(DependencyProperty dp)
        {
            return base.ShouldSerializeProperty(dp);
        }
    }
}
