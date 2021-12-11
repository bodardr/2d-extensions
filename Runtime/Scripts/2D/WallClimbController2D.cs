using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Physics2D/2D Controller/Wall Climb Controller")]
[RequireComponent(typeof(PhysicsController2D))]
public class WallClimbController2D : MonoBehaviour, IInputOverridable
{
    private PhysicsSolver2D physicsSolver;
    private PhysicsController2D physicsController;

    private bool climbInput;
    
    private bool isClimbing;
    private bool fromRight;

    private void Start()
    {
        physicsController = GetComponent<PhysicsController2D>();
        physicsSolver = GetComponent<PhysicsSolver2D>();
    }

    private void FixedUpdate()
    {
        if (!isClimbing)
            return;
    }

    void OnWallEnter(bool fromRight)
    {
        this.fromRight = fromRight;
    }

    private void OnClimbing(InputValue value)
    {
        climbInput = value.isPressed;
    }

    private void BeginClimbing()
    {
        physicsController.OverrideControl(this);
        physicsSolver.IsKinematic = true;
        isClimbing = false;
    }

    private void StopClimbing()
    {
        physicsController.RegainControl();
    }

    public Vector2 OverrideInputUpdate(Vector2 inputVector)
    {
        return inputVector;
    }

    public void OnControlRegained()
    {
        physicsSolver.IsKinematic = false;
        isClimbing = false;
    }
}