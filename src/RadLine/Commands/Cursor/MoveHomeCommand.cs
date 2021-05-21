namespace RadLine
{
    public sealed class MoveHomeCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Buffer.MoveHome();
        }
    }
}
