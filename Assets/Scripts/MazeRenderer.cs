//using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject MazeCellPrefab;

    // Monster Path, Spawnpoint and the Monster Itself
    public NavMeshSurface navMeshSurface;
    public GameObject monsterPrefab;
    public Transform monsterSpawnPoint;
    public Transform targetCheckpoint;


    // This the physical size of our maze cells. Getting this wrong will result in overlapping
    // or visible gaps between each cell. 
    public float cellSize = 1f;

    public float navMeshSearchRadius = 3f;

    private void Start()
    {
        // Get our MazeGenerator script to make us a maze.
        MazeCell[,] maze = mazeGenerator.GetMaze();

        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                // Instantiate a new maze cell prefab as a child of the MazeRenderer object.
                GameObject newCell = Instantiate(MazeCellPrefab, new UnityEngine.Vector3((float)x * cellSize, 0f, (float)y * cellSize), UnityEngine.Quaternion.identity, transform);

                // Get a reference to the cell's MazeCellPrefab script.
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                // Determine which walls need to be active.
                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;

                // Bottom and right walls are deactivated by default unless we are at the bottom or right
                // edge of the maze.
                bool right = false;
                bool bottom = false;
                if (x == mazeGenerator.mazeWidth - 1)
                {
                    right = true;
                }
                if (y == 0)
                {
                    bottom = true;
                }

                mazeCell.Init(top, bottom, left, right);
            }
        }


        // Inities the Navmesh after the Maze is created
        navMeshSurface.BuildNavMesh();

        Vector3 desiredSpawnPosition = new Vector3( (mazeGenerator.mazeWidth - 1) * cellSize, 0f, (mazeGenerator.mazeHeight - 1) * cellSize);

        monsterSpawnPoint.position = desiredSpawnPosition;

        
        // 4. Find the nearest valid NavMesh position
        if (NavMesh.SamplePosition(desiredSpawnPosition, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            // 5. Move the monster safely onto the NavMesh
            NavMeshAgent agent = monsterPrefab.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                agent.Warp(hit.position);
            }
            else
            {
                monsterPrefab.transform.position = hit.position;
            }

            monsterPrefab.transform.rotation = monsterSpawnPoint.rotation;

            // 6. Tell the monster where to go
            MonsterMovement movement = monsterPrefab.GetComponent<MonsterMovement>();

            if (movement != null)
            {
                movement.targetCheckpoint = targetCheckpoint;
                movement.GoToCheckpoint();
            }
            else
            {
                Debug.LogWarning("MonsterMovement script is missing from the monster.");
            }
            }
            else
            {
            Debug.LogWarning("Could not find a valid NavMesh position near the monster spawn point.");
        }
    }
}
