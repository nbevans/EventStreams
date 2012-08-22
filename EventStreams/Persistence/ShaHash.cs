using System;
using System.Security.Cryptography;

namespace EventStreams.Persistence {
    internal class ShaHash : SHA1Managed {

        public static readonly int ByteLength = new ShaHash().HashSize / 8;

        public static bool AreNotEqual(byte[] a, byte[] b) {
            return !AreEqual(a, b);
        }

        public static bool AreEqual(byte[] a, byte[] b) {
#if DEBUG
            if (a == null)
                throw new ArgumentNullException("a");

            if (b == null)
                throw new ArgumentNullException("b");

            if (a.Length != ByteLength || b.Length != ByteLength)
                throw new InvalidOperationException(
                    string.Format(
                        "One or both of the hash values are invalid. They must be {0} bytes in length.",
                        ByteLength));
#endif

            if (a.Length != b.Length)
                return false;

            for (var i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;

            return true;
        }
    }
}
