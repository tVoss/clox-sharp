using System;
using System.IO;

namespace CloxSharp {
    public class Lox {
        
        private static bool _hadError = false;

        public static void Main(string[] args) {
            switch (args.Length) {
                case 0:
                    RunPrompt();
                    break;
                case 1:
                    RunFile(args[0]);
                    break;
                default:
                    Console.WriteLine("Usage: clox-sharp [script]");
                    break;
            }
        }

        private static void RunPrompt() {
            for (;;) {
                Console.Write("> ");
                Run(Console.ReadLine());
                _hadError = false;
            }
        }

        private static void RunFile(string fileName) {
            var source = File.ReadAllText(fileName);
            Run(source);

            if (_hadError) {
                Environment.Exit(-1);
            }
        }

        private static void Run(string source) {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens) {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message) {
            Report(line, string.Empty, message);
        }

        private static void Report(int line, string where, string message) {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }

    }
}
