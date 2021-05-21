namespace RadLine
{
    public sealed class MoveFirstLineCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Submit(SubmitAction.MoveFirst);
        }
    }
}
