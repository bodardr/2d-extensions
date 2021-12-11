using UnityEngine;

[AddComponentMenu("Physics2D/2D Controller/Wall Controller")]
[RequireComponent(typeof(PhysicsController2D))]
public class WallController2D : MonoBehaviour
{
    private bool checkForWallExit;
    private PhysicsController2D physicsController;
    private float retainedDrag;

    [Range(0,1)]
    [SerializeField]
    private float wallDrag = .95f;

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
        physicsController.Drag = retainedDrag;
    }

    private void SetupWallPhysics()
    {
        retainedDrag = physicsController.Drag;
        physicsController.Drag = wallDrag;
    }
}