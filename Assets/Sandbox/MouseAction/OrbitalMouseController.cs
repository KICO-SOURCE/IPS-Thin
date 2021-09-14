using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Assets.Sandbox.MouseActions
{
    public class OrbitalMouseController : MouseActionBase
    {
        public float xSpeed = 1.0f;
        public float ySpeed = 1.0f;
        public Transform target;
        public float distance = 400.0f;
        protected float x = 0.0f;
        protected float y = 0.0f;
        private bool resetPosition = false;
        private float _sensitivityFactor = 0.01f;

        protected override ButtonControl MouseButton =>
                                    Mouse.current.leftButton;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        internal void ResetPosition()
        {
            resetPosition = true;
        }
        protected override void LateUpdate()
        {
            if (resetPosition)
            {
                transform.LookAt(target, transform.up);
            }
            base.LateUpdate();
            if (!IsActive)
            {
                return;
            }
            if (MouseButton?.isPressed ?? false)
            {
                if (IsMouseInViewArea())
                {
                    if (target)
                    {
                        //  get the distance the mouse moved in the respective direction

                        var deltaX = Mouse.current.delta.x.ReadValue();
                        var deltaY = Mouse.current.delta.y.ReadValue();

                        x += deltaX * xSpeed * distance * _sensitivityFactor;
                        y -= deltaY * ySpeed * distance * _sensitivityFactor;
                        // when mouse moves left and right we actually rotate around local y axis	
                        transform.RotateAround(target.position, transform.up, x);
                        // when mouse moves up and down we actually rotate around the local x axis	
                        transform.RotateAround(target.position, transform.right, y);
                        // reset back to 0 so it doesn't continue to rotate while holding the button
                        x = 0;
                        y = 0;
                    }
                }
            }
        }
    }
}
