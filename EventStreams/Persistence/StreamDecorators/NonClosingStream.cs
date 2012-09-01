using System;
using System.IO;

namespace EventStreams.Persistence.StreamDecorators {

    /// <summary>
    /// Wraps a stream for all operations except <see cref="Stream.Close"/> and <see cref="Stream.Dispose"/>,
    /// which merely flush the stream and prevent further operations from being carried out using this wrapper.
    /// </summary>
    internal sealed class NonClosingStream : Stream {

        private readonly Stream _stream;
        private bool _closed;

        public Stream BaseStream {
            get { return _stream; }
        }

        public NonClosingStream(Stream stream) {
            if (stream == null) throw new ArgumentNullException("stream");
            _stream = stream;
        }

        private void CheckClosed() {
            if (_closed)
                throw new InvalidOperationException(
                    "The outer decorator of the non-closing inner stream has been closed or disposed.");
        }

        public override IAsyncResult BeginRead(
            byte[] buffer, int offset, int count,
            AsyncCallback callback, object state) {
            CheckClosed();
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(
            byte[] buffer, int offset, int count,
            AsyncCallback callback, object state) {
            CheckClosed();
            return _stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override bool CanRead {
            get { return !_closed && _stream.CanRead; }
        }

        public override bool CanSeek {
            get { return !_closed && _stream.CanSeek; }
        }

        public override bool CanWrite {
            get { return !_closed && _stream.CanWrite; }
        }

        public override void Close() {
            if (!_closed)
                _stream.Flush();
            _closed = true;
        }

        public override int EndRead(IAsyncResult asyncResult) {
            CheckClosed();
            return _stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult) {
            CheckClosed();
            _stream.EndWrite(asyncResult);
        }

        public override void Flush() {
            CheckClosed();
            _stream.Flush();
        }

        public override long Length {
            get {
                CheckClosed();
                return _stream.Length;
            }
        }

        public override long Position {
            get {
                CheckClosed();
                return _stream.Position;
            }
            set {
                CheckClosed();
                _stream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            CheckClosed();
            return _stream.Read(buffer, offset, count);
        }

        public override int ReadByte() {
            CheckClosed();
            return _stream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            CheckClosed();
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            CheckClosed();
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            CheckClosed();
            _stream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value) {
            CheckClosed();
            _stream.WriteByte(value);
        }
    }
}