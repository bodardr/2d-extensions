using UnityEngine;

public class WallController2D : MonoBehaviour
{
    [SerializeField]
    private float walledMaxDownwardsVelocity = -2;

    private bool checkForWallExit;
    private PhysicsController2D physicsController;
    private Vector2 retainedYRange;

    private void Start()
    {
        physicsController = GetComponent<PhysicsController2D>();
    }

    private void OnWallEnter(bool fromRight)
    {
        SetupWallPhysics();
    }

    private void OnWallExit()
    {
        physicsController.YVelocityRange = retainedYRange;
    }

    private void SetupWallPhysics()
    {
        retainedYRange = physicsController.YVelocityRange;
        physicsController.YVelocityRange = new Vector2(walledMaxDownwardsVelocity, retainedYRange.y);
    }
}