using System;
using System.Collections.Generic;

using CorrugatedIron;

namespace EventStreams.Persistence.Riak {
    internal static class ExceptionExtensions {
        public static RiakPersistenceException With<TResult>(this RiakPersistenceException exception, RiakResult<TResult> result) {
            exception.Data.Add("Result", result);
            return exception;
        }

        public static RiakPersistenceException With<TResult>(this RiakPersistenceException exception, IEnumerable<RiakResult<TResult>> results) {
            exception.Data.Add("Results", results);
            return exception;
        }
    }
}
