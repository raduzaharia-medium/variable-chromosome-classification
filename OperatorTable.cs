namespace Classification;

internal static class OperatorTable
{
    public static IReadOnlyList<Operator> Operators { get; } = CreateOperators();
    public static int OperatorCount => Operators.Count;

    private static List<Operator> CreateOperators()
    {
        Compositor[] compositors =
        [
            new("+", operands => operands.Sum()),
            new("-", operands => Fold(operands, (result, operand) => result - operand)),
            new("*", operands => Fold(operands, (result, operand) => result * operand)),
            new("/", operands => Fold(operands, (result, operand) => result / operand)),
            new("^", operands => Fold(operands, Math.Pow)),
        ];

        Evaluator[] evaluators =
        [
            new("+", operand => operand),
            new("-", operand => -operand),
            new("sin", Math.Sin),
            new("cos", Math.Cos),
            new("tan", Math.Tan),
            new("acos", Math.Acos),
            new("asin", Math.Asin),
            new("atan", Math.Atan),
            new("cosh", Math.Cosh),
            new("sqrt", Math.Sqrt),
            new("sqr", operand => operand * operand),
            new("lg", Math.Log10),
            new("exp", Math.Exp),
            new("inv", operand => 1 / operand),
        ];

        return [.. from evaluator in evaluators
                   from compositor in compositors
                   select new Operator(evaluator, compositor)];
    }

    /// <summary>Folds the operands left to right, starting from the first one (0 when there are none).</summary>
    private static double Fold(double[] operands, Func<double, double, double> combine) =>
        operands.Length == 0 ? 0 : operands.Aggregate(combine);
}
