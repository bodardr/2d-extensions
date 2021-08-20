using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Physics2D/2D Controller/Dash Controller")]
[RequireComponent(typeof(PhysicsController2D))]
public class DashController2D : MonoBehaviour
{
    [SerializeField]
    private float dashForce = 5;

    [SerializeField]
    private float dashDuration = 0.35f;

    [Range(0, 2)]
    [SerializeField]
    private float dashVelocityRetention = 0.45f;

    [Header("Dash Cancelling")]
    [SerializeField]
    private float waveDashStrength = 10;

    [Range(0.5f, 5)]
    [SerializeField]
    private float waveDashInfluence;

    private bool canDash = false;
    private bool checkForDashRefill = true;

    private Coroutine dashCoroutine;
    private Vector2 dashDirection;

    private Vector2 dashPosition;
    private Tweener dashTween;
    private Vector2 lastDashPosition;

    private Vector2 moveVector;
    private PhysicsController2D physicsController;

    private bool IsDashing => dashCoroutine != null;

    private void Start()
    {
        physicsController = GetComponent<PhysicsController2D>();
    }

    private void FixedUpdate()
    {
        if (checkForDashRefill && physicsController.Grounded)
            RefillDash();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * dashForce * dashDuration);
    }

    private void RefillDash()
    {
        canDash = true;
    }

    private void OnDash()
    {
        if (canDash)
            Dash();
    }

    private void OnDashAim(InputValue value)
    {
        var vector = value.Get<Vector2>();

        if (vector.magnitude > 0.1f)
            moveVector = vector;
    }

    private void Dash()
    {
        if (dashCoroutine != null)
            StopDash();

        dashCoroutine = StartCoroutine(DashCoroutine());
    }

    private void StopDash()
    {
        if (!IsDashing)
            return;

        StopCoroutine(dashCoroutine);
        dashCoroutine = null;
        physicsController.IsKinematic = false;
        dashTween.Kill();
        BroadcastMessage("OnDashExit", SendMessageOptions.DontRequireReceiver);
    }

    private IEnumerator DashCoroutine()
    {
        BroadcastMessage("OnDashEnter", SendMessageOptions.DontRequireReceiver);

        canDash = false;
        StartCoroutine(DisableDashRefill());

        dashDirection = moveVector.normalized;
        physicsController.IsKinematic = true;

        lastDashPosition = dashPosition = physicsController.Position;
        dashTween = DOTween.To(() => dashPosition, val => dashPosition = val,
                physicsController.Position + dashDirection * (dashForce * dashDuration), dashDuration)
            .SetUpdate(UpdateType.Fixed).OnUpdate(UpdateDash);

        yield return dashTween.WaitForCompletion();

        physicsController.Velocity = dashDirection * (dashForce * dashVelocityRetention);
        physicsController.IsKinematic = false;

        dashCoroutine = null;
        BroadcastMessage("OnDashExit", SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Temporarily disables dash refill, in order to wait for physics to catch up.
    /// Fixes a double dash issue.
    /// </summary>
    private IEnumerator DisableDashRefill()
    {
        checkForDashRefill = false;
        yield return new WaitForFixedUpdate();
        checkForDashRefill = true;
    }

    private void UpdateDash()
    {
        var delta = dashPosition - lastDashPosition;
        physicsController.Position += delta;
        lastDashPosition = dashPosition;
    }

    private void OnJumpEnter(Vector2 jumpVector)
    {
        if (!IsDashing)
            return;

        WaveDash(jumpVector);
    }

    private void WaveDash(Vector2 jumpVector)
    {
        var waveDashDirection = dashDirection * waveDashInfluence + jumpVector;

        //If this is a side-jump : e.g wall jump.
        if (Mathf.Abs(jumpVector.x) > 0.05f)
            waveDashDirection.x = jumpVector.x;

        waveDashDirection.Normalize();

        StopDash();
        physicsController.Velocity = waveDashDirection * waveDashStrength;
    }
}