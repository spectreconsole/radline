namespace RadLine
{
    public sealed class BackspaceCommand : LineEditorCommand
    {
        public override void Execute(LineEditorContext context)
        {
            var removed = context.Buffer.Clear(context.Buffer.Position - 1, 1);
            if (removed == 1)
            {
                context.Buffer.Move(context.Buffer.Position - 1);
            }
        }
    }
}
