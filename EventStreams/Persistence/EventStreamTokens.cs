using System;
using System.Text;

namespace EventStreams.Persistence {
    internal static class EventStreamTokens {
        public static readonly string NewLine = "\r\n";
        public static readonly byte[] NewLineBytes = Encoding.UTF8.GetBytes(NewLine);

        public static readonly string Id = "Id";
        public static readonly string Time = "Time";
        public static readonly string Type = "Type";
        public static readonly string Args = "Args";
        public static readonly string Hash = "Hash";

        public static readonly string HeaderSuffix = ":";
        public static readonly char HeaderSuffixWhitespace = ' ';

        public static readonly byte[] HashBytes = Encoding.UTF8.GetBytes(Hash);
        public static readonly byte[] HashHeaderBytes =
            Encoding.UTF8.GetBytes(
                Hash + HeaderSuffix +
                HeaderSuffixWhitespace + HeaderSuffixWhitespace);
    }
}
