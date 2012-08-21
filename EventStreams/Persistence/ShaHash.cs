using System.Security.Cryptography;

namespace EventStreams.Persistence {
    internal class ShaHash : SHA1Managed {
        public static readonly int ByteLength = new ShaHash().HashSize / 8;
    }
}
