using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    public Transform targetCheckpoint;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void GoToCheckpoint()
    {
        if (agent == null)
        {
            Debug.LogWarning("No NavMeshAgent found on monster.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Monster is not on NavMesh.");
            return;
        }

        if (targetCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint assigned.");
            return;
        }

        agent.SetDestination(targetCheckpoint.position);
    }
}