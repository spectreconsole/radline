using System;
using System.Collections.Generic;

namespace RadLine.Tests
{
    public sealed class TestInputSource : IInputSource
    {
        private readonly Queue<ConsoleKeyInfo> _input;

        public bool ByPassProcessing => true;

        public TestInputSource()
        {
            _input = new Queue<ConsoleKeyInfo>();
        }

        public TestInputSource Push(string input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            foreach (var character in input)
            {
                Push(character);
            }

            return this;
        }

        public TestInputSource Push(char input)
        {
            var control = char.IsUpper(input);
            _input.Enqueue(new ConsoleKeyInfo(input, (ConsoleKey)input, false, false, control));
            return this;
        }

        public TestInputSource Push(ConsoleKey input)
        {
            _input.Enqueue(new ConsoleKeyInfo((char)input, input, false, false, false));
            return this;
        }

        public TestInputSource PushNewLine()
        {
            Push(ConsoleKey.Enter, ConsoleModifiers.Shift);
            return this;
        }

        public TestInputSource PushEnter()
        {
            Push(ConsoleKey.Enter);
            return this;
        }

        public TestInputSource Push(ConsoleKey input, ConsoleModifiers modifiers)
        {
            var shift = modifiers.HasFlag(ConsoleModifiers.Shift);
            var control = modifiers.HasFlag(ConsoleModifiers.Control);
            var alt = modifiers.HasFlag(ConsoleModifiers.Alt);

            _input.Enqueue(new ConsoleKeyInfo((char)0, input, shift, alt, control));
            return this;
        }

        public bool IsKeyAvailable()
        {
            return _input.Count > 0;
        }

        ConsoleKeyInfo IInputSource.ReadKey()
        {
            if (_input.Count == 0)
            {
                throw new InvalidOperationException("No keys available");
            }

            return _input.Dequeue();
        }
    }
}
