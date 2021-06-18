using System;

namespace RadLine
{
    public interface IInputSource
    {
        bool ByPassProcessing { get; }

        bool IsKeyAvailable();
        ConsoleKeyInfo ReadKey();
    }
}
