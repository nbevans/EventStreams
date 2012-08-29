using System;
using System.IO;
using System.Security.Cryptography;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;

    public class EventStreamReader : IDisposable {

        private readonly Stream _innerStream;
        private readonly IEventReader _eventReader;
        private readonly Action<EventStreamReaderState> _beforeStateChange, _afterStateChange;

        public EventStreamReader(Stream innerStream, IEventReader eventReader)
            : this(innerStream, eventReader, null, null) { }

        public EventStreamReader(Stream innerStream, IEventReader eventReader, Action<EventStreamReaderState> beforeStateChange, Action<EventStreamReaderState> afterStateChange) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (eventReader == null) throw new ArgumentNullException("eventReader");
            _innerStream = innerStream;
            _eventReader = eventReader;
            _beforeStateChange = beforeStateChange;
            _afterStateChange = afterStateChange;
        }

        ~EventStreamReader() {
            Dispose(false);
        }

        public IStreamedEvent Next() {
            long previousHashPosition;
            var previousHash =
                new EventStreamBacktracker(_innerStream)
                    .HashOrNull(out previousHashPosition);

            using (var hashAlgo = new ShaHash())
            using (var cryptoStream = new CryptoStream(_innerStream.PreventClosure(), hashAlgo, CryptoStreamMode.Read)) {
                var rc =
                    new StatefulReadContext(
                        new EventStreamReaderContext(_innerStream, cryptoStream, hashAlgo, previousHash),
                        _beforeStateChange,
                        _afterStateChange);

                var recordOffset = _innerStream.Position;
                try {
                    rc.HeadIndicator();
                    rc.HeadRecordLength();
                    rc.Id();
                    rc.Timestamp();
                    rc.ArgumentsType();
                    rc.Body(_eventReader);
                    rc.Hash();
                    rc.TailRecordLength();
                    rc.TailIndicator();

                } catch (EndOfStreamException) {
                    throw new TruncationCorruptionPersistenceException(recordOffset);
                }

                previousHash = rc.StreamHash;
                if (ShaHash.AreNotEqual(previousHash, rc.CurrentHash))
                    if (previousHashPosition > 0)
                        throw new DataCorruptionPersistenceException(previousHashPosition, rc.CurrentHashPosition);
                    else
                        throw new DataCorruptionPersistenceException(rc.CurrentHashPosition);

                return rc.Event;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing)
                GC.SuppressFinalize(this);

            _innerStream.Dispose();
        }

        public void Dispose() {
            Dispose(true);
        }

        private sealed class StatefulReadContext : IEventStreamReaderContext {
            private readonly IEventStreamReaderContext _innerReader;
            private readonly Action<EventStreamReaderState> _beforeStateChange, _afterStateChange;

            public StatefulReadContext(IEventStreamReaderContext innerReader, Action<EventStreamReaderState> beforeStateChange, Action<EventStreamReaderState> afterStateChange) {
                _innerReader = innerReader;
                _beforeStateChange = beforeStateChange;
                _afterStateChange = afterStateChange;
            }

            private void Before(EventStreamReaderState state) {
                if (_beforeStateChange != null)
                    _beforeStateChange(state);
            }

            private void After(EventStreamReaderState state) {
                if (_afterStateChange != null)
                    _afterStateChange(state);
            }

            public IStreamedEvent Event { get { return _innerReader.Event; } }
            public byte[] StreamHash { get { return _innerReader.StreamHash; } }
            public byte[] CurrentHash { get { return _innerReader.CurrentHash; } }
            public long CurrentHashPosition { get { return _innerReader.CurrentHashPosition; } }

            public void HeadIndicator() {
                Before(EventStreamReaderState.HeadIndicator);
                {
                    _innerReader.HeadIndicator();
                }
                After(EventStreamReaderState.HeadIndicator);
            }

            public void HeadRecordLength() {
                Before(EventStreamReaderState.HeadRecordLength);
                {
                    _innerReader.HeadRecordLength();
                }
                After(EventStreamReaderState.HeadRecordLength);
            }

            public void Id() {
                Before(EventStreamReaderState.Id);
                {
                    _innerReader.Id();
                }
                After(EventStreamReaderState.Id);
            }

            public void Timestamp() {
                Before(EventStreamReaderState.Timestamp);
                {
                    _innerReader.Timestamp();
                }
                After(EventStreamReaderState.Timestamp);
            }

            public void ArgumentsType() {
                Before(EventStreamReaderState.ArgumentsType);
                {
                    _innerReader.ArgumentsType();
                }
                After(EventStreamReaderState.ArgumentsType);
            }

            public void Body(IEventReader eventReader) {
                Before(EventStreamReaderState.Body);
                {
                    _innerReader.Body(eventReader);
                }
                After(EventStreamReaderState.Body);
            }

            public void Hash() {
                Before(EventStreamReaderState.Hash);
                {
                    _innerReader.Hash();
                }
                After(EventStreamReaderState.Hash);
            }

            public void TailRecordLength() {
                Before(EventStreamReaderState.TailRecordLength);
                {
                    _innerReader.TailRecordLength();
                }
                After(EventStreamReaderState.TailRecordLength);
            }

            public void TailIndicator() {
                Before(EventStreamReaderState.TailIndicator);
                {
                    _innerReader.TailIndicator();
                }
                After(EventStreamReaderState.TailIndicator);
            }
        }
    }
}
