namespace RadLine
{
    public sealed class NextHistoryCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Submit(SubmitAction.NextHistory);
        }
    }
}
