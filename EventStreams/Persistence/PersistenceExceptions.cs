using System;

namespace EventStreams.Persistence {
    using Resources;

    [Serializable]
    public class PersistenceException : ApplicationException {
        public PersistenceException(string message) : base(message) { }
        public PersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class StreamNotFoundPersistenceException : PersistenceException {
        public Guid Identifier { get; set; }
        public string EventStoreName { get; set; }

        public StreamNotFoundPersistenceException(Guid identifier, string eventStoreName, Exception innerException)
            : base(string.Format(ExceptionStrings.Stream_not_found, identifier, eventStoreName), innerException) {

            Identifier = identifier;
            EventStoreName = eventStoreName;
        }
    }

    [Serializable]
    public class DataVerificationPersistenceException : PersistenceException {
        public DataVerificationPersistenceException(string message) : base(message) { }
        public DataVerificationPersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class HashVerificationPersistenceException : DataVerificationPersistenceException {
        public long PreviousHashPosition { get; private set; }
        public long CurrentHashPosition { get; private set; }

        public HashVerificationPersistenceException(long previousHashPosition, long currentHashPosition)
            : base(string.Format(ExceptionStrings.Data_corruption_with_previous_and_current_position, previousHashPosition, currentHashPosition)) {

            PreviousHashPosition = previousHashPosition;
            CurrentHashPosition = currentHashPosition;
        }

        public HashVerificationPersistenceException(long currentHashPosition)
            : base(string.Format(ExceptionStrings.Data_corruption_with_current_position_only, currentHashPosition)) {

            CurrentHashPosition = currentHashPosition;
        }
    }

    [Serializable]
    public class TruncationVerificationPersistenceException : DataVerificationPersistenceException {
        public long Offset { get; private set; }

        public TruncationVerificationPersistenceException(long offset)
            : this(offset, null) { }

        public TruncationVerificationPersistenceException(long offset, Exception innerException)
            : base(string.Format(ExceptionStrings.Truncation_corruption, offset), innerException) {

            Offset = offset;
        }
    }

    [Serializable]
    public class IrreparableCorruptionPersistenceException : DataVerificationPersistenceException {

        public IrreparableCorruptionPersistenceException()
            : this(null) { }

        public IrreparableCorruptionPersistenceException(Exception innerException)
            : base(string.Format(ExceptionStrings.Truncation_corruption), innerException) { }
    }
}
