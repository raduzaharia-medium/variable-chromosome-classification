namespace Classification;

public sealed class Population
{
    private const int EliteCount = 3;

    public List<Individual> Individuals { get; private set; }
    public int GenerationCount { get; private set; }

    public Individual BestIndividual => Individuals[0];

    public Population(Problem problem, int individualCount, int individualLength)
    {
        Individuals = [.. Enumerable.Range(0, individualCount).Select(_ => new Individual(problem, individualLength))];
        Individuals.Sort();
    }

    public void NextGeneration(int selectionSize, double mutationProbability, int cutPointCount)
    {
        List<Individual> newIndividuals = [.. Individuals.Take(EliteCount)];

        for (int i = EliteCount; i < Individuals.Count; i++)
        {
            (Individual first, Individual second) = TournamentSelection(selectionSize).Crossover(TournamentSelection(selectionSize), cutPointCount);
            Individual[] candidates = [first, second, first.Mutation(mutationProbability), second.Mutation(mutationProbability)];

            newIndividuals.Add(candidates.Min()!);
        }

        newIndividuals.Sort();
        Individuals = newIndividuals;
        GenerationCount++;
    }

    private Individual TournamentSelection(int size) =>
        Enumerable.Range(0, size).Select(_ => Individuals[Random.Shared.Next(Individuals.Count)]).Min()!;
}
