using System.Diagnostics;

class Executor
{
    public static void RunPipeline(List<ParsedCommand> commands)
    {
        if (commands.Count == 1)
        {
            RunSingle(commands[0]);
            return;
        }

        var processes = new List<Process>();
        Process? prev = null;

        for (int i = 0; i < commands.Count; i++)
        {
            var cmd = commands[i];
            bool isLast = i == commands.Count - 1;

            var psi = BuildStartInfo(cmd);
            psi.RedirectStandardInput = (prev != null);
            psi.RedirectStandardOutput = !isLast;

            var p = Process.Start(psi)!;
            processes.Add(p);

            // Feed previous process stdout into this process stdin
            if (prev != null)
            {
                var prevOut = prev.StandardOutput;
                Task.Run(() =>
                {
                    string output = prevOut.ReadToEnd();
                    p.StandardInput.Write(output);
                    p.StandardInput.Close();
                });
            }

            prev = p;
        }

        processes.Last().WaitForExit();
    }

    private static void RunSingle(ParsedCommand cmd)
    {
        var psi = BuildStartInfo(cmd);

        if (cmd.InputFile != null)
        {
            psi.RedirectStandardInput = true;
        }
        if (cmd.OutputFile != null)
        {
            psi.RedirectStandardOutput = true;
        }

        var p = Process.Start(psi)!;

        if (cmd.InputFile != null)
        {
            var text = File.ReadAllText(cmd.InputFile);
            p.StandardInput.Write(text);
            p.StandardInput.Close();
        }

        if (cmd.OutputFile != null)
        {
            string output = p.StandardOutput.ReadToEnd();
            if (cmd.AppendOutput)
                File.AppendAllText(cmd.OutputFile, output);
            else
                File.WriteAllText(cmd.OutputFile, output);
        }

        if (!cmd.Background)
            p.WaitForExit();
        else
            Console.WriteLine($"[bg] PID {p.Id}");
    }

    private static ProcessStartInfo BuildStartInfo(ParsedCommand cmd)
    {
        return new ProcessStartInfo
        {
            FileName = cmd.Args[0],
            Arguments = string.Join(' ', cmd.Args.Skip(1)),
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardInput = false,
            RedirectStandardError = false,
        };
    }
}
