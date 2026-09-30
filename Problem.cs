namespace Classification;

/// <summary>A labelled data set split into a training part (used for fitness) and a held-out test part (used only for reporting).</summary>
public sealed class Problem
{
    public IReadOnlyList<KnownData> Training { get; }
    public IReadOnlyList<KnownData> Test { get; }
    public int VariableCount { get; }

    public Problem(IReadOnlyList<KnownData> training, IReadOnlyList<KnownData> test)
    {
        Training = training;
        Test = test;
        VariableCount = training[0].Point.Length;
    }

    /// <summary>Loads rows of space-separated integers (the features followed by the label, +1 or -1) and shuffles them into a training and a test set.</summary>
    public static Problem Load(string fileName, double testFraction, int seed)
    {
        Random random = new(seed);
        KnownData[] data =
        [.. File.ReadLines(fileName)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
            .Select(values => new KnownData(values[..^1], values[^1]))
            .OrderBy(_ => random.Next())];

        int testCount = (int)(data.Length * testFraction);
        return new Problem(data[testCount..], data[..testCount]);
    }
}
