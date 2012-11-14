using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace EventStreams.ReadModelling {

    public class ReadModelContext : IDisposable {
        private static readonly string _keyName = typeof(ReadModelContext).FullName;

        public IReadModel ReadModel { get; private set; }

        public static Stack<IReadModel> Current {
            get { return CallContext.GetData(_keyName) as Stack<IReadModel>; }
        }

        public ReadModelContext(IReadModel readModel) {
            if (readModel == null) throw new ArgumentNullException("readModel");
            ReadModel = readModel;

            Push();
        }

        public void Dispose() {
            Pop();
        }

        private void Push() {
            var stack = CallContext.GetData(_keyName) as Stack<IReadModel> ?? new Stack<IReadModel>();
            stack.Push(ReadModel);
            CallContext.SetData(_keyName, stack);
        }

        private void Pop() {
            var stack = CallContext.GetData(_keyName) as Stack<IReadModel>;
            if (stack != null)
                if (ReferenceEquals(stack.Peek(), ReadModel))
                    stack.Pop();
                else
                    throw new InvalidOperationException(
                        "The read model context is in an invalid state. " +
                        "The call stack has been unwound in an out of sequence fashion. " +
                        "This is usually caused by one or more read model context's not being disposed of at the correct time.");
        }
    }
}
