using System;

public interface ILogger {
	void Log(string message);
    void Log(string message, params object[] formatArgs);
    string prefix { get; set; }
}

