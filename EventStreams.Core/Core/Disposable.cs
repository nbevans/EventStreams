using System;

namespace EventStreams.Core {
    public static class Disposable {
        private static readonly IDisposable _emptyDisposer = new DisposableImpl(null);

        public static IDisposable Empty {
            get { return _emptyDisposer; }
        }

        public static IDisposable Create(Action dispose) {
            return new DisposableImpl(dispose);
        }

        private sealed class DisposableImpl : IDisposable {
            private readonly Action _dispose;

            public DisposableImpl(Action dispose) {
                _dispose = dispose;
            }

            ~DisposableImpl() {
                Dispose(false);
            }

            public void Dispose() {
                Dispose(true);
            }

            private void Dispose(bool disposing) {
                if (_dispose != null)
                    _dispose();

                if (disposing)
                    GC.SuppressFinalize(this);
            }
        }
    }
}
