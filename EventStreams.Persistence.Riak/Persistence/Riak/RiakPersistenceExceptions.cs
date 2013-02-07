using System;
using System.Collections.Generic;

using CorrugatedIron;
using CorrugatedIron.Models;

namespace EventStreams.Persistence.Riak {

    [Serializable]
    public class RiakPersistenceException : PersistenceException {
        public RiakPersistenceException(string message)
            : base(message) { }

        public RiakPersistenceException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class RiakCommitPersistenceException : RiakPersistenceException {
        public IEnumerable<RiakResult<RiakObject>> Results { get; private set; }

        // ReSharper disable PossibleMultipleEnumeration
        public RiakCommitPersistenceException(string message, IEnumerable<RiakResult<RiakObject>> results)
            : base(message) {

            Results = results;
        }
        // ReSharper restore PossibleMultipleEnumeration
    }

    [Serializable]
    public class RiakObjectCreationPersistenceException : RiakPersistenceException {
        public IEnumerable<RiakResult<RiakObject>> Results { get; private set; }
        public IEnumerable<RiakResult> RollbackResults { get; private set; }

        // ReSharper disable PossibleMultipleEnumeration
        public RiakObjectCreationPersistenceException(string message, IEnumerable<RiakResult<RiakObject>> putResults, IEnumerable<RiakResult> rollbackResults)
            : base(message) {

            Results = putResults;
            RollbackResults = rollbackResults;
        }
        // ReSharper restore PossibleMultipleEnumeration
    }

    [Serializable]
    public class RiakObjectWireupPersistenceException : RiakPersistenceException {
        public RiakObjectWireupPersistenceException(string message)
            : base(message) { }

        public RiakObjectWireupPersistenceException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class RiakPointerPersistenceException : RiakPersistenceException {
        public RiakResult<RiakObject> Result { get; private set; }

        public RiakPointerPersistenceException(string message, RiakResult<RiakObject> result)
            : base(message) {

            Result = result;
        }
    }

    [Serializable]
    public class RiakHeadPointerPersistenceException : RiakPointerPersistenceException {
        public RiakHeadPointerPersistenceException(string message, RiakResult<RiakObject> result)
            : base(message, result) { }
    }

    [Serializable]
    public class RiakTailPointerPersistenceException : RiakPointerPersistenceException {
        public RiakTailPointerPersistenceException(string message, RiakResult<RiakObject> result)
            : base(message, result) { }
    }
}
