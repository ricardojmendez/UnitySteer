using System;

namespace Tests {
    public class TestLogger : ILogger {

        public string prefix { get; set; }

        public void Log(string message) {
            Console.Error.WriteLine(prefix);
            Console.Error.WriteLine(message);
        }

        public void Log(string message, object[] args) {
            Log(string.Format(message, args));
        }
    }
}

