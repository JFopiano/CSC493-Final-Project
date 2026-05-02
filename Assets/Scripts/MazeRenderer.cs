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
    [SerializeField] GameObject exitDoorPrefab;
    [SerializeField] GameObject doorFramePrefab;

    // Monster Path, Spawnpoint and the Monster Itself
    public NavMeshSurface navMeshSurface;
    public GameObject monsterPrefab;
    public Transform monsterSpawnPoint;
    //public Transform targetCheckpoint;

    // This the physical size of our maze cells. Getting this wrong will result in overlapping
    // or visible gaps between each cell. 
    public float cellSize = 1f;


    // This is the radius used when searching for a valid NavMesh position near the monster spawn point.
    public float navMeshSearchRadius = 3f;

    private void Start()
    {
        // Get our MazeGenerator script to make us a maze.
        MazeCell[,] maze = mazeGenerator.GetMaze();

        // Choose exit cell (top-right corner)
        int exitX = mazeGenerator.mazeWidth - 1;
        int exitY = mazeGenerator.mazeHeight - 1;

        // Remove ONLY the top wall so there is an opening
        maze[exitX, exitY].topWall = false;

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

                // IMPORTANT: no extra modifications here — only the top wall is removed above

                mazeCell.Init(top, bottom, left, right);
            }
        }

        SpawnExitDoor(exitX, exitY);

        // Inities the Navmesh after the Maze is created, if we do it before, 
        // the navmesh will be empty and the monster will not move
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        //Calculate the desired spawn position for the monster (opposite corner of the player)
        Vector3 desiredSpawnPosition = new Vector3((mazeGenerator.mazeWidth - 1) * cellSize, 0f, (mazeGenerator.mazeHeight - 1) * cellSize);


        //Move the monster prefab to the desired spawn position (this may be off the NavMesh)
        if (monsterSpawnPoint != null)
        {
            monsterSpawnPoint.position = desiredSpawnPosition;
        }

        

        //Find the nearest valid NavMesh position
        if (NavMesh.SamplePosition(desiredSpawnPosition, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {

            GameObject monster = Instantiate(monsterPrefab, desiredSpawnPosition, Quaternion.identity);


            //Move the monster safely onto the NavMesh
            NavMeshAgent agent = monster.GetComponent<NavMeshAgent>();

            // If the monster has a NavMeshAgent, use Warp to move it instantly. Otherwise, just set the position directly.
            if (agent != null)
            {
                agent.Warp(hit.position);
            }
            else if (monster != null)
            {
                monster.transform.position = hit.position;
            }

            // Set the monster's rotation to match the spawn point's rotation (if both are assigned)
            if (monster != null && monsterSpawnPoint != null)
            {
                monster.transform.rotation = monsterSpawnPoint.rotation;
            }

            // Tell the monster where to go
            MonsterMovement movement = monster.GetComponent<MonsterMovement>();

            // Get the corner checkpoints for the monster to patrol between
            if (movement != null)
            {
                Vector3[] cornerCheckpoints = GetCornerCheckpoints();
                movement.SetCheckpoints(cornerCheckpoints);
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

    void SpawnExitDoor(int exitX, int exitY)
    {
        // Door position (slightly outside the maze)
        Vector3 doorPosition = new Vector3(
            exitX * cellSize,
            0.45f,
            exitY * cellSize + cellSize / 2f
        );

        Quaternion doorRotation = Quaternion.identity;

        if (exitDoorPrefab != null)
        {
            Instantiate(exitDoorPrefab, doorPosition, doorRotation, transform);
        }

        if (doorFramePrefab != null)
        {
            Vector3 framePosition = new Vector3(
                exitX * cellSize,
                0f,
                exitY * cellSize + cellSize / 2f
            );

            Instantiate(doorFramePrefab, framePosition, doorRotation, transform);
        }
    }

    // This function returns the world positions of the four corners of the maze, 
    // which can be used as checkpoints for the monster to patrol between.
    private Vector3[] GetCornerCheckpoints()
    {
        float maxX = (mazeGenerator.mazeWidth - 1) * cellSize;
        float maxZ = (mazeGenerator.mazeHeight - 1) * cellSize;

        Vector3[] rawCorners =
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(maxX, 0f, 0f),
            new Vector3(0f, 0f, maxZ),
            new Vector3(maxX, 0f, maxZ)
        };

        Vector3[] validCorners = new Vector3[rawCorners.Length];

        for (int i = 0; i < rawCorners.Length; i++)
        {
            if (NavMesh.SamplePosition(rawCorners[i], out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
            {
                validCorners[i] = hit.position;
            }
            else
            {
                validCorners[i] = rawCorners[i];
                Debug.LogWarning("Could not find NavMesh near checkpoint corner " + i);
            }
        }

        return validCorners;
    }
}