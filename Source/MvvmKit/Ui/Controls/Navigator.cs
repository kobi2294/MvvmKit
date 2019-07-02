using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmKit
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(NavigatorItem))]
    [TemplatePart(Name = PartMarker, Type = typeof(FrameworkElement))]
    public class Navigator : Selector
    {
        private const string PartMarker = nameof(PartMarker);
        private FrameworkElement _partMarker;
        private MatrixTransform _markerTransform;

        static Navigator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Navigator),
                new FrameworkPropertyMetadata(typeof(Navigator)));

        }

        #region Command Property
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(Navigator), new PropertyMetadata(null));

        #endregion

        #region DoubleClickCommand Property

        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(Navigator), new PropertyMetadata(null));

        #endregion

        #region Duration Property

        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration",
                typeof(Duration), typeof(Navigator),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.5))));

        #endregion

        #region EasingFunction Property

        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(Navigator), new PropertyMetadata(null));

        #endregion



        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(Navigator), new PropertyMetadata(Brushes.Transparent));




        #region Marker Transformation Logic

        private void _invalidateMarkerPosition(bool animate)
        {
            var selected = ItemContainerGenerator.ContainerFromItem(SelectedItem) as NavigatorItem;

            var shouldShowMarker = (selected != null) && (_partMarker != null) && (selected.IsLoaded) && (_partMarker.IsLoaded);
            var shouldAnimateMarker = shouldShowMarker && animate && _partMarker.Visibility == Visibility.Visible;

            var duration = shouldAnimateMarker ? Duration : new Duration(TimeSpan.Zero);

            if (shouldShowMarker)
            {
                _partMarker.Visibility = Visibility.Visible;
                var matrix = _calcTransformationToTarget(selected);

                _animateMarkerTo(matrix, selected.ActualWidth, selected.ActualHeight, duration);
            } else
            {
                if (_partMarker != null)
                {
                    _partMarker.Visibility = Visibility.Hidden;
                    _partMarker.BeginAnimation(WidthProperty, null);
                    _partMarker.BeginAnimation(HeightProperty, null);
                    _markerTransform.BeginAnimation(MatrixTransform.MatrixProperty, null);
                }
            }

        }

        private void _animateMarkerTo(Matrix translate, double width, double height, Duration duration)
        {
            var widthAnim = new DoubleAnimation(width, duration) { EasingFunction = EasingFunction };
            var heightAnim = new DoubleAnimation(height, duration) { EasingFunction = EasingFunction };
            var transAnim = new LinearMatrixAnimation
            {
                Duration = duration,
                To = translate, 
                Easing = EasingFunction
            };

            _partMarker.BeginAnimation(WidthProperty, widthAnim);
            _partMarker.BeginAnimation(HeightProperty, heightAnim);
            _markerTransform.BeginAnimation(MatrixTransform.MatrixProperty, transAnim);
        }

        private Matrix _calcTransformationToTarget(FrameworkElement target)
        {
            var zeroPoint = target.TranslatePoint(new Point(0, 0), _partMarker);
            var translate = _markerTransform.Value;
            translate.Translate(zeroPoint.X, zeroPoint.Y);
            return translate;
        }

        #endregion



        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NavigatorItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NavigatorItem;
        }

        internal void ContainerClicked(NavigatorItem itemContainer)
        {
            var data = ItemContainerGenerator.ItemFromContainer(itemContainer);
            if (Command != null)
            {
                Command.Execute(data);
            }
        }

        internal void ContainerDoubleClicked(NavigatorItem itemContainer)
        {
            var data = ItemContainerGenerator.ItemFromContainer(itemContainer);
            if (DoubleClickCommand != null)
            {
                DoubleClickCommand.Execute(data);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            _invalidateMarkerPosition(true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _partMarker = GetTemplateChild(PartMarker) as FrameworkElement;

            if (_partMarker != null)
            {
                _partMarker.Width = 0;
                _partMarker.Height = 0;
                _partMarker.VerticalAlignment = VerticalAlignment.Top;
                _partMarker.HorizontalAlignment = HorizontalAlignment.Left;
                _markerTransform = new MatrixTransform(Matrix.Identity);
                _partMarker.RenderTransform = _markerTransform;
            }
        }

        public Navigator()
        {
            Loaded += Navigator_Loaded;
        }


        private void Navigator_Loaded(object sender, RoutedEventArgs e)
        {
            _invalidateMarkerPosition(false);
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _invalidateMarkerPosition(false);
        }
    }

}
