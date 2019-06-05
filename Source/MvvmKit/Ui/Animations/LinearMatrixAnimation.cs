using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmKit
{
    public class LinearMatrixAnimation : AnimationTimeline
    {
        public Matrix? From
        {
            set { SetValue(FromProperty, value); }
            get { return (Matrix?)GetValue(FromProperty); }
        }
        public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));

        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix?)GetValue(ToProperty); }
        }
        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));




        public EasingFunctionBase Easing
        {
            get { return (EasingFunctionBase)GetValue(EasingProperty); }
            set { SetValue(EasingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Easing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingProperty =
            DependencyProperty.Register("Easing", typeof(EasingFunctionBase), typeof(LinearMatrixAnimation), new PropertyMetadata(null));



        public override object GetCurrentValue(
            object defaultOriginValue, object defaultDestinationValue, 
            AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return null;
            }

            double progress = animationClock.CurrentProgress.Value;
            if (Easing!= null)
            {
                progress = Easing.Ease(progress);
            }

            Matrix from = From ?? (Matrix)defaultOriginValue;

            if (To.HasValue)
            {
                Matrix to = To.Value;
                Matrix newMatrix = new Matrix(((to.M11 - from.M11) * progress) + from.M11, 0, 0, ((to.M22 - from.M22) * progress) + from.M22,
                                              ((to.OffsetX - from.OffsetX) * progress) + from.OffsetX, ((to.OffsetY - from.OffsetY) * progress) + from.OffsetY);
                return newMatrix;
            }

            return Matrix.Identity;
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(Matrix);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new LinearMatrixAnimation();
        }
    }
}
