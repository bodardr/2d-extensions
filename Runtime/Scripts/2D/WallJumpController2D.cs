using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Physics2D/2D Controller/Wall Jump Controller")]
[RequireComponent(typeof(PhysicsController2D))]
public class WallJumpController2D : MonoBehaviour, IInputOverridable
{
    private PhysicsController2D physicsController;
    
    private Vector2 moveVector;
    private Vector2 jumpVector;

    private bool grounded;
    private bool canJump = false;
    private bool fromRight = false;

    Coroutine coyoteTimeCoroutine;

    [SerializeField]
    private float jumpForce = 5;

    [Header("Jump angle range from 0-90°. 0° is right")]
    [Tooltip("Ranges from 0 to 90°. 0° is right")]
    [SerializeField]
    private Vector2 jumpAngleRange;

    [Header("Coyote Time")]
    private float coyoteTime = 0.5f;

    [Header("Override input")]
    [SerializeField]
    private bool overrideInput = false;

    [SerializeField]
    private float inputOverrideDuration = 0.2f;

    private void Start()
    {
        physicsController = GetComponent<PhysicsController2D>();
    }

    private void OnGroundEnter()
    {
        grounded = true;
    }

    private void OnGroundExit()
    {
        grounded = false;
    }

    private void OnWallEnter(bool fromRight)
    {
        canJump = true;
        this.fromRight = fromRight;
    }

    private void OnWallExit()
    {
        if (canJump)
            coyoteTimeCoroutine = StartCoroutine(CoyoteTimeCoroutine());
    }

    private void OnJump(InputValue value)
    {
        if (!canJump || grounded || !value.isPressed)
            return;

        var angle = FindJumpAngle();

        jumpVector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        physicsController.Velocity = jumpVector * MathExtensions.GetJumpForce(jumpForce);

        SendMessage("OnJumpEnter", jumpVector, SendMessageOptions.DontRequireReceiver);

        if (overrideInput)
            StartCoroutine(OverrideInputCoroutine());

        canJump = false;
    }

    private IEnumerator CoyoteTimeCoroutine()
    {
        yield return new WaitForSeconds(coyoteTime);
        canJump = false;
    }

    private IEnumerator OverrideInputCoroutine()
    {
        physicsController.OverrideControl(this);
        yield return new WaitForSeconds(inputOverrideDuration);
        physicsController.RegainControl();
    }

    private void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
    }

    private float FindJumpAngle()
    {
        var range = fromRight ? new Vector2(180 - jumpAngleRange.y, 180 - jumpAngleRange.x) : jumpAngleRange;

        float jumpAngle;
        
        if (moveVector.magnitude < 0.1f)
        {
            jumpAngle = (range.x + range.y) / 2;
        }
        else
        {
            var angle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;
            jumpAngle = Mathf.Clamp(angle, range.x, range.y);
        }

        return jumpAngle;
    }
    
    public Vector2 OverrideInputUpdate(Vector2 inputVector)
    {
        return jumpVector;
    }

    public void OnControlRegained()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position,
            transform.position + Quaternion.Euler(0, 0, fromRight ? 180 - jumpAngleRange.x : jumpAngleRange.x) *
            Vector3.right);
        Gizmos.DrawLine(transform.position,
            transform.position + Quaternion.Euler(0, 0, fromRight ? 180 - jumpAngleRange.y : jumpAngleRange.y) *
            Vector3.right);

        if (canJump)
        {
            Gizmos.color = Color.red;
            var angle = FindJumpAngle();
            Gizmos.DrawLine(transform.position,
                transform.position +
                (Vector3) new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)));
        }
    }
}