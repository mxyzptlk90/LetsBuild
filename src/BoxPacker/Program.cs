namespace BoxPacker
{
    public class Program
    {
        private const int _approximateFunctionSize = 300;

        static void Main(string[] args) {

            var input = Console.In.ReadToEnd();
            if (string.IsNullOrEmpty(input)) {
                return;
            }

            // If -i option is passed then program uses iterative method instead of recursive
            var useRecursion = !args.Contains("-i");

            var problemLines = input.Split(Environment.NewLine);

            foreach (var problem in problemLines) {
                try {
                    Console.Out.WriteLine(CalculateMinSetOfCubesForProblem(problem, useRecursion));
                }
                catch {
                    // Task specifies that program can write into output only result or -1 if problem has no solution. 
                    // Assume that an exception means no solution
                    Console.Out.WriteLine(-1);
                }
            }
        }

        private static int CalculateMinSetOfCubesForProblem(string problem, bool useRecursion = true) {
            var result = -1;

            // Parsing problem string into box and list of available cubes
            var data = ParseProblem(problem);
            if (!data.HasValue) {
                return result;
            }

            var (box, cubes) = data.Value;

            return CountMinCubesInBox(box, cubes, useRecursion);
        }

        private static ((int, int, int), IndexedStack<int>)? ParseProblem(string problem) {
            var problemArgs = problem.Split(' ');

            if (problemArgs.Length < 3) {
                return null;
            }
            // Create box dimentions from first three args in problem
            if (!int.TryParse(problemArgs[0], out int h) || !int.TryParse(problemArgs[1], out int w) || !int.TryParse(problemArgs[2], out int l)) {
                return null;
            }

            var box = (h, w, l);

            // Create stack of cubes, so the biggest cube will be on top
            var cubes = new Stack<(int, int)>();
            for (var i = 3; i < problemArgs.Length; i++) {
                if (problemArgs[i] != "0") {
                    (int edge, int count) cube = ((int)Math.Pow(2, i - 3), int.Parse(problemArgs[i]));
                    cubes.Push(cube);
                }
            }

            // This class helps to index through cubes
            var indexedCubes = new IndexedStack<int>(cubes);

            return (box, indexedCubes);
        }

        private static int CountMinCubesInBox((int, int, int) box, IndexedStack<int> cubes, bool useRecursion = true) {
            var result = -1;
            var cubesCount = cubes.Count;

            Func<(int, int, int), IndexedStack<int>, IEnumerable<IEnumerable<int>>> getCubesThatCanFit = useRecursion ? BoxPacking.GetCubesThatCanFillBoxRecursivly
                                                                                                               : BoxPacking.GetCubesThatCanFillBoxIteratively;

            ThreadStart action = () => {
                var solutions = getCubesThatCanFit(box, cubes)
                    .Where(c => BoxPacking.CanFitCubesGeometrically(box, c))
                    .Select(s => s.Count());
                result = solutions.Count() > 0 ? solutions.Min() : -1;
            };

            // Well, I used backtracking approach with recursion first, so there is a good chance of StackOverflow
            // I've checked in ildasm and calculated that function takes approximately 300 bytes of stack memory per recursion
            // If it will take more that 3/4 of stack memory (for x64), program will execute method in separate thread with enormous stack
            if (useRecursion && cubesCount * _approximateFunctionSize > 3 * 1024 * 1024) {
                var t = new Thread(action, cubesCount * _approximateFunctionSize);
                t.Start();
                t.Join();
            } else {
                action();
            }

            return result;
        }

        
    }
}