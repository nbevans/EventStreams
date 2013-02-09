using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace EventStreams.Persistence.Riak.ClusterTools {
    internal static class Plink {
        [DllImport("user32.dll")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        public static string Execute(string scriptFileName, string sessionName) {
            var plink = Directory.GetFiles(Environment.CurrentDirectory, "plink.exe", SearchOption.AllDirectories).SingleOrDefault();
            if (plink == null)
                throw new InvalidOperationException("Plink executable cannot be found in the build folder.");

            var path = Path.GetDirectoryName(plink);
            if (path == null)
                throw new InvalidOperationException("Plink path is null.");

            var fullScriptFileName = Path.Combine(path, scriptFileName);

            var args = string.Format("-v -agent -batch -m \"{0}\" \"{1}\"", fullScriptFileName, sessionName);
            var psi = new ProcessStartInfo(plink, args) { RedirectStandardOutput = true, UseShellExecute = false };

            using (var proc = Process.Start(psi)) {
                while (proc.MainWindowHandle == IntPtr.Zero) {
                    Thread.Sleep(10);
                    proc.Refresh();
                }
                SetWindowText(proc.MainWindowHandle, string.Format("ClusterTools: {0} >> {1}", scriptFileName, sessionName));
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                    throw new InvalidOperationException("Plink process exited with error code: " + proc.ExitCode);

                return proc.StandardOutput.ReadToEnd();
            }
        }
    }
}
