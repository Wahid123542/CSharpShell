class Shell
{
    private readonly History _history = new();

    public void Run()
    {
        Console.WriteLine("CSharpShell 1.0 — type 'exit' to quit");

        while (true)
        {
            Console.Write($"{Environment.UserName}:{Directory.GetCurrentDirectory()}$ ");

            string? line = ReadLineWithHistory();
            if (line == null) break;
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            _history.Add(line);

            if (line == "exit") break;

            // Built-ins
            if (line.StartsWith("cd "))
            {
                string path = line[3..].Trim();
                try { Directory.SetCurrentDirectory(path); }
                catch { Console.WriteLine($"cd: {path}: No such directory"); }
                continue;
            }

            if (line == "history")
            {
                foreach (var (entry, idx) in _history.All.Select((e, i) => (e, i + 1)))
                    Console.WriteLine($"  {idx}  {entry}");
                continue;
            }

            try
            {
                var segments = CommandParser.SplitPipeline(line);
                var parsed = segments.Select(CommandParser.Parse).ToList();
                Executor.RunPipeline(parsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private string ReadLineWithHistory()
    {
        var buffer = new System.Text.StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return buffer.ToString();
            }
            else if (key.Key == ConsoleKey.Backspace && buffer.Length > 0)
            {
                buffer.Remove(buffer.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                string? prev = _history.NavigateUp();
                if (prev != null) ReplaceInput(buffer, prev);
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                string? next = _history.NavigateDown();
                if (next != null) ReplaceInput(buffer, next);
            }
            else if (!char.IsControl(key.KeyChar))
            {
                buffer.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    private static void ReplaceInput(System.Text.StringBuilder buffer, string newText)
    {
        // Clear current line visually
        Console.Write(new string('\b', buffer.Length));
        Console.Write(new string(' ', buffer.Length));
        Console.Write(new string('\b', buffer.Length));

        buffer.Clear();
        buffer.Append(newText);
        Console.Write(newText);
    }
}
