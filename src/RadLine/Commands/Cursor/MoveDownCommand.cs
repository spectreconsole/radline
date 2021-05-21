namespace RadLine
{
    public sealed class MoveDownCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Submit(SubmitAction.MoveDown);
        }
    }
}
