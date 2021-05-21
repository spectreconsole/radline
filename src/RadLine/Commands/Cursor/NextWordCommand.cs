namespace RadLine
{
    public sealed class NextWordCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Buffer.MoveToNextWord();
        }
    }
}
