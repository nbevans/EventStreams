using System;
using System.IO;
using System.Text;

namespace EventStreams.Persistence {

    internal class EventStreamBacktracker {

        private static readonly int _hashHeaderPrefixLength =
            EventStreamTokens.HashHeaderBytes.Length;

        private static readonly int _hashBase64Length =
            ((new ShaHash().HashSize / 8) + 2) / 3 * 4;

        private readonly Stream _innerStream;

        public EventStreamBacktracker(Stream innerStream) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            _innerStream = innerStream;
        }

        public byte[] HashSeedOrNull() {
            var peekBackLength = _hashHeaderPrefixLength + _hashBase64Length + (EventStreamTokens.NewLineBytes.Length * 2);
            var peekBackPosition = _innerStream.Position - peekBackLength;
            var restorePosition = _innerStream.Position;

            if (peekBackPosition < 0)
                return null;

            try {
                _innerStream.Position = peekBackPosition;

                var buffer = new byte[_hashHeaderPrefixLength + _hashBase64Length];
                int bytesRead;
                if ((bytesRead = _innerStream.Read(buffer, 0, buffer.Length)) != buffer.Length)
                    throw new InvalidOperationException(
                        string.Format(
                            "An unexpected number of bytes were read from the stream. Expected {0}, but got {1}.",
                            buffer.Length,
                            bytesRead));

                if (!StartsWith(buffer, EventStreamTokens.HashHeaderBytes))
                    throw new InvalidOperationException(
                        "The buffer read from the stream does not start with a hash prefix.");

                var str = Encoding.UTF8.GetString(buffer, _hashHeaderPrefixLength, _hashBase64Length);
                return Convert.FromBase64String(str);

            } finally {
                _innerStream.Position = restorePosition;
            }
        }

        private static bool StartsWith(byte[] haystack, byte[] needle) {
            if (haystack.Length < needle.Length)
                return false;

            for (var i = 0; i < needle.Length; i++)
                if (haystack[i] != needle[i])
                    return false;

            return true;
        }
    }
}
