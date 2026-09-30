namespace Classification;

/// <summary>
/// A variable-length chromosome in postfix form: negative genes are variables (-1 is the first input),
/// non-negative genes are indexes into <see cref="OperatorTable.Operators"/>.
/// </summary>
public sealed class Individual : IComparable<Individual>
{
    /// <summary>Weight of the chromosome length in the fitness; small enough to only break ties between equally accurate individuals.</summary>
    private const double LengthPenalty = 0.001;

    private readonly Problem problem;
    private double? fitness;

    public int[] Genes { get; }

    /// <summary>The number of misclassified training samples plus a tiny penalty for length. Lower is better.</summary>
    public double Fitness => fitness ??= Genes.Length * LengthPenalty + TrainingErrors();

    public Individual(Problem problem, int[] genes)
    {
        this.problem = problem;
        Genes = genes;
    }

    public Individual(Problem problem, int length) : this(problem, RandomGenes(problem, length)) { }

    /// <summary>The fraction of the given samples that are classified correctly.</summary>
    public double Accuracy(IReadOnlyList<KnownData> data) => data.Count(known => Evaluate(known.Point) == known.Value) / (double)data.Count;

    public List<int[]> SplitGenes(int cutPointCount)
    {
        List<int> cutPoints = [0, Genes.Length];

        for (int i = 0; i < cutPointCount; i++) cutPoints.Add(Random.Shared.Next(1, Genes.Length - 1));
        cutPoints.Sort();

        return [.. cutPoints.Zip(cutPoints.Skip(1), (start, end) => Genes[start..end])];
    }

    public (Individual First, Individual Second) Crossover(Individual parent, int cutPointCount)
    {
        List<int[]> parts1 = SplitGenes(cutPointCount);
        List<int[]> parts2 = parent.SplitGenes(cutPointCount);
        List<int> genes1 = [];
        List<int> genes2 = [];

        for (int i = 0; i < cutPointCount + 1; i++)
        {
            if (i % 2 == 0)
            {
                genes1.AddRange(parts1[i]);
                genes2.AddRange(parts2[i]);
            }
            else
            {
                genes2.AddRange(parts1[i]);
                genes1.AddRange(parts2[i]);
            }
        }

        return (new Individual(problem, [.. genes1]), new Individual(problem, [.. genes2]));
    }

    public Individual Mutation(double probability)
    {
        int[] newGenes = [.. Genes.Select(gene => Random.Shared.NextDouble() < probability ? RandomGene(problem) : gene)];
        return new Individual(problem, newGenes);
    }

    /// <summary>Returns the predicted class (the sign of the expression), or 0 when the chromosome is not a valid expression at this point.</summary>
    public int Evaluate(int[] point)
    {
        List<double> stack = [];

        foreach (int gene in Genes)
        {
            if (gene < 0) stack.Add(point[-gene - 1]);
            else if (stack.Count > 0)
            {
                double composed = OperatorTable.Operators[gene].Evaluate([.. stack]);

                stack.Clear();
                stack.Add(composed);
            }
        }

        return stack is [double value] && double.IsFinite(value) ? Math.Sign(value) : 0;
    }

    public int CompareTo(Individual? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Fitness.CompareTo(other.Fitness);
    }

    public override string ToString() =>
        string.Join(' ', Genes.Select(gene => gene < 0 ? $"x{-gene}" : OperatorTable.Operators[gene].ToString()));

    private int TrainingErrors() => problem.Training.Count(known => Evaluate(known.Point) != known.Value);

    private static int RandomGene(Problem problem) => Random.Shared.Next(-problem.VariableCount, OperatorTable.OperatorCount);

    private static int[] RandomGenes(Problem problem, int length) => [.. Enumerable.Range(0, length).Select(_ => RandomGene(problem))];
}
