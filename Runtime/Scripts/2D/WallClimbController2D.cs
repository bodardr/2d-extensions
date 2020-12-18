using UnityEngine;
using UnityEngine.InputSystem;

public class WallClimbController2D : MonoBehaviour, InputOverridable
{
    private PhysicsController2D physicsController;

    private bool climbInput;
    
    private bool isClimbing;
    private bool fromRight;

    private void Start()
    {
        physicsController = GetComponent<PhysicsController2D>();
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
        physicsController.IsKinematic = true;
        isClimbing = false;
    }

    private void StopClimbing()
    {
        physicsController.RegainControl(this);
    }

    public void OnControlRegained()
    {
        physicsController.IsKinematic = false;
        isClimbing = false;
    }
}