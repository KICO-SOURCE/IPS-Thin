#region Usings

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#endregion

namespace Assets.Sandbox.MouseActions
{
    /// <summary>
    /// Handles the zoom mouse action.
    /// </summary>
    public class ViewportMouseZoom : MouseActionBase
    {
        #region Public Members

        public float minLimit = 20f;
        public float maxLimit = 3000f;
        float offsetY = 0f;

        #endregion

        protected override ButtonControl MouseButton =>
                                    Mouse.current.rightButton;

        #region Protected Methods

        /// <summary>
        /// Monobehavior start method.
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update for the UI
        /// </summary>
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
                    var mousePosition = Mouse.current.position.ReadValue();
                    offsetY = mousePosition.y;
                    offsetY = (PrevPoint.Value.y - mousePosition.y) * (ViewCamera.orthographicSize / 100);
                    var newSize = ViewCamera.orthographicSize + offsetY;
                    if ((newSize < maxLimit) && (newSize > minLimit))
                    {
                        ViewCamera.orthographicSize = newSize;
                    }
                    PrevPoint = mousePosition;
                    return;
                }
            }
        }

        #endregion
    }
}