using Classification;

const int Generations = 100;

Problem problem = Problem.Load("Data.txt", testFraction: 0.3, seed: 1);
Population population = new(problem, 500, 6);

Console.WriteLine($"{problem.Training.Count} training samples, {problem.Test.Count} test samples, {problem.VariableCount} features");

for (int generation = 1; generation <= Generations; generation++)
{
    population.NextGeneration(3, 0.1, 2);
    if (generation % 10 == 0) Print(generation, population.BestIndividual);
}

void Print(int generation, Individual best) =>
    Console.WriteLine($"generation {generation,3}: train {best.Accuracy(problem.Training):P1}  test {best.Accuracy(problem.Test):P1}  {best}");
