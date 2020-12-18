using System.Collections;
using UnityEngine;

public class JumpController2D : MonoBehaviour
{
    private PhysicsController2D physicsController2D;
    private Coroutine coyoteTimeCoroutine;

    private bool canJump = false;
    private bool isDashing = false;

    [SerializeField]
    private float jumpForce;

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


    private void OnJump()
    {
        if (!canJump)
            return;

        physicsController2D.Velocity = new Vector2(physicsController2D.Velocity.x, jumpForce);
        canJump = false;
        SendMessage("OnJumpEnter", Vector2.up, SendMessageOptions.DontRequireReceiver);
    }
}