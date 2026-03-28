record ParsedCommand(
    string[] Args,
    string? InputFile,
    string? OutputFile,
    bool AppendOutput,
    bool Background
);

class CommandParser
{
    // Splits a full line into pipeline segments, e.g. "ls -la | grep foo"
    public static List<string> SplitPipeline(string line) =>
        line.Split('|').Select(s => s.Trim()).ToList();

    public static ParsedCommand Parse(string segment)
    {
        var tokens = segment.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        string? inputFile = null, outputFile = null;
        bool append = false, background = false;

        // Strip background operator
        if (tokens.LastOrDefault() == "&")
        {
            background = true;
            tokens.RemoveAt(tokens.Count - 1);
        }

        // Parse redirection tokens
        for (int i = tokens.Count - 1; i >= 0; i--)
        {
            if (tokens[i] == ">" && i + 1 < tokens.Count)
            {
                outputFile = tokens[i + 1];
                tokens.RemoveRange(i, 2);
            }
            else if (tokens[i] == ">>" && i + 1 < tokens.Count)
            {
                outputFile = tokens[i + 1]; append = true;
                tokens.RemoveRange(i, 2);
            }
            else if (tokens[i] == "<" && i + 1 < tokens.Count)
            {
                inputFile = tokens[i + 1];
                tokens.RemoveRange(i, 2);
            }
        }

        return new ParsedCommand(tokens.ToArray(), inputFile, outputFile, append, background);
    }
}
