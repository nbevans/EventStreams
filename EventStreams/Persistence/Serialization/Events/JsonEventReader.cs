using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace EventStreams.Persistence.Serialization.Events {
    internal sealed class JsonEventReader : IEventReader {
        public EventArgs Read(Stream innerStream, Type concreteType) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (concreteType == null) throw new ArgumentNullException("concreteType");

            using (var sr = new StreamReader(new NonClosingStreamWrapper(innerStream), Encoding.UTF8, false, 128))
            using (var jtr = new JsonTextReader(sr)) {
                SetupJsonTextReader(jtr);
                return
                    (EventArgs)
                    new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto }
                        .Deserialize(jtr, concreteType);
            }
        }

        private void SetupJsonTextReader(JsonTextReader jtr) {
            jtr.CloseInput = false;
            jtr.DateParseHandling = DateParseHandling.DateTime;
            jtr.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}
