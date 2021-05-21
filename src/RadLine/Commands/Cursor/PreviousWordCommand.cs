namespace RadLine
{
    public sealed class PreviousWordCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Buffer.MoveToPreviousWord();
        }
    }
}
