using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StringConsole : IConsole
    {
        private List<string> _text = new List<string>();

        public int CursorTop { get; private set; }
        public int CursorLeft { get; private set; }

        public int BufferWidth { get; private set; }

        public int BufferHeight => _text.Count;

        public string this[int index] => _text[index];

        private void _ensureItem(int index, int length)
        {
            while (_text.Count <= index)
            {
                _text.Add("");
            }

            _text[index] = _text[index].PadRight(length, ' ');
            BufferWidth = Math.Max(BufferWidth, length);
        }

        public void SetCursorPosition(int left, int top)
        {
            CursorTop = top;
            CursorLeft = left;
        }

        public void Write(string txt)
        {
            _ensureItem(CursorTop, CursorLeft + txt.Length);
            var cur = _text[CursorTop];

            var left = cur.Substring(0, CursorLeft).PadRight(CursorLeft);
            var right = cur.Substring(CursorLeft + txt.Length);

            _text[CursorTop] = $"{left}{txt}{right}";
            CursorLeft += txt.Length;

            BufferWidth = Math.Max(BufferWidth, _text[CursorTop].Length);
        }

        public void Clear()
        {
            _text = new List<string>();
            CursorLeft = 0;
            CursorTop = 0;
            BufferWidth = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in _text)
            {
                sb.AppendLine(item);
            }

            return sb.ToString();
        }

    }
}
