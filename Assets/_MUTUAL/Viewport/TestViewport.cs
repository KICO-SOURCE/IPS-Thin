#region Usings

using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A dummy viewport class for testing.
    /// </summary>
    public class TestViewport : Viewport
    {
        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public TestViewport()
        {
            Views.Add(new TestView() { Postion = new Vector2(0, 0), Size = new Vector2(100,100) });
        }

        #endregion
    }
}