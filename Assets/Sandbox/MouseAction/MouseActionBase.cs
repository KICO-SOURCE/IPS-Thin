#region Usings

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
        protected bool IsActive = false;
        protected Vector2? PrevPoint = null;
        protected DateTime LastSynchedOn { get; set; }
        protected virtual ButtonControl MouseButton { get; }

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
            if (MouseButton.wasPressedThisFrame)
            {
                if (IsMouseInViewArea())
                {
                    var mousePosition = Mouse.current.position.ReadValue();
                    PrevPoint = new Vector2(mousePosition.x, mousePosition.y);
                    IsActive = true;
                    return;
                }
                IsActive = false;
            }
            if (MouseButton.wasReleasedThisFrame)
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
            var mousePosition = Mouse.current.position.ReadValue();
            var normalizedMousePos = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            return ViewCamera.rect.Contains(normalizedMousePos);
        }

        #endregion
    }
}