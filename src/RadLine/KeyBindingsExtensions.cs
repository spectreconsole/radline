using System;

namespace RadLine
{
    public static class KeyBindingsExtensions
    {
        public static void Add<TCommand>(this KeyBindings bindings, ConsoleKey key, ConsoleModifiers? modifiers = null)
            where TCommand : LineEditorCommand, new()
        {
            if (bindings is null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            bindings.Add(new KeyBinding(key, modifiers), () => new TCommand());
        }

        public static void Add<TCommand>(this KeyBindings bindings, ConsoleKey key, Func<TCommand> func)
            where TCommand : LineEditorCommand
        {
            if (bindings is null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            bindings.Add(new KeyBinding(key), () => func());
        }

        public static void Add<TCommand>(this KeyBindings bindings, ConsoleKey key, ConsoleModifiers modifiers, Func<TCommand> func)
            where TCommand : LineEditorCommand
        {
            if (bindings is null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            bindings.Add(new KeyBinding(key, modifiers), () => func());
        }

        public static void Remove(this KeyBindings bindings, ConsoleKey key, ConsoleModifiers? modifiers = null)
        {
            if (bindings is null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            bindings.Remove(new KeyBinding(key, modifiers));
        }
    }
}
