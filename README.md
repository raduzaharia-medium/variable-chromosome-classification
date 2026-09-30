# Variable-Chromosome Classification

A console demo that evolves a small mathematical expression able to classify feature vectors into two classes (+1 / −1).

## What it does

`Program.cs` loads labelled samples from `Data.txt` (34 integer features each, label `1` or `-1`), shuffles them into a 70% training set and a 30% held-out test set, and evolves a population for 100 generations. Every 10 generations it prints the best individual's accuracy on both sets and its expression:

```
generation  50: train 86.2%  test 81.1%  (exp ^) x6 x1 (cosh *) ...
```

Only the training set influences the search; the test accuracy shows how well the result generalises. A large gap between the two means the expression is overfitting.

## How it works

- **Individual**: a chromosome of `int` genes in postfix (stack) order.
  - A **negative** gene `-k` is the variable for feature `k` (`-1` is the first feature).
  - A **non-negative** gene is an index into the operator table.
- **Operators**: each is an *evaluator* applied to a *compositor*. There are 5 compositors (`+ - * / ^`, which fold all the values collected so far into one) and 14 evaluators (identity, negate, `sin`, `cos`, `tan`, `acos`, `asin`, `atan`, `cosh`, `sqrt`, `sqr`, `lg`, `exp`, `inv`), giving 70 operators. An operator is written like `(sin +)`.
- **Evaluation**: genes are read left to right. Variables push their value; an operator collapses everything pushed so far into a single value. A chromosome gives a prediction only if it ends with exactly one finite value; the predicted class is the sign of that value. Otherwise, for that sample, it predicts nothing and counts as wrong.
- **Fitness** (lower is better): the number of misclassified training samples, plus 0.001 per gene. The length term is only a tie-breaker that prefers shorter expressions among equally accurate ones.
- **Selection**: tournament selection (best of *k* random individuals).
- **Crossover**: multi-point. Both parents are cut at random points and the pieces are interleaved. Cut points differ per parent, so children can have different lengths than their parents, hence *variable chromosome*.
- **Mutation**: each gene is replaced by a random gene with the given probability.
- **Generation step**: the 3 best individuals are kept (elitism). Each other slot is filled by crossing two tournament winners, mutating both children, and keeping the best of the four candidates. The population is then sorted.

The settings in `Program.cs` are: population 500, initial length 6, tournament size 3, mutation probability 0.1, 2 cut points, 100 generations.

## Data

`Data.txt` has one sample per line: 34 integers (the features) followed by the label (`1` or `-1`), separated by spaces. The number of features is taken from the first row.

The bundled file is **synthetic**: 300 rows generated so that the label is `+1` when `x1² > x2` (the first feature squared exceeds the second) and `-1` otherwise. The other 32 features are mostly zero or small noise. The exact rule is expressible as `x1 (sqr +) x2 (+ -)`, so you can check what the search finds against a known answer. The search normally reaches 80-90% training accuracy in 100 generations without recovering the rule exactly, and the test accuracy is usually several points lower. To try real data, replace `Data.txt` with your own file in the same format.

## Running

Requires the .NET 10 SDK.

```bash
dotnet run
```

## Code map

| File | Purpose |
| --- | --- |
| `Program.cs` | Loads the data, runs the evolution, prints progress |
| `Individual.cs` | Chromosome, evaluation, fitness, crossover, mutation |
| `Population.cs` | Tournament selection, elitism, next generation |
| `Problem.cs` | Loads `Data.txt` and splits it into training and test sets |
| `OperatorTable.cs` | The operator table |
| `Operator.cs`, `Compositor.cs`, `Evaluator.cs` | Operator building blocks |
| `KnownData.cs` | A labelled sample |
