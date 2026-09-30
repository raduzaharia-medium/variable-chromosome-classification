namespace Classification;

/// <summary>Combines all operands collected so far into a single value.</summary>
internal sealed record Compositor(string Symbol, Func<double[], double> Compose)
{
    public override string ToString() => Symbol;
}
