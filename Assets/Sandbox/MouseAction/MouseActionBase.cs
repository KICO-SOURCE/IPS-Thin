#region Usings

using System;
using UnityEngine;

#endregion

namespace Assets.Sandbox.MouseActions
{
    /// <summary>
    /// Mouse action base for mouse based zoom, pan, rotate behavior
    /// </summary>
    public class MouseActionBase : MonoBehaviour
    {
        #region Protected Members

        protected Camera ViewCamera;
        protected byte MouseButton = 0; // 0 = Left, 1 = Right, 2 = Middle, 3 = Wheel
        protected bool IsActive = false;
        protected Vector2? PrevPoint = null;
        protected DateTime LastSynchedOn { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Monobehavior start method.
        /// </summary>
        protected virtual void Start()
        {
            ViewCamera = GetComponent<Camera>();
        }

        /// <summary>
        /// Update for the UI
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (Input.GetMouseButtonDown(MouseButton))
            {
                if (IsMouseInViewArea())
                {
                    PrevPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    IsActive = true;
                    return;
                }
                IsActive = false;
            }
            if (Input.GetMouseButtonUp(MouseButton))
            {
                PrevPoint = null;
                IsActive = false;
            }
        }

        /// <summary>
        /// Check if the mouse position within the view area
        /// </summary>
        /// <returns>True if the mouse position within the view area, otherwise returns false</returns>
        protected bool IsMouseInViewArea()
        {
            var normalizedMousePos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            return ViewCamera.rect.Contains(normalizedMousePos);
        }

        #endregion
    }
}