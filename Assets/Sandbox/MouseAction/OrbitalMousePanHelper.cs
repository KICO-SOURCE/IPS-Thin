using UnityEngine;

namespace Assets.Sandbox.MouseActions
{
    public class OrbitalMousePanHelper : MouseActionBase
    {
        public Transform pivotTarget;
        protected override void Start()
        {
            base.Start();
            MouseButton = 1;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (!IsActive)
            {
                return;
            }
            if (UnityEngine.Input.GetMouseButton(MouseButton))
            {
                if (IsMouseInViewArea())
                {
                    var newPoint = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
                    var movement = ViewCamera.transform.TransformVector(PrevPoint.Value - newPoint);
                    movement *= (ViewCamera.orthographicSize * 2 / (UnityEngine.Screen.height * 0.8f));
                    ViewCamera.transform.position += movement;
                    PrevPoint = newPoint;
                }
            }
        }
    }
}