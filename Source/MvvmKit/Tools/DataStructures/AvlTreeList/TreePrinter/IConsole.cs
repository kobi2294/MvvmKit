namespace MvvmKit
{
    public interface IConsole
    {
        int BufferHeight { get; }
        int BufferWidth { get; }
        int CursorLeft { get; }
        int CursorTop { get; }

        void Clear();
        void SetCursorPosition(int left, int top);
        void Write(string txt);
    }
}