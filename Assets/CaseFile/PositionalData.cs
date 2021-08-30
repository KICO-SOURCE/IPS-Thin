#region Usings

using Assets._MUTUAL.Utils;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets.CaseFile
{
    /// <summary>
    /// Positional Data class.
    /// </summary>
    public class PositionalData
    {
        #region Public Properties

        public double TranslationX { get; set; }
        public double TranslationY { get; set; }
        public double TranslationZ { get; set; }
        public double Yaw { get; set; }
        public double Roll { get; set; }
        public double Pitch { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of positional data.
        /// </summary>
        /// <param name="positionEuler"></param>
        public PositionalData(string positionEuler)
        {
            ParseEulerAndInitPositionalData(positionEuler);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the transform from positional data.
        /// </summary>
        /// <returns>Transform</returns>
        public Transform GetTransfrom()
        {
            GameObject obj = new GameObject();
            Transform finalTransform = obj.transform;
            finalTransform.SetFromRightHandEuler((float)TranslationX, (float)TranslationY, (float)TranslationZ, (float)Yaw, (float)Roll, (float)Pitch);
            // GameObject.DestroyImmediate(obj);
            return finalTransform;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load positional data.
        /// </summary>
        private void ParseEulerAndInitPositionalData(string positionEuler)
        {
            if (!string.IsNullOrEmpty(positionEuler))
            {
                var parts = positionEuler.Split(',').Select(i => float.Parse(i)).ToArray();
                if (parts.Length == 6)
                {
                    var translationEuler = new Vector3(parts[0], parts[1], parts[2]);
                    var rotation = new Vector3(parts[3], parts[4], parts[5]);
                    TranslationX = translationEuler.x;
                    TranslationY = translationEuler.y;
                    TranslationZ = translationEuler.z;
                    Yaw = rotation.x;
                    Roll = rotation.y;
                    Pitch = rotation.z;
                }
            }
        }

        #endregion
    }

}
