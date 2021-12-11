using DG.Tweening;
using UnityEngine;

public class WaitFor2DControllerArrival : CustomYieldInstruction, IInputOverridable
{
    private const double DISTANCE_TOLERANCE = 0.25f;
    
    private readonly PhysicsController2D physicsController;
    private readonly Vector2 targetPosition;
    private readonly bool initialKinematic;
    private readonly Tween tween;

    private bool isFinished = false;
    private readonly Vector2 delta;


    public override bool keepWaiting => !isFinished;

    public WaitFor2DControllerArrival(PhysicsController2D physicsController, Vector2 targetPosition)
    {
        this.physicsController = physicsController;
        this.targetPosition = targetPosition;

        delta = (targetPosition - physicsController.PhysicsSolver.Position).normalized;
        
        this.physicsController.OverrideControl(this);
    }

    public void OnControlRegained()
    {
        //Nothing to do.
    }

    public Vector2 OverrideInputUpdate(Vector2 inputVector)
    {
        if (!(Mathf.Abs(targetPosition.x - physicsController.PhysicsSolver.Position.x) < DISTANCE_TOLERANCE))
            return delta;
        
        isFinished = true;
        physicsController.RegainControl();
        return Vector2.zero;

    }
}