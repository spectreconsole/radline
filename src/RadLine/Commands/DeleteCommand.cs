namespace RadLine
{
    public sealed class DeleteCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            var buffer = context.Buffer;
            buffer.Clear(buffer.Position, 1);
        }
    }
}
