using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MvvmKit
{
    [TemplatePart(Name = PartPaintArea, Type = typeof(Shape))]
    [TemplatePart(Name = PartMainContent, Type = typeof(ContentPresenter))]
    public class TransitionContentControl : ContentControl
    {
        private const string PartPaintArea = "PART_PaintArea";
        private const string PartMainContent = "PART_MainContent";

        private Shape _partPaintArea;
        private ContentPresenter _partMainContent;
        private AnimationTimeline _currentAnimation;

        #region IndexPath Property

        public PropertyPath IndexPath
        {
            get { return (PropertyPath)GetValue(IndexPathProperty); }
            set { SetValue(IndexPathProperty, value); }
        }

        public static readonly DependencyProperty IndexPathProperty =
            DependencyProperty.Register("IndexPath", typeof(PropertyPath), typeof(TransitionContentControl), new PropertyMetadata(null));

        #endregion

        #region IndexResolver

        public Func<object, int> IndexResolver
        {
            get { return (Func<object, int>)GetValue(IndexResolverProperty); }
            set { SetValue(IndexResolverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndexResolver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexResolverProperty =
            DependencyProperty.Register("IndexResolver", typeof(Func<object, int>), typeof(TransitionContentControl), new PropertyMetadata(null));

        #endregion

        #region EasingFunction Property

        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(TransitionContentControl), new PropertyMetadata(null));

        #endregion

        #region Orientation Property
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TransitionContentControl), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region Duration Property

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TransitionContentControl), new PropertyMetadata(TimeSpan.FromSeconds(0.5)));

        #endregion



        private int _indexOfContent(object content)
        {
            if (content == null) return -1;

            if (IndexResolver != null)
            {
                return IndexResolver(content);
            }

            if (IndexPath != null)
            {
                return IndexPath.Evaluate<int>(content);
            }

            return 0;
        }
        

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if ((_partPaintArea != null) && (_partMainContent != null))
            {
                int oldIndex = _indexOfContent(oldContent);
                int newIndex = _indexOfContent(newContent);

                TransitionDirection direction;
                if ((oldContent == null) || (newContent == null))
                    direction = TransitionDirection.None;
                else if (oldIndex <= newIndex) direction = TransitionDirection.Forwards;
                else direction = TransitionDirection.Backwards;
                    

                _partPaintArea.Fill = CreateBrushFromVisual(_partMainContent);
                BeginAnimateContentReplacement(direction);

            }

            base.OnContentChanged(oldContent, newContent);
        }

        private void BeginAnimateContentReplacement(TransitionDirection direction)
        {
            var forward = direction != TransitionDirection.Backwards;

            var newContentTransform = new TranslateTransform();
            var oldContentTransform = new TranslateTransform();

            _partPaintArea.RenderTransform = oldContentTransform;
            _partMainContent.RenderTransform = newContentTransform;
            _partPaintArea.Visibility = Visibility.Visible;

            double newFrom, newTo, oldFrom, oldTo;

            if ((forward) && (Orientation == Orientation.Horizontal))
            {
                newFrom = this.ActualWidth;
                newTo = 0;
                oldFrom = 0;
                oldTo = -this.ActualWidth;
            }
            else if ((!forward) && (Orientation == Orientation.Horizontal))
            {
                newFrom = -this.ActualWidth;
                newTo = 0;
                oldFrom = 0;
                oldTo = this.ActualWidth;
            } else if ((forward) && (Orientation == Orientation.Vertical))
            {
                newFrom = this.ActualHeight;
                newTo = 0;
                oldFrom = 0;
                oldTo = -this.ActualHeight;

            }
            else
            {
                // backwards, vertical
                newFrom = -this.ActualHeight;
                newTo = 0;
                oldFrom = 0;
                oldTo = this.ActualHeight;
            }

            // if we are not supposed to move in any direction, than the duration is actually 0
            var duration = (direction == TransitionDirection.None)
                ? new Duration(TimeSpan.Zero)
                : Duration;

            var newAnim = CreateAnimation(newFrom, newTo, duration);
            _currentAnimation = CreateAnimation(oldFrom, oldTo, duration, (s, e) =>
            {
                var ac = s as AnimationClock;
                var anim = ac.Timeline;

                if (_currentAnimation == anim)
                {
                    _partPaintArea.Visibility = Visibility.Hidden;
                }
            });

            if (Orientation == Orientation.Horizontal)
            {
                newContentTransform.BeginAnimation(TranslateTransform.XProperty, newAnim);
                oldContentTransform.BeginAnimation(TranslateTransform.XProperty, _currentAnimation);
            } else
            {
                newContentTransform.BeginAnimation(TranslateTransform.YProperty, newAnim);
                oldContentTransform.BeginAnimation(TranslateTransform.YProperty, _currentAnimation);
            }

        }

        private AnimationTimeline CreateAnimation(double from, double to, Duration duration, EventHandler whenDone = null)
        {
            var anim = new DoubleAnimation(from, to, duration) { EasingFunction = EasingFunction };

            if (whenDone != null)
                anim.Completed += whenDone;
            anim.Freeze();
            return anim;
        }


        private Brush CreateBrushFromVisual(Visual v)
        {
            if (v == null)
            {
                throw new ArgumentNullException(nameof(v));
            }

            var width = (int)ActualWidth;
            var height = (int)ActualHeight;

            if ((width <= 0) || (height <= 0)) return Brushes.Transparent;

            var target = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            target.Render(v);
            var brush = new ImageBrush(target);
            brush.Freeze();
            return brush;
        }

        public override void OnApplyTemplate()
        {
            _partMainContent = GetTemplateChild(PartMainContent) as ContentPresenter;
            _partPaintArea = GetTemplateChild(PartPaintArea) as Shape;

            base.OnApplyTemplate();
        }

        static TransitionContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionContentControl), new FrameworkPropertyMetadata(typeof(TransitionContentControl)));
        }
    }
}
