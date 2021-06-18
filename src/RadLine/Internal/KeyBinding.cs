using System;

namespace RadLine
{
    internal sealed class KeyBinding : IEquatable<ConsoleKeyInfo>
    {
        public ConsoleKey Key { get; }
        public ConsoleModifiers? Modifiers { get; }

        public KeyBinding(ConsoleKey key, ConsoleModifiers? modifiers = null)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public ConsoleKeyInfo AsConsoleKeyInfo()
        {
            return new ConsoleKeyInfo(
                (char)0, Key,
                HasModifier(ConsoleModifiers.Shift),
                HasModifier(ConsoleModifiers.Alt),
                HasModifier(ConsoleModifiers.Control));
        }

        private bool HasModifier(ConsoleModifiers modifier)
        {
            if (Modifiers != null)
            {
                return Modifiers.Value.HasFlag(modifier);
            }

            return false;
        }

        public bool Equals(ConsoleKeyInfo other)
        {
            return other.Modifiers == (Modifiers ?? 0) && other.Key == Key;
        }
    }
}
