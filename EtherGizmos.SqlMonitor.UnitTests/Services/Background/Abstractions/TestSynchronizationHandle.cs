using Medallion.Threading;

namespace EtherGizmos.SqlMonitor.UnitTests.Services.Background.Abstractions;

internal class TestSynchronizationHandle : IDistributedSynchronizationHandle
{
    private readonly CancellationTokenSource _handleLostTokenSource;

    public CancellationToken HandleLostToken => _handleLostTokenSource.Token;

    public TestSynchronizationHandle()
    {
        _handleLostTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _handleLostTokenSource.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
