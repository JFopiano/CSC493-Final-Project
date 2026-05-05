using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script represents a cell in the maze, controlling the visibility of its walls based on the provided parameters.
public class MazeCellObject : MonoBehaviour
{   
    // References to the wall GameObjects for the top, bottom, left, and right walls of the maze cell, set in the Unity Inspector.
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;

    // Method to initialize the maze cell by setting the active state of its walls based on the provided parameters.
    public void Init (bool top, bool bottom, bool left, bool right)
    {
        topWall.SetActive(top);
        bottomWall.SetActive(bottom);
        leftWall.SetActive(left);
        rightWall.SetActive(right);
    }
}
