namespace RadLine
{
    public sealed class PreviousHistoryCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            context.Submit(SubmitAction.PreviousHistory);
        }
    }
}
