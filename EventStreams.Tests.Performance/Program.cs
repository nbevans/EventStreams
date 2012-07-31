using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EventStreams {
    using Core;

    public class Program {
        public static void Main() {
            Console.ForegroundColor = ConsoleColor.White;

#if DEBUG
            ColoredConsole(
                ConsoleColor.Yellow,
                () => Console.WriteLine("WARN: Compiled in DEBUG mode."));
#endif

            if (Debugger.IsAttached) {
                ColoredConsole(
                    ConsoleColor.Yellow,
                    () => Console.WriteLine("WARN: A debugger is attached."));
            }

            var testSuites =
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPerformanceTestSuite) != t)
                    .Where(t => typeof(IPerformanceTestSuite).IsAssignableFrom(t))
                    .Select(Activator.CreateInstance)
                    .Cast<IPerformanceTestSuite>();

            foreach (var testSuite in testSuites) {
                Console.WriteLine("Opening test suite: " + testSuite.GetType().FullName);

                const int batchCount = 5;
                var testNumber = 0;
                foreach (var test in testSuite.GetTests()) {
                    Console.WriteLine("   Test #{0:N0}: {1} ({2:N0} batches / {3:N0} iterations)", testNumber, test.Method.Name.PadRight(20), batchCount, testSuite.Repeat);

                    for (var i = 0; i < batchCount; ++i) {
                        var sw = Stopwatch.StartNew();

                        for (var j = 0; j < testSuite.Repeat; ++j)
                            test();

                        sw.Stop();

                        Console.WriteLine("      " + sw.Elapsed);
                    }

                    var sw2 = Stopwatch.StartNew();
                    test();
                    sw2.Stop();
                    Console.WriteLine("      " + sw2.Elapsed + " * single exec");

                    testNumber++;
                }
            }

            ColoredConsole(
                ConsoleColor.Green,
                () => Console.WriteLine("Press any key to exit."));

            Console.ReadKey();
        }

        private static void ColoredConsole(ConsoleColor color, Action action) {
            var revertColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            action();
            Console.ForegroundColor = revertColor;
        }
    }
}
