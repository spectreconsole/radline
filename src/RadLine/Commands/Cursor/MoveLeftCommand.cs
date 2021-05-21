using System;

namespace RadLine
{
    public sealed class MoveLeftCommand : LineEditorCommand
    {
        private readonly int _count;

        public MoveLeftCommand()
        {
            _count = 1;
        }

        public MoveLeftCommand(int count)
        {
            _count = Math.Max(0, count);
        }

        public override void Execute(LineEditorContext context)
        {
            context.Buffer.MoveLeft(_count);
        }
    }
}
