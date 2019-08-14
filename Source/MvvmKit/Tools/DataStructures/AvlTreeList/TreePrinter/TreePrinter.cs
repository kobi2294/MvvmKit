using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class TreePrinter
    {
        class NodeInfo<T>
        {
            public AvlTreeNode<T> Node;
            public string Text;
            public int StartPos;
            public int Size { get { return Text.Length; } }
            public int EndPos { get { return StartPos + Size; } set { StartPos = value - Size; } }
            public NodeInfo<T> Parent, Left, Right;
        }

        public static void PrintToConsole<T>(this AvlTreeNode<T> root, IConsole console,  string textFormat = "{0}", int spacing = 2, int topMargin = 2, int leftMargin = 2)
        {
            root._printToConsole(console, textFormat, spacing, topMargin, leftMargin);
        }

        private static void _printToConsole<T>(this AvlTreeNode<T> root, IConsole console, string textFormat = "{0}", int spacing = 2, int topMargin = 2, int leftMargin = 2)
        {
            if (root == null) return;

            int rootTop = topMargin;
            var last = new List<NodeInfo<T>>();
            var next = root;
            for (int level = 0; next != null; level++)
            {
                var item = new NodeInfo<T> { Node = next, Text = string.Format(textFormat, next.Item) };
                if (level < last.Count)
                {
                    item.StartPos = last[level].EndPos + spacing;
                    last[level] = item;
                }
                else
                {
                    item.StartPos = leftMargin;
                    last.Add(item);
                }
                if (level > 0)
                {
                    item.Parent = last[level - 1];
                    if (next == item.Parent.Node.Left)
                    {
                        item.Parent.Left = item;
                        item.EndPos = Math.Max(item.EndPos, item.Parent.StartPos - 1);
                    }
                    else
                    {
                        item.Parent.Right = item;
                        item.StartPos = Math.Max(item.StartPos, item.Parent.EndPos + 1);
                    }
                }
                next = next.Left ?? next.Right;
                for (; next == null; item = item.Parent)
                {
                    int top = rootTop + 2 * level;
                    _printString(console, item.Text, top, item.StartPos);
                    if (item.Left != null)
                    {
                        _printString(console, "/", top + 1, item.Left.EndPos);
                        _printString(console, "_", top, item.Left.EndPos + 1, item.StartPos);
                    }
                    if (item.Right != null)
                    {
                        _printString(console, "_", top, item.EndPos, item.Right.StartPos - 1);
                        _printString(console, "\\", top + 1, item.Right.StartPos - 1);
                    }
                    if (--level < 0) break;
                    if (item == item.Parent.Left)
                    {
                        item.Parent.StartPos = item.EndPos + 1;
                        next = item.Parent.Node.Right;
                    }
                    else
                    {
                        if (item.Parent.Left == null)
                            item.Parent.EndPos = item.StartPos - 1;
                        else
                            item.Parent.StartPos += (item.StartPos - 1 - item.Parent.EndPos) / 2;
                    }
                }
            }
        }

        private static void _printString(IConsole console, string s, int top, int left, int right = -1)
        {
            console.SetCursorPosition(left, top);
            if (right < 0) right = left + s.Length;
            while (console.CursorLeft < right) console.Write(s);
        }
    }
}
