using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RadLine
{
    public sealed class KeyBindings
    {
        private readonly Dictionary<KeyBinding, Func<LineEditorCommand>> _bindings;
        private readonly Dictionary<Type, KeyBinding> _bindingLookup;

        public int Count => _bindings.Count;

        public KeyBindings()
        {
            _bindings = new Dictionary<KeyBinding, Func<LineEditorCommand>>(new KeyBindingComparer());
            _bindingLookup = new Dictionary<Type, KeyBinding>();
        }

        internal void Add<TCommand>(KeyBinding binding, Func<TCommand> command)
            where TCommand : LineEditorCommand
        {
            if (binding is null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            _bindings[binding] = () => command();
            _bindingLookup[typeof(TCommand)] = binding;
        }

        internal bool TryFindKeyBindings<TCommand>([NotNullWhen(true)] out KeyBinding? binding)
            where TCommand : LineEditorCommand
        {
            return _bindingLookup.TryGetValue(typeof(TCommand), out binding);
        }

        internal void Remove(KeyBinding binding)
        {
            if (binding is null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            _bindings.Remove(binding);
        }

        public void Clear()
        {
            _bindings.Clear();
        }

        public LineEditorCommand? GetCommand(ConsoleKey key, ConsoleModifiers? modifiers = null)
        {
            var candidates = _bindings.Keys as IEnumerable<KeyBinding>;

            if (modifiers != null && modifiers != 0)
            {
                candidates = _bindings.Keys.Where(b => b.Modifiers == modifiers);
            }

            var result = candidates.FirstOrDefault(x => x.Key == key);
            if (result != null)
            {
                if (modifiers == null && result.Modifiers != null)
                {
                    return null;
                }

                if (result != null && _bindings.TryGetValue(result, out var factory))
                {
                    return factory();
                }
            }

            return null;
        }
    }
}
