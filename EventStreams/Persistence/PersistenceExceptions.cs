using System;

namespace EventStreams.Persistence {
    using Resources;

    [Serializable]
    public class PersistenceException : ApplicationException {
        public PersistenceException(string message) : base(message) { }
        public PersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class DataCorruptionPersistenceException : PersistenceException {
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
}
