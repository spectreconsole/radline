namespace RadLine
{
    public sealed class InsertCommand : LineEditorCommand
    {
        private readonly char? _character;
        private readonly string? _text;

        public InsertCommand(char character)
        {
            _character = character;
            _text = null;
        }

        public InsertCommand(string text)
        {
            _text = text ?? string.Empty;
            _character = null;
        }

        public override void Execute(LineEditorContext context)
        {
            var buffer = context.Buffer;

            if (_character != null)
            {
                buffer.Insert(_character.Value);
                buffer.Move(buffer.Position + 1);
            }
            else if (_text != null)
            {
                buffer.Insert(_text);
                buffer.Move(buffer.Position + _text.Length);
            }
        }
    }
}
