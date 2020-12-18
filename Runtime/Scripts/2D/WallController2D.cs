using UnityEngine;
using UnityEngine.InputSystem;

public class WallController2D : MonoBehaviour
{
    private PhysicsController2D physicsController;
    private float retainedMaxDownwardsVelocity;
    
    private bool checkForWallExit;

    [SerializeField]
    private float walledMaxDownwardsVelocity = -2;

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
        physicsController.MaxDownwardsVelocity = retainedMaxDownwardsVelocity;
    }

    private void SetupWallPhysics()
    {
        retainedMaxDownwardsVelocity = physicsController.MaxDownwardsVelocity;
        physicsController.MaxDownwardsVelocity = walledMaxDownwardsVelocity;
    }
}