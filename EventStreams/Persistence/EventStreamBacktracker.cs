using System;
using System.IO;

namespace EventStreams.Persistence {

    internal class EventStreamBacktracker {

        private readonly Stream _innerStream;

        public EventStreamBacktracker(Stream innerStream) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            _innerStream = innerStream;
        }

        public byte[] HashOrNull() {
            long foo;
            return HashOrNull(out foo);
        }

        public byte[] HashOrNull(out long hashPosition) {
            var peekBackLength = sizeof(byte) + sizeof(int) + ShaHash.ByteLength;
            var peekBackPosition = _innerStream.Position - peekBackLength;
            var restorePosition = _innerStream.Position;

            hashPosition = peekBackPosition;

            if (peekBackPosition < 0)
                return null;

            try {
                _innerStream.Position = peekBackPosition;
                
                // Extract the hash.
                var hashBuffer = new byte[ShaHash.ByteLength];
                int bytesRead;
                if ((bytesRead = _innerStream.Read(hashBuffer, 0, hashBuffer.Length)) != hashBuffer.Length)
                    throw new InvalidOperationException(
                        string.Format(
                            "An unexpected number of bytes were read from the stream. Expected {0}, but got {1}.",
                            hashBuffer.Length,
                            bytesRead));

                // Advance past the record length indicator.
                _innerStream.Position += sizeof(int);

                // Do a quick sanity check to make sure there is a record-end-indicator where we would expect.
                if (_innerStream.ReadByte() != EventStreamTokens.TailIndicator)
                    throw new InvalidOperationException(
                        "The buffer read from the stream does not end with a record suffix; " +
                        "the stream appears to be invalid, malformed or corrupt.");

                return hashBuffer;

            } finally {
                _innerStream.Position = restorePosition;
            }
        }
    }
}
