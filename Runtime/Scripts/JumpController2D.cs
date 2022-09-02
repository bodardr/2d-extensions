using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Physics2D/2D Controller/Jump Controller")]
[RequireComponent(typeof(PhysicsController2D))]
public class JumpController2D : MonoBehaviour
{
    [SerializeField]
    private float jumpHeight;

    private bool canJump = false;
    private Coroutine coyoteTimeCoroutine;
    private bool isDashing = false;
    private PhysicsController2D physicsController2D;

    private void Start()
    {
        physicsController2D = GetComponent<PhysicsController2D>();
    }

    private void OnDashEnter()
    {
        isDashing = true;
    }

    private void OnDashExit()
    {
        isDashing = false;
    }

    private void OnGroundEnter()
    {
        canJump = true;

        if (coyoteTimeCoroutine != null)
        {
            StopCoroutine(coyoteTimeCoroutine);
            coyoteTimeCoroutine = null;
        }
    }

    private void OnGroundExit()
    {
        if (canJump && !isDashing)
            coyoteTimeCoroutine = StartCoroutine(CoyoteTimeCoroutine());
        else
            canJump = false;
    }

    private IEnumerator CoyoteTimeCoroutine()
    {
        yield return new WaitForSeconds(5 / 60f);
        canJump = false;
    }


    private void OnJump(InputValue value)
    {
        var vel = physicsController2D.Velocity;
        
        if (value.isPressed && canJump)
        {
            var force = MathExtensions.GetJumpForce(jumpHeight);
        
            physicsController2D.Velocity = new Vector2(vel.x, force);
            canJump = false;
            
            BroadcastMessage("OnJumpEnter", Vector2.up, SendMessageOptions.DontRequireReceiver);
        }   
        else if (!value.isPressed && vel.y > 0)
        {
            physicsController2D.Velocity = new Vector2(vel.x, vel.y / 2);
        }
    }
}