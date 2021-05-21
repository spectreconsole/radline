using System;

namespace RadLine
{
    internal sealed class KeyBinding
    {
        public ConsoleKey Key { get; }
        public ConsoleModifiers? Modifiers { get; }

        public KeyBinding(ConsoleKey key, ConsoleModifiers? modifiers = null)
        {
            Key = key;
            Modifiers = modifiers;
        }
    }
}
