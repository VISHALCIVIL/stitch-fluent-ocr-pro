using System.Threading;
using System.Threading.Tasks;

namespace StitchFluentOcrPro.Infrastructure
{
    public struct PauseToken
    {
        private readonly PauseTokenSource? _source;

        internal PauseToken(PauseTokenSource source) => _source = source;

        public bool IsPaused => _source?.IsPaused ?? false;

        public Task WaitWhilePausedAsync(CancellationToken token = default)
        {
            return _source != null ? _source.WaitWhilePausedAsync(token) : Task.CompletedTask;
        }
    }

    public class PauseTokenSource
    {
        private readonly object _lock = new object();
        private TaskCompletionSource<bool>? _tcs;

        public bool IsPaused
        {
            get
            {
                lock (_lock)
                {
                    return _tcs != null;
                }
            }
        }

        public PauseToken Token => new PauseToken(this);

        public void Pause()
        {
            lock (_lock)
            {
                _tcs ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }

        public void Resume()
        {
            TaskCompletionSource<bool>? tcsToResume = null;
            lock (_lock)
            {
                if (_tcs != null)
                {
                    tcsToResume = _tcs;
                    _tcs = null;
                }
            }
            tcsToResume?.TrySetResult(true);
        }

        public Task WaitWhilePausedAsync(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<bool>? tcs;
            lock (_lock)
            {
                tcs = _tcs;
            }

            if (tcs == null)
            {
                return Task.CompletedTask;
            }

            if (cancellationToken.CanBeCanceled)
            {
                return tcs.Task.WaitAsync(cancellationToken);
            }

            return tcs.Task;
        }
    }
}
