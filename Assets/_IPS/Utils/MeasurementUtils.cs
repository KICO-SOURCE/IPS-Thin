using Assets.CaseFile;
using UnityEngine;

namespace Ips.Utils
{
    class MeasurementUtils
    {
        public static void GetFemurAxes(Patient patient, out Vector3 siAxis, out Vector3 mlAxis, out Vector3 apAxis)
        {
            siAxis = Vector3.forward;
            mlAxis = Vector3.left;
            apAxis = Vector3.down;
        }

        public static void GetPatellaAxes(Patient patient, out Vector3 siAxis, out Vector3 mlAxis, out Vector3 apAxis)
        {
            siAxis = Vector3.forward;
            mlAxis = Vector3.left;
            apAxis = Vector3.down;
        }

        public static void GetTibiaAxes(Patient patient, out Vector3 siAxis, out Vector3 mlAxis, out Vector3 apAxis)
        {
            siAxis = Vector3.forward;
            mlAxis = Vector3.right;
            apAxis = Vector3.up;
        }
    }
}
