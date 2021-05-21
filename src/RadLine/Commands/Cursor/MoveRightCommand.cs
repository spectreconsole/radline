using System;

namespace RadLine
{
    public sealed class MoveRightCommand : LineEditorCommand
    {
        private readonly int _count;

        public MoveRightCommand()
        {
            _count = 1;
        }

        public MoveRightCommand(int count)
        {
            _count = Math.Max(0, count);
        }

        public override void Execute(LineEditorContext context)
        {
            context.Buffer.MoveRight(_count);
        }
    }
}
