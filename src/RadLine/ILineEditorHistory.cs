namespace RadLine
{
    public interface ILineEditorHistory
    {
        int Count { get; }
        void Add(string text);
    }
}
