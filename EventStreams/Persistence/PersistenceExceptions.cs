using System;

namespace EventStreams.Persistence {
    using Resources;

    [Serializable]
    public class PersistenceException : ApplicationException {
        public PersistenceException(string message) : base(message) { }
        public PersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class DataVerificationPersistenceException : PersistenceException {
        public DataVerificationPersistenceException(string message) : base(message) { }
        public DataVerificationPersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class DataCorruptionPersistenceException : DataVerificationPersistenceException {
        public long PreviousHashPosition { get; private set; }
        public long CurrentHashPosition { get; private set; }

        public DataCorruptionPersistenceException(long previousHashPosition, long currentHashPosition)
            : base(string.Format(ExceptionStrings.Data_corruption_with_previous_and_current_position, previousHashPosition, currentHashPosition)) {

            PreviousHashPosition = previousHashPosition;
            CurrentHashPosition = currentHashPosition;
        }

        public DataCorruptionPersistenceException(long currentHashPosition)
            : base(string.Format(ExceptionStrings.Data_corruption_with_current_position_only, currentHashPosition)) {

            CurrentHashPosition = currentHashPosition;
        }
    }

    [Serializable]
    public class TruncationCorruptionPersistenceException : DataVerificationPersistenceException {
        public long Offset { get; private set; }

        public TruncationCorruptionPersistenceException(long offset)
            : this(offset, null) { }

        public TruncationCorruptionPersistenceException(long offset, Exception innerException)
            : base(string.Format(ExceptionStrings.Truncation_corruption, offset), innerException) {

            Offset = offset;
        }
    }
}
