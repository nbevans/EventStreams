using System;
using System.IO;

namespace EventStreams.Persistence {
    using Resources;

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
                    throw new DataVerificationPersistenceException(
                        string.Format(
                            ExceptionStrings.Unexpected_length_returned_from_stream_read_while_backtracking,
                            hashBuffer.Length,
                            bytesRead));

                // Advance past the record length indicator.
                _innerStream.Position += sizeof(int);

                // Do a quick sanity check to make sure there is a record-end-indicator where we would expect.
                if (_innerStream.ReadByte() != EventStreamTokens.TailIndicator)
                    throw new DataVerificationPersistenceException(
                        ExceptionStrings.Tail_indicator_byte_not_present_while_backtracking);

                return hashBuffer;

            } finally {
                _innerStream.Position = restorePosition;
            }
        }
    }
}
