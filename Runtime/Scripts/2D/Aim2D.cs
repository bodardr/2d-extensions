using UnityEngine;
using UnityEngine.InputSystem;

namespace Bodardr.Utility.Runtime
{
    public class Aim2D : MonoBehaviour
    {
        private PlayerInput playerInput;
        
        [SerializeField]
        private Vector2 aimOffset;

        public bool IsGamepad { get; private set; }
        public float Angle { get; private set; }
        public Vector2 Delta { get; private set; }
        public Vector3 MouseWorldPos { get; private set; }

        public Vector2 AimOffset => aimOffset;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            switch (playerInput.currentControlScheme)
            {
                case "Gamepad":
                {
                    IsGamepad = true;
                    
                    var inputRightStick = Gamepad.current.rightStick.ReadValue();
                    var inputLeftStick = Gamepad.current.leftStick.ReadValue();
                    
                    Delta = inputRightStick.magnitude > 0.2f ? inputRightStick : inputLeftStick;
                    break;
                }
                case "Keyboard&Mouse":
                {
                    IsGamepad = false;

                    var mainCam = Camera.main ? Camera.main : Camera.current;
                    
                    var mousePosVal = Mouse.current.position.ReadValue();
                    var playerPos = mainCam.WorldToViewportPoint(transform.position + (Vector3)AimOffset);
                    var mousePos = mainCam.ScreenToViewportPoint(mousePosVal);

                    MouseWorldPos = mainCam.ScreenToWorldPoint(mousePosVal);
                    Delta = mousePos - playerPos;
                    break;
                }
            }

            Angle = Mathf.Atan2(Delta.y, Delta.x) * Mathf.Rad2Deg;
        }
    }
}