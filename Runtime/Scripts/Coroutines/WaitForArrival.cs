using UnityEngine;
using UnityEngine.AI;

public class WaitForArrival : CustomYieldInstruction
{
    private readonly NavMeshAgent navMeshAgent;
    private readonly float distanceTolerance;

    public override bool keepWaiting => navMeshAgent.pathPending || navMeshAgent.remainingDistance > distanceTolerance;

    public WaitForArrival(NavMeshAgent navMeshAgent, float distanceTolerance = 0.5f)
    {
        this.navMeshAgent = navMeshAgent;
        this.distanceTolerance = distanceTolerance;
    }
}