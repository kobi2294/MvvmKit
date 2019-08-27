using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MvvmKit
{
    public static class VisualTreeExtensions
    {
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject child)
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return Enumerable.Empty<DependencyObject>();

            return parentObject.Yield().Concat(parentObject.Ancestors());
        }

        public static T Ancestor<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            return parent ?? Ancestor<T>(parentObject);
        }

        public static T LogicalAncstor<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = LogicalTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            return parent ?? LogicalAncstor<T>(parentObject);
        }

        public static IEnumerable<DependencyObject> AllDescendents(this DependencyObject source)
        {
            if (source == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(source); i++)
            {
                var child = VisualTreeHelper.GetChild(source, i);
                yield return child;

                foreach (var subChild in child.AllDescendents())
                {
                    yield return subChild;
                }
            }
        }

        public static IEnumerable<T> AllDescendents<T>(this DependencyObject source)
        {
            return source.AllDescendents().OfType<T>();
        }

        public static Rect BoundingBoxOf(this UIElement source, UIElement element)
        {
            Point relativeLocation = element.TranslatePoint(new Point(0, 0), source);
            return new Rect(relativeLocation, element.RenderSize);
        }

        public static UIElement FindFocusableChild(this UIElement source)
        {
            var child = source.AllDescendents<UIElement>().FirstOrDefault(c => c.Focusable);

            return child;
        }

        public static IEnumerable<DependencyObject> CommonAncestors(this DependencyObject obj1, DependencyObject obj2)
        {
            var ancestors1 = obj1.Ancestors().Reverse();
            var ancestors2 = obj2.Ancestors().Reverse();

            return ancestors1.FirstEquals(ancestors2);
        }

        private static IEnumerable<DependencyObject> _directChildren(this DependencyObject obj)
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);

            for (int i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(obj, i);
            }
        }

        private static IEnumerable<DependencyObject> _childrenByDepth(this DependencyObject obj)
        {
            var _queue = new Queue<DependencyObject>();

            yield return obj;
            _queue.Enqueue(obj);

            while (_queue.Count > 0)
            {
                var currentParent = _queue.Dequeue();

                var childrenEnumerator = currentParent._directChildren().GetEnumerator();

                while (childrenEnumerator.MoveNext())
                {
                    var child = childrenEnumerator.Current;
                    yield return child;
                    _queue.Enqueue(child);
                }
            }
        }

        public static IEnumerable<T> DescendentsByDepth<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return _childrenByDepth(obj).OfType<T>();
        }
    }
}
