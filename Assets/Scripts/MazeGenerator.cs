using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;       // The dimensions of the maze to generate.
    public int startX, startY;                   // The position our algorithm will start from.
    MazeCell[,] maze;

    Vector2Int currentCell;                          // The cell the algorithm is currently on.

    public MazeCell[,] GetMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        CarvePath(startX, startY);
        return maze;
    }

    List<Direction> directions = new List<Direction>
    {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right
    };

    List<Direction> GetRandomDirections()
    {
        // Make a copy of our directions list that we can mess around with.
        List<Direction> dir = new List<Direction>(directions);

        // Make a directions list to put our randomized directions in.
        List<Direction> randomDir = new List<Direction>();

        while (dir.Count > 0)
        {
            // Get a random index from the directions list.
            int index = Random.Range(0, dir.Count);

            // Add the direction at that index to our random directions list.
            randomDir.Add(dir[index]);

            // Remove that direction from the original list so we don't get it again.
            dir.RemoveAt(index);
        }

        // When we have all four directions in a random order, return the queue.
        return randomDir;
    }

    bool IsCellValid (int x, int y)
    {
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || maze[x, y].visited)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    Vector2Int CheckNeighbors ()
    {
        List<Direction> randomDir = GetRandomDirections();

        for (int i = 0; i < randomDir.Count; i++)
        {
            // Set neighbor coordinates to current cell for now.
            Vector2Int neighbor = currentCell;

            switch (randomDir[i])
            {
                case Direction.Up:
                    neighbor.y++;
                    break;
                case Direction.Down:
                    neighbor.y--;
                    break;
                case Direction.Left:
                    neighbor.x--;
                    break;
                case Direction.Right:
                    neighbor.x++;
                    break;
            }

            // If the neighbor we're checking is valid, return it.
            if (IsCellValid(neighbor.x, neighbor.y))
            {
                return neighbor;
            }
        }

        // If we tried all directions and didn't find a valid neighbor, we return the currentCell values.
        return currentCell;

    }

    // Takes in two maze positions and sets the cells accordingly
    void BreakWalls (Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        // We can only go in one direction at a time so we can handle this using if else statements.
        if (primaryCell.x > secondaryCell.x)    // Primary Cell's Left Wall
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
        }
        else if (primaryCell.x < secondaryCell.x)   // Secondary Cell's Left Wall
        {
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        else if (primaryCell.y < secondaryCell.y)   // Primary Cell's Top Wall
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
        }
        else if (primaryCell.y > secondaryCell.y)   // Secondary Cell's Top Wall
        {
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }

    // Starting at the x, y passed in, carves a path through the maze until it encounters a "dead end"
    // (a dead end is a cell with no valid neighbours).
    void CarvePath (int x, int y)
    {
        // Perform a check to make sure our start position is within the bounds of the maze,
        // If not, set them to a default (I'm using 0) and throw in a little warning up.
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            Debug.LogWarning("Start position is out of bounds! Defaulting to (0, 0).");
            x = y = 0;
        }

        // Set the curretnt cell to the starting position we were passed.
        currentCell = new Vector2Int(x, y);

        // A list to keep track of our current path.
        List<Vector2Int> path = new List<Vector2Int>();

        // Loop until we hit a dead end.
        bool deadend = false;
        while (!deadend)
        {
            // Get the next cell to move to.
            Vector2Int nextCell = CheckNeighbors();

            // If the cell has no valid neighbors, set deadend to true so we can exit the loop.
            if (nextCell == currentCell)
            {
                // If that cell has no valid neighbors, set deadend to true so we can break out of the loop.
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    currentCell = path[i];
                    path.RemoveAt(i);
                    nextCell = CheckNeighbors();

                    // If we find a valid neighbor, break out of the loop.
                    if (nextCell != currentCell)
                    {
                        break;
                    }
                }

                if (nextCell == currentCell)
                {
                    deadend = true;
                }
            }
            else
            {
                BreakWalls(currentCell, nextCell);    // Set wall flags on these two cells.
                maze[currentCell.x, currentCell.y].visited = true;    // Set cell to visited before moving on.
                currentCell = nextCell;    // Set the current cell to the valid neighbor we found.
                path.Add(currentCell);    // Add this cell to our path.

            }
        }
    }

}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    public bool visited;
    public int x, y;

    public bool topWall;
    public bool leftWall;

    // Return x and y as a Vector2Int for convenience sake.
    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell (int x, int y)
    {
        // The coordinates of this cell in the maze grid.
        this.x = x;
        this.y = y;

        // Whether the algorithm has visited this cell or not - false to start
        visited = false;

        // All walls are present until the algorithm removes them.
        topWall = leftWall = true;
    }
}