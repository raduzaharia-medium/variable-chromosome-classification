namespace Classification;

internal sealed record Operator(Evaluator Evaluator, Compositor Compositor)
{
    public double Evaluate(double[] point) => Evaluator.Evaluate(Compositor.Compose(point));

    public override string ToString() => $"({Evaluator} {Compositor})";
}
