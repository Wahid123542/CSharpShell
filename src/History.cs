class History
{
    private readonly List<string> _entries = new();
    private int _index = -1;

    public void Add(string cmd)
    {
        if (!string.IsNullOrWhiteSpace(cmd))
            _entries.Add(cmd);
        _index = _entries.Count;
    }

    public string? NavigateUp()
    {
        if (_entries.Count == 0) return null;
        _index = Math.Max(0, _index - 1);
        return _entries[_index];
    }

    public string? NavigateDown()
    {
        if (_index >= _entries.Count - 1) { _index = _entries.Count; return ""; }
        return _entries[++_index];
    }

    public IReadOnlyList<string> All => _entries;
}
