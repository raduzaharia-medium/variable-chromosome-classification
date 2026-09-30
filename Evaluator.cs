namespace Classification;

/// <summary>A unary function applied to the value produced by a <see cref="Compositor"/>.</summary>
internal sealed record Evaluator(string Symbol, Func<double, double> Evaluate)
{
    public override string ToString() => Symbol;
}
