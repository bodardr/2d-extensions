using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TopDownMovement : MonoBehaviour
{
    private CharacterController characterController;

    private Vector2 moveVector;
    private Vector2 lookVector;

    private Vector2 moveVelocity;

    private bool canMove = true;
    private bool isDashing = false;

    private PlayerInput playerInput;
    private InputActionMap playerActionMap;

    [SerializeField]
    private float acceleration = 4f;

    [SerializeField]
    private float friction = 3;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private float dashSpeed = 10;

    [SerializeField]
    private float dashDuration = 0.3f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        playerActionMap = playerInput.actions.FindActionMap("Player");
    }

    private void Update()
    {
        if (!canMove)
            return;

        Move();
        Rotate();
    }

    private void FixedUpdate()
    {
        ApplyFriction();

        if (!canMove)
            return;

        ApplyMovement();
    }

    private void Move()
    {
        characterController.SimpleMove(new Vector3(moveVelocity.x, 0, moveVelocity.y));
    }

    private void ApplyMovement()
    {
        moveVelocity += moveVector * (acceleration * Time.fixedDeltaTime);
    }

    private void ApplyFriction()
    {
        moveVelocity -= moveVelocity * (friction * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        float yRotation;
        if (lookVector.magnitude > 0.125f)
            yRotation = -Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg + 90;
        else if (moveVector.magnitude > 0.125f)
            yRotation = -Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg + 90;
        else
            yRotation = transform.rotation.eulerAngles.y;

        characterController.transform.rotation = Quaternion.Slerp(characterController.transform.rotation,
            Quaternion.Euler(0, yRotation, 0),
            Time.fixedDeltaTime * rotationSpeed);
    }

    private void OnMove(InputValue val)
    {
        moveVector = val.Get<Vector2>();
    }

    private void OnLook(InputValue val)
    {
        lookVector = val.Get<Vector2>();
    }

    private void OnDash()
    {
        if (isDashing)
            return;

        StartCoroutine(DashCoroutine());
    }

    public void EnablePlayerInput()
    {
        playerActionMap.Enable();
    }

    public void DisablePlayerInput()
    {
        playerActionMap.Disable();
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;

        var move = new Vector3(moveVector.x, 0, moveVector.y);
        var velocity = Vector3.zero;
        yield return DOTween.To(() => velocity, val =>
        {
            velocity = val;
            characterController.Move(velocity);
        }, Vector3.zero, dashDuration).From(move.normalized * dashSpeed).SetEase(Ease.OutSine);

        isDashing = false;
    }
}