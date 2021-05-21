using System;
using System.Threading;
using System.Threading.Tasks;

namespace RadLine
{
    public interface IInputSource
    {
        Task<ConsoleKeyInfo?> ReadKey(CancellationToken cancellationToken);
    }
}
