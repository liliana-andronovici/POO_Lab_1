using System;

public class ParenthesesValidator
{
    private readonly Logger _logger;

    public ParenthesesValidator(Logger logger)
    {
        _logger = logger;
    }

    public bool IsValid(string input)
    {
        if (input is null)
        {
            _logger.Error("Input \"<null>\" is invalid because it is null.");
            _logger.Info("Validation result: invalid.");
            return false;
        }

        int openParentheses = 0;
        bool hasUnmatchedClosingParenthesis = false;

        foreach (char character in input)
        {
            if (character == '(')
            {
                openParentheses++;
                _logger.Trace("A new opening parenthesis was found.");
            }
            else if (character == ')')
            {
                if (openParentheses == 0)
                {
                    _logger.Warn("A closing parenthesis has no opening parenthesis to match it.");
                    if (!hasUnmatchedClosingParenthesis)
                    {
                        _logger.Error($"Input \"{input}\" is invalid: a closing parenthesis has no match.");
                    }

                    hasUnmatchedClosingParenthesis = true;
                }
                else
                {
                    openParentheses--;
                    _logger.Info("A matching pair was found.");
                }
            }
            // Characters other than parentheses do not affect the result.
        }

        if (openParentheses > 0)
        {
            _logger.Error($"Input \"{input}\" is invalid: {openParentheses} opening parenthesis/parentheses were not closed.");
        }

        bool isValid = openParentheses == 0 && !hasUnmatchedClosingParenthesis;
        _logger.Info($"Validation result: {(isValid ? "valid" : "invalid")}.");
        return isValid;
    }
}

public static class Program
{
    public static void Main()
    {
        Logger.SetFormat("[%t] [%l] %n: %m");
        Logger.SetLevel(LogLevel.TRACE);

        var logger = new Logger("Parentheses");
        var validator = new ParenthesesValidator(logger);

        (string Input, bool Expected)[] testCases =
        {
            ("()", true),
            (")((()))", false),
            ("(", false),
            ("(())((()())())", true)
        };

        foreach (var testCase in testCases)
        {
            bool actual = validator.IsValid(testCase.Input);
            string status = actual == testCase.Expected ? "PASS" : "FAIL";
            Console.WriteLine(
                $"Test \"{testCase.Input}\": expected {testCase.Expected}, got {actual} - {status}");
        }

        Console.WriteLine();
        Console.WriteLine("Changing the shared level to INFO hides TRACE messages:");
        Logger.SetLevel(LogLevel.INFO);
        logger.Trace("This trace message should be hidden.");
        logger.Info("This info message should be visible.");

        Console.WriteLine();
        Console.WriteLine("Extra credit example using the %r placeholder:");
        Logger.SetFormat("[%l] %r - %m");
        logger.Info("Parentheses were checked.");
    }
}
