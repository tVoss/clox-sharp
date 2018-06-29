using System;
using System.IO;
using CloxSharp.Exceptions;
using CloxSharp.Expressions;
using CloxSharp.Visitors;

namespace CloxSharp {
    public class Lox {
        
        private static bool _hadError = false;
        private static bool _hadRuntimeError = false;
        private static readonly Interpreter _interpreter = new Interpreter();

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
                Environment.Exit(65);
            }
            if (_hadRuntimeError) {
                Environment.Exit(70);
            }
        }

        private static void Run(string source) {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expr = parser.Parse();

            if (_hadError) {
                return;
            }

            _interpreter.Interpret(expr);
        }

        public static void Error(int line, string message) {
            Report(line, string.Empty, message);
        }

        public static void Error(Token token, string message) {
            if (token.Type == TokenType.Eof) {
                Report(token.Line, " at end", message);
            } else {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeException e) {
            Console.Error.WriteLine($"{e.Message}\n[line {e.Token.Line}]");
            _hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message) {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }

    }
}
