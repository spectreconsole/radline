using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RadLine
{
    internal sealed class InputBuffer
    {
        private readonly IInputSource _source;
        private readonly Queue<ConsoleKeyInfo> _queue;
        private KeyBinding? _newLineBinding;
        private KeyBinding? _submitBinding;

        public InputBuffer(IInputSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _queue = new Queue<ConsoleKeyInfo>();
        }

        public void Initialize(KeyBindings bindings)
        {
            bindings.TryFindKeyBindings<NewLineCommand>(out _newLineBinding);
            bindings.TryFindKeyBindings<SubmitCommand>(out _submitBinding);
        }

        public async Task<ConsoleKeyInfo?> ReadKey(bool multiline, CancellationToken cancellationToken)
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }

            // Wait for the user to enter a key
            var key = await ReadKeyFromSource(wait: true, cancellationToken);
            if (key == null)
            {
                return null;
            }
            else
            {
                _queue.Enqueue(key.Value);
            }

            if (_source.IsKeyAvailable())
            {
                // Read all remaining keys from the buffer
                await ReadRemainingKeys(multiline, cancellationToken);
            }

            // Got something?
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }

            return null;
        }

        private async Task ReadRemainingKeys(bool multiline, CancellationToken cancellationToken)
        {
            var keys = new Queue<ConsoleKeyInfo>();

            while (true)
            {
                var key = await ReadKeyFromSource(wait: false, cancellationToken);
                if (key == null)
                {
                    break;
                }

                keys.Enqueue(key.Value);
            }

            if (keys.Count > 0)
            {
                // Process the input when we're somewhat sure that
                // the input has been automated in some fashion,
                // and the editor support multiline. The input source
                // can bypass this kind of behavior, so we need to check
                // it as well to see if we should do any processing.
                var shouldProcess = multiline && keys.Count >= 5 && !_source.ByPassProcessing;

                while (keys.Count > 0)
                {
                    var key = keys.Dequeue();

                    if (shouldProcess && _submitBinding != null && _newLineBinding != null)
                    {
                        // Is the key trying to submit?
                        if (_submitBinding.Equals(key))
                        {
                            // Insert a new line instead
                            key = _newLineBinding.AsConsoleKeyInfo();
                        }
                    }

                    _queue.Enqueue(key);
                }
            }
        }

        private async Task<ConsoleKeyInfo?> ReadKeyFromSource(bool wait, CancellationToken cancellationToken)
        {
            if (wait)
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    if (_source.IsKeyAvailable())
                    {
                        break;
                    }

                    await Task.Delay(5, cancellationToken).ConfigureAwait(false);
                }
            }

            if (_source.IsKeyAvailable())
            {
                return _source.ReadKey();
            }

            return null;
        }
    }
}
