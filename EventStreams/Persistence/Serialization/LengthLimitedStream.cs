using System;
using System.IO;

namespace EventStreams.Persistence.Serialization {
    public class LengthLimitedStream : Stream {
        private readonly Stream _innerStream;
        private readonly long _lengthLimit;

        public LengthLimitedStream(Stream innerStream, long count) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            _innerStream = innerStream;
            _lengthLimit = innerStream.Position + count;
        }

        public override void Flush() {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (_lengthLimit - Position > 0) {
                var a = Math.Min(_lengthLimit, Position + count);
                var b = Math.Min(a, count);

                return _innerStream.Read(buffer, offset, (int)b);
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
