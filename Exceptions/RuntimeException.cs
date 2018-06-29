using System.Runtime.Serialization;

namespace CloxSharp.Exceptions {
    [System.Serializable]
    public class RuntimeException : System.Exception {
        public Token Token { get; }
        public RuntimeException(Token token, string message) : base(message) { 
            Token = token;
        }
    }
}