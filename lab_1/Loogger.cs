using System;
using System.Collections.Generic;
using System.Text;

public enum LogLevel
{
    TRACE,
    INFO,
    WARN,
    ERROR
}

public class Logger
{
    private static readonly Dictionary<LogLevel, string[]> RandomMessages = new()
    {
        [LogLevel.TRACE] = new[] { "Checking the next character", "A processing step was reached" },
        [LogLevel.INFO] = new[] { "Operation completed", "Input was checked", "Here is the result" },
        [LogLevel.WARN] = new[] { "An unexpected situation was found", "Please check this input" },
        [LogLevel.ERROR] = new[] { "The operation failed", "The input is invalid" }
    };

    public string Name { get; }

    // These settings belong to the class, so every Logger instance shares them.
    public static LogLevel Level { get; private set; } = LogLevel.INFO;
    public static string Format { get; private set; } = "[%t] [%l] %n: %m";

    public Logger(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A logger name is required.", nameof(name));
        }

        Name = name;
    }

    public static void SetLevel(LogLevel newLevel)
    {
        Level = newLevel;
    }

    public static void SetFormat(string newFormat)
    {
        Format = newFormat ?? string.Empty;
    }

    public void Log(LogLevel messageLevel, string message)
    {
        if ((int)messageLevel < (int)Level)
        {
            return;
        }

        Console.WriteLine(FormatMessage(messageLevel, message ?? string.Empty));
    }

    public void Trace(string message) => Log(LogLevel.TRACE, message);
    public void Info(string message) => Log(LogLevel.INFO, message);
    public void Warn(string message) => Log(LogLevel.WARN, message);
    public void Error(string message) => Log(LogLevel.ERROR, message);

    private string FormatMessage(LogLevel messageLevel, string message)
    {
        string currentTime = DateTime.Now.ToString("HH:mm:ss");
        string randomMessage = GetRandomMessage(messageLevel);
        var result = new StringBuilder();

        for (int i = 0; i < Format.Length; i++)
        {
            if (Format[i] != '%' || i == Format.Length - 1)
            {
                result.Append(Format[i]);
                continue;
            }

            char placeholder = Format[i + 1];
            switch (placeholder)
            {
                case 't':
                    result.Append(currentTime);
                    i++;
                    break;
                case 'm':
                    result.Append(message);
                    i++;
                    break;
                case 'n':
                    result.Append(Name);
                    i++;
                    break;
                case 'l':
                    result.Append(messageLevel);
                    i++;
                    break;
                case 'r':
                    result.Append(randomMessage);
                    i++;
                    break;
                default:
                    // Keep unknown placeholders such as %x unchanged.
                    result.Append('%');
                    break;
            }
        }

        return result.ToString();
    }

    private static string GetRandomMessage(LogLevel level)
    {
        if (!RandomMessages.TryGetValue(level, out string[]? messages))
        {
            return "Log message";
        }

        return messages[Random.Shared.Next(messages.Length)];
    }
}
