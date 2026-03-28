# 🐚 CSharpShell

A Unix-style command shell built from scratch in C#, demonstrating process management, pipeline chaining, I/O redirection, and command history.

---

## Features

| Feature | Description |
|---|---|
| **Command Execution** | Run any system binary directly from the prompt |
| **Piping** | Chain commands with `\|` — e.g. `ls -la \| grep src` |
| **I/O Redirection** | Redirect input/output with `<`, `>`, and `>>` |
| **Background Processes** | Run commands in the background with `&` |
| **Command History** | Navigate previous commands with ↑ / ↓ arrow keys |
| **Built-in `cd`** | Change directories natively within the shell |
| **Built-in `history`** | Print all commands from the current session |

---

## Demo

```
wahid:/home/wahid$ ls -la | grep .cs
-rw-r--r-- 1 wahid wahid  312 Mar 27 Shell.cs
-rw-r--r-- 1 wahid wahid  198 Mar 27 Executor.cs

wahid:/home/wahid$ echo "hello world" > output.txt
wahid:/home/wahid$ cat < output.txt
hello world

wahid:/home/wahid$ sleep 5 &
[bg] PID 18342

wahid:/home/wahid$ history
  1  ls -la | grep .cs
  2  echo "hello world" > output.txt
  3  cat < output.txt
  4  sleep 5 &
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- Linux or macOS (Windows support limited — system binaries differ)

### Installation

```bash
git clone https://github.com/Wahid123542/CSharpShell.git
cd CSharpShell
dotnet run
```

---

## Project Structure

```
CSharpShell/
├── src/
│   ├── Program.cs          # Entry point and REPL loop
│   ├── Shell.cs            # Core shell logic + input handling
│   ├── CommandParser.cs    # Tokenizer and pipeline parser
│   ├── Executor.cs         # Process spawning, pipes, redirection
│   └── History.cs          # Command history and arrow-key navigation
└── README.md
```

---

## How It Works

**Parsing** — Each input line is first split on `|` into pipeline segments. Each segment is then scanned for redirection operators (`<`, `>`, `>>`) and the background flag (`&`), producing a `ParsedCommand` record.

**Execution** — For single commands, the shell spawns a child process using `System.Diagnostics.Process` and optionally wires up file-based stdin/stdout. For pipelines, each process's stdout is streamed asynchronously into the next process's stdin.

**History** — Keystrokes are intercepted character-by-character using `Console.ReadKey`. Up/down arrow presses navigate an in-memory list, replacing the current input buffer and redrawing the line.

---

## Technical Concepts Demonstrated

- Process spawning and management with `System.Diagnostics.Process`
- Stdin/stdout stream chaining across process boundaries
- OS-level I/O redirection via file streams
- Real-time keyboard input handling without blocking reads
- Clean separation of parsing, execution, and UI concerns

---

## Known Limitations

- Session history is not persisted between runs
- No support for environment variable expansion (`$VAR`)
- No tab completion
- Tested on Linux; macOS should work; Windows behavior depends on available binaries

---

## License

MIT
