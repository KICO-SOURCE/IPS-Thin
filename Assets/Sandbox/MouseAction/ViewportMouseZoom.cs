#region Usings

using UnityEngine;

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

        #region Protected Methods

        /// <summary>
        /// Monobehavior start method.
        /// </summary>
        protected override void Start()
        {
            MouseButton = 1;
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
            if (Input.GetMouseButton(MouseButton))
            {
                if (IsMouseInViewArea())
                {
                    offsetY = Input.mousePosition.y;
                    offsetY = (PrevPoint.Value.y - Input.mousePosition.y) * (ViewCamera.orthographicSize / 100);
                    var newSize = ViewCamera.orthographicSize + offsetY;
                    if ((newSize < maxLimit) && (newSize > minLimit))
                    {
                        ViewCamera.orthographicSize = newSize;
                    }
                    PrevPoint = Input.mousePosition;
                    return;
                }
            }
        }

        #endregion
    }
}