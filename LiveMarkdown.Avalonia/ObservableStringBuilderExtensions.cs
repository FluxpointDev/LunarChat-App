namespace LiveMarkdown.Avalonia;

public static class ObservableStringBuilderExtensions
{
    extension(ObservableStringBuilder builder)
    {
        public IDisposable SubscribeAppend(IObservable<string> source)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (source is null) throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousObserver<string>(
                onNext: value => builder.Append(value),
                onError: ex => throw ex,
                onCompleted: () => { });

            return source.Subscribe(observer);
        }

        public IDisposable SubscribeAppendLine(IObservable<string> source)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (source is null) throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousObserver<string>(
                onNext: value => builder.AppendLine(value),
                onError: ex => throw ex,
                onCompleted: () => { });

            return source.Subscribe(observer);
        }

        public async Task EnumerateAppendAsync(
            IAsyncEnumerable<string> asyncEnumerable,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (asyncEnumerable is null) throw new ArgumentNullException(nameof(asyncEnumerable));

            await foreach (var line in asyncEnumerable.WithCancellation(cancellationToken))
            {
                builder.Append(line);
                if (timeSpan.HasValue)
                {
                    await Task.Delay(timeSpan.Value, cancellationToken);
                }
            }
        }

        public async Task EnumerateAppendLineAsync(
            IAsyncEnumerable<string> asyncEnumerable,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (asyncEnumerable is null) throw new ArgumentNullException(nameof(asyncEnumerable));

            await foreach (var line in asyncEnumerable.WithCancellation(cancellationToken))
            {
                builder.AppendLine(line);
                if (timeSpan.HasValue)
                {
                    await Task.Delay(timeSpan.Value, cancellationToken);
                }
            }
        }
    }

    private sealed class AnonymousObserver<T>(Action<T>? onNext, Action<Exception>? onError, Action? onCompleted) : IObserver<T>
    {
        private bool _isCompleted;

        public void OnNext(T value)
        {
            if (!_isCompleted)
                onNext?.Invoke(value);
        }

        public void OnError(Exception error)
        {
            if (_isCompleted) return;
            _isCompleted = true;
            onError?.Invoke(error);
        }

        public void OnCompleted()
        {
            if (_isCompleted) return;
            _isCompleted = true;
            onCompleted?.Invoke();
        }
    }
}
