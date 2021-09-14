using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Assets.Sandbox.MouseActions
{
    public class OrbitalMousePanHelper : MouseActionBase
    {
        public Transform pivotTarget;

        protected override ButtonControl MouseButton =>
                                    Mouse.current.rightButton;

        protected override void Start()
        {
            base.Start();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (!IsActive)
            {
                return;
            }
            if (MouseButton.isPressed)
            {
                if (IsMouseInViewArea())
                {
                    var newPoint = Mouse.current.position.ReadValue();
                    var movement = ViewCamera.transform.TransformVector(PrevPoint.Value - newPoint);
                    movement *= (ViewCamera.orthographicSize * 2 / (UnityEngine.Screen.height * 0.8f));
                    ViewCamera.transform.position += movement;
                    PrevPoint = newPoint;
                }
            }
        }
    }
}