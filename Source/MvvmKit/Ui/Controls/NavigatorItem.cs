using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmKit
{
    [TemplateVisualState(GroupName = IsSelectedGroup, Name = IsSelectedFalse)]
    [TemplateVisualState(GroupName = IsSelectedGroup, Name = IsSelectedTrue)]
    public class NavigatorItem : Button
    {
        public const string IsSelectedGroup = nameof(IsSelectedGroup);
        public const string IsSelectedTrue = nameof(IsSelectedTrue);
        public const string IsSelectedFalse = nameof(IsSelectedFalse);

        static NavigatorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigatorItem), 
                new FrameworkPropertyMetadata(typeof(NavigatorItem)));
        }

        #region IsSelected Property

        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(NavigatorItem),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnIsSelectedChanged));

        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var si = d as NavigatorItem;
            bool isSelected = (bool)e.NewValue;

            si._calcState(true);

            if (isSelected)
            {
                si.OnSelected();
            }
            else
            {
                si.OnUnselected();
            }
        }

        protected virtual void OnSelected()
        {
        }

        protected virtual void OnUnselected()
        {
        }

        #endregion

        internal Navigator Owner
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as Navigator;
            }
        }

        private void _calcState(bool useTransition)
        {
            if (IsSelected)
            {
                VisualStateManager.GoToState(this, IsSelectedTrue, useTransition);
            }
            else
            {
                VisualStateManager.GoToState(this, IsSelectedFalse, useTransition);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _calcState(false);
        }

        protected override void OnClick()
        {
            base.OnClick();
            Owner?.ContainerClicked(this);
        }

    }

}
