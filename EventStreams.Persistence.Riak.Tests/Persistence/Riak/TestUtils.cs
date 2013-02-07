using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace EventStreams.Persistence.Riak {

    internal static class TestUtils {

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod() {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            
            // ReSharper disable PossibleNullReferenceException
            return sf.GetMethod().DeclaringType.Name + "_" + sf.GetMethod().Name;
            // ReSharper restore PossibleNullReferenceException
        }
    }
}
