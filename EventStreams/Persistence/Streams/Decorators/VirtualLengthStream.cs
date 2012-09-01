using System;
using System.IO;

namespace EventStreams.Persistence.Streams.Decorators {

    internal sealed class VirtualLengthStream : Stream {
        private readonly Stream _innerStream;
        private readonly long _lengthLimit;
        private long _bytesRead;

        public VirtualLengthStream(Stream innerStream, long length) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            _innerStream = innerStream;
            _lengthLimit = length;
        }

        public override void Flush() {
            _innerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (_lengthLimit - _bytesRead > 0) {
                var a = Math.Min(_lengthLimit, _bytesRead + count);
                var b = Math.Min(a, count);
                var r = _innerStream.Read(buffer, offset, (int)b);

                _bytesRead += r;
                return r;
            }

            return 0;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }

        public override bool CanRead {
            get { return _innerStream.CanRead; }
        }

        public override bool CanSeek {
            get { return _innerStream.CanSeek; }
        }

        public override bool CanWrite {
            get { return false; }
        }

        public override long Length {
            get { return _lengthLimit; }
        }

        public override long Position {
            get { return _innerStream.Position; }
            set { _innerStream.Position = value; }
        }
    }
}
