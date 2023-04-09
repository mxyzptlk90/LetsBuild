public static partial class BoxPacking
{
    // This method helps do determine whether we can fit given cubes in a box without slicing them or not
    public static bool CanFitCubesGeometrically((int h, int w, int l) box, IEnumerable<int> cubes) {

        // Sort cubes to start fitting with the biggest one
        var sortedCubes = cubes.OrderByDescending(c => Math.Pow(c, 3));

        // Create and initialize a 3d array model of a box
        bool[][][] boxSpaceOccupied = new bool[box.h][][];

        for (int i = 0; i < box.h; i++) {
            boxSpaceOccupied[i] = new bool[box.w][];
            for (int j = 0; j < box.w; j++) {
                boxSpaceOccupied[i][j] = new bool[box.l];
            }
        }

        // Initialize starting position
        var possibleStartPoints = new List<(int x, int y, int z)>() { (0, 0, 0) };

        foreach (var cube in sortedCubes) {
            if (cube == 1) {
                // If all previous cubes fit into the box and only 1x1x1 cubes are left
                // It means that they fill perfectly fit too
                break;
            }
            bool isCubePlaced = false;

            for (int i = 0; i < possibleStartPoints.Count; i++) {
                var position = possibleStartPoints[i];
                if (IsValidPosition(box, cube, position.x, position.y, position.z)) {
                    // If cube fits when placed at this position, we update 2d model and remove 
                    // position from the list
                    UpdateUsedSpace(boxSpaceOccupied, cube, position.x, position.y, position.z);
                    possibleStartPoints.RemoveAt(i);

                    // Calculate next possible startPoints and add them to the list if they are not there
                    // and are not occupied by a cube
                    int xCubeEnd = position.x + cube;
                    int yCubeEnd = position.y + cube;
                    int zCubeEnd = position.z + cube;

                    if (xCubeEnd < box.h && !boxSpaceOccupied[xCubeEnd][position.y][position.z]) {
                        if (!possibleStartPoints.Contains((xCubeEnd, position.y, position.z))) {
                            possibleStartPoints.Add((xCubeEnd, position.y, position.z));
                        }
                    }

                    if (yCubeEnd < box.w && !boxSpaceOccupied[position.x][yCubeEnd][position.z]) {
                        if (!possibleStartPoints.Contains((position.x, yCubeEnd, position.z))) {
                            possibleStartPoints.Add((position.x, yCubeEnd, position.z));
                        }
                    }

                    if (zCubeEnd < box.l && !boxSpaceOccupied[position.x][position.y][zCubeEnd]) {
                        if (!possibleStartPoints.Contains((position.x, position.y, zCubeEnd))) {
                            possibleStartPoints.Add((position.x, position.y, zCubeEnd));
                        }
                    }

                    isCubePlaced = true;
                    break;
                }
            }

            // If any cube cannot be placed in the box, it means that set of cubes is incorrect
            if (!isCubePlaced) {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidPosition((int h, int w, int l) box, int cube, int x, int y, int z) => x + cube <= box.h && y + cube <= box.w && z + cube <= box.l;

    private static void UpdateUsedSpace(bool[][][] usedSpace, int cube, int x, int y, int z) {
        for (int i = 0; i < cube; i++) {
            for (int j = 0; j < cube; j++) {
                for (int k = 0; k < cube; k++) {
                    usedSpace[x + i][y + j][z + k] = true;
                }
            }
        }
    }
}
