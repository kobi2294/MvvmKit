using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AvlTreeEdge<T>: IEquatable<AvlTreeEdge<T>>
    {
        public AvlTreeNode<T> Source { get; private set; }

        public AvlTreeNodeDirection Direction { get; private set; }

        #region Equatable and operators

        public bool Equals(AvlTreeEdge<T> other)
        {
            if (ReferenceEquals(other, null)) return false;

            return (Source == other.Source)
                && (Direction == other.Direction);
        }

        public override bool Equals(object obj)
        {
            return (obj is AvlTreeEdge<T> edge) && (Equals(edge));
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Source, Direction);
        }

        public static bool operator ==(AvlTreeEdge<T> x, AvlTreeEdge<T> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Equals(y);
        }

        public static bool operator !=(AvlTreeEdge<T> x, AvlTreeEdge<T> y)
        {
            return !(x == y);
        }
        #endregion

        private AvlTreeEdge(AvlTreeNode<T> source, AvlTreeNodeDirection direction)
        {
            Source = source;
            Direction = direction;
        }

        public AvlTreeNode<T> Target()
        {
            if (Source == null) throw new InvalidOperationException("Can not find target of null Edge");

            switch (Direction)
            {
                case AvlTreeNodeDirection.None:
                    return Source;
                case AvlTreeNodeDirection.Parent:
                    return Source.Parent;
                case AvlTreeNodeDirection.Left:
                    return Source.Left;
                case AvlTreeNodeDirection.Right:
                    return Source.Right;
            }

            return null;
        }

        public bool IsChild => (Direction == AvlTreeNodeDirection.Left) || (Direction == AvlTreeNodeDirection.Right);

        public bool IsRoot => (Source == null) && (Direction == AvlTreeNodeDirection.None);

        public bool IsSelf => (Source != null) && (Direction == AvlTreeNodeDirection.None);

        public bool IsParent => (Source != null) && (Direction == AvlTreeNodeDirection.Parent);

        public bool IsChildOrRoot => IsChild || IsRoot;


        public static AvlTreeEdge<T> For(AvlTreeNode<T> source, AvlTreeNodeDirection direction)
        {
            if (source == null) return Root;
            return new AvlTreeEdge<T>(source, direction);
        }

        public static AvlTreeEdge<T> Root { get; } = new AvlTreeEdge<T>(null, AvlTreeNodeDirection.None);
    }

    public static class AVlTreeEdge
    {
        public static AvlTreeEdge<T> For<T>(AvlTreeNode<T> source, AvlTreeNodeDirection direction)
        {
            return AvlTreeEdge<T>.For(source, direction);
        }

        public static AvlTreeEdge<T> EdgeTo<T>(this AvlTreeNode<T> source, AvlTreeNodeDirection direction)
        {
            return For(source, direction);
        }

        public static AvlTreeEdge<T> EdgeLeft<T>(this AvlTreeNode<T> source)
        {
            return source.EdgeTo(AvlTreeNodeDirection.Left);
        }

        public static AvlTreeEdge<T> EdgeRight<T>(this AvlTreeNode<T> source)
        {
            return source.EdgeTo(AvlTreeNodeDirection.Right);
        }

        public static AvlTreeEdge<T> EdgeParent<T>(this AvlTreeNode<T> source)
        {
            return source.EdgeTo(AvlTreeNodeDirection.Parent);
        }

        public static AvlTreeEdge<T> EdgeSelf<T>(this AvlTreeNode<T> source)
        {
            return source.EdgeTo(AvlTreeNodeDirection.None);
        }
    }
}
