namespace RadLine;

public sealed class CancelCommand : LineEditorCommand
{
    public override void Execute(LineEditorContext context)
    {
        context.Buffer.Reset();
        context.Submit(SubmitAction.Cancel);
    }
}