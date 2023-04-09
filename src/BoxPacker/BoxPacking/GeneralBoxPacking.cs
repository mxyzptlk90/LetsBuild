using BoxPacker;

public static partial class BoxPacking
{
    public static IEnumerable<IEnumerable<int>> GetCubesThatCanFillBoxRecursivly((int h, int w, int l) box, IndexedStack<int> indexedCubes) {
        int boxVolume = box.l * box.h * box.w;
        int maxFit = Math.Min(box.l, Math.Min(box.h, box.w));

        List<List<int>> possibleCombinations = new();

        // We will create inner function here to use benefits of closure
        // Here program creates combinations of cubes to determine minimal count
        Action<int, int, List<int>> combineCubes = null;
        combineCubes = (int position, int volume, List<int> combination) => {
            // Exit clause, if volume is 0 it means this combination of cubes fills box
            // Possible solution
            if (volume == 0) {
                possibleCombinations.Add(new(combination));
                return;
            }

            // If cube that is added doesn't fit into a box or number of cubes in this combination is equal to the total number of cubes
            // Not a solution
            if (volume < 0 || possibleCombinations.Any(c => c.Count == indexedCubes.Count)) {
                return;
            }

            var previous = -1;

            // Program iterates through cubes as array and creates all possible combinations
            for (var i = position; i < indexedCubes.Count; i++) {
                var cubeEdge = indexedCubes[i];
                if (cubeEdge == previous || cubeEdge > maxFit) {
                    continue;
                }

                combination.Add(cubeEdge);
                combineCubes(i + 1, volume - (int)Math.Pow(cubeEdge, 3), combination);
                combination.RemoveAt(combination.Count - 1);

                previous = cubeEdge;

                if (possibleCombinations.Any(c => c.Count == indexedCubes.Count)) {
                    return;
                }
            }
        };

        combineCubes(0, boxVolume, new List<int>());
        return possibleCombinations;
    }

    public static IEnumerable<List<int>> GetCubesThatCanFillBoxIteratively((int h, int w, int l) box, IndexedStack<int> indexedCubes) {
        int boxVolume = box.l * box.h * box.w;
        int maxFit = Math.Min(box.l, Math.Min(box.h, box.w));

        List<List<int>> possibleCombinations = new();

        // Create a stack to store the state at each stage
        Stack<(int currentCount, int position, int volume, List<int> combination)> stateStack = new();
        // Initial state
        stateStack.Push((0, 0, boxVolume, new()));

        while (stateStack.Count > 0) {
            (int currentCount, int position, int volume, List<int> combination) = stateStack.Pop();

            // Exit clause, if volume is 0 it means this combination of cubes fills box
            // Possible solution
            if (volume == 0) {
                possibleCombinations.Add(combination);
                continue;
            }

            // If cube that is added doesn't fit into a box or number of cubes in any combination is the number of all cubes
            // Not a solution
            if (volume < 0 || possibleCombinations.Any(c => c.Count() == indexedCubes.Count)) {
                continue;
            }

            var previous = -1;

            // We iterate through all cubes, create unique combination and push new stage into stack
            for (var i = 1; i <= indexedCubes.Count - position; i++) {
                var cubeEdge = indexedCubes[^i];
                if (cubeEdge == previous || cubeEdge > maxFit) {
                    continue;
                }

                combination.Add(cubeEdge);
                stateStack.Push((currentCount + 1, position + 1, volume - (int)Math.Pow(cubeEdge, 3), new(combination)));
                combination.RemoveAt(combination.Count - 1);

                previous = cubeEdge;

                if (possibleCombinations.Any(c => c.Count == indexedCubes.Count)) {
                    break;
                }
            }
        }

        return possibleCombinations;
    }
}
