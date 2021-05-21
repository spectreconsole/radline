namespace RadLine
{
    public sealed class MoveEndCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Buffer.MoveEnd();
        }
    }
}
