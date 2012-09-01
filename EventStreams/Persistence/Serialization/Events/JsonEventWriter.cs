using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace EventStreams.Persistence.Serialization.Events {
    using StreamDecorators;

    internal sealed class JsonEventWriter : IEventWriter {
        public IEventReader Opposite { get { return new JsonEventReader(); } }

        public void Write(Stream innerStream, EventArgs args)
        {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (args == null) throw new ArgumentNullException("args");

            using (var sw = new StreamWriter(innerStream.PreventClosure(), Encoding.UTF8, 1024))
            using (var jtw = new JsonTextWriter(sw)) {
                SetupJsonTextWriter(jtw);
                new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto }
                    .Serialize(jtw, args);
            }
        }

        private void SetupJsonTextWriter(JsonTextWriter jtw) {
            jtw.CloseOutput = false;
            jtw.Formatting = Formatting.Indented;
            jtw.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            jtw.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}
