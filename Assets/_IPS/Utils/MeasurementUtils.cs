using Assets.CaseFile;
using System.Linq;
using UnityEngine;

namespace Ips.Utils
{
    class MeasurementUtils
    {
        public static void GetFemurAxes(Patient patient, out Vector3 siAxis, out Vector3 mlAxis, out Vector3 apAxis)
        {
            // Construct the femur bone anatomical axes
            // SI - FemurCenter to HipCenter (points towards superior direction)
            // ML - MedialSulcus to LateralEpicondyle (projected axes) (points towards lateral direction)
            // AP - Perpendicular to SI and ML axis (points towards posterior direction)
            siAxis = Vector3.zero;
            mlAxis = Vector3.zero;
            apAxis = Vector3.zero;

            var femoralCenter = patient.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter");
            var hipCenter = patient.Landmarks.FirstOrDefault(lm => lm.Type == "hipCenter");
            siAxis = hipCenter.Position - femoralCenter.Position;

            var lateralEpicondyle = patient.Landmarks.FirstOrDefault(lm => lm.Type == "lateralEpicondyle");
            var medialSulcus = patient.Landmarks.FirstOrDefault(lm => lm.Type == "medialSulcus");

            // inital projection on axial plane
            var boneLateralEpicondylePos = Vector3.ProjectOnPlane(lateralEpicondyle.Position, siAxis);
            var bonemedialSulcusPos = Vector3.ProjectOnPlane(medialSulcus.Position, siAxis);

            mlAxis = boneLateralEpicondylePos - bonemedialSulcusPos;
            // fix the direction of axis based on side
            apAxis = patient.Leftright.ToLower() == "left" ?
                Vector3.Cross(mlAxis, siAxis) : Vector3.Cross(siAxis, mlAxis);

            boneLateralEpicondylePos = Vector3.ProjectOnPlane(boneLateralEpicondylePos, apAxis);
            bonemedialSulcusPos = Vector3.ProjectOnPlane(bonemedialSulcusPos, apAxis);
            // Correct the projection after AP axis calculation.
            mlAxis = boneLateralEpicondylePos - bonemedialSulcusPos;

            siAxis.Normalize();
            mlAxis.Normalize();
            apAxis.Normalize();
        }

        public static void GetPatellaAxes(Patient patient, out Vector3 siAxis, out Vector3 mlAxis, out Vector3 apAxis)
        {
            siAxis = Vector3.zero;
            mlAxis = Vector3.zero;
            apAxis = Vector3.zero;

            var patTendonOrigin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "patTendonOrigin");
            var quadTendonInsertion = patient.Landmarks.FirstOrDefault(lm => lm.Type == "quadTendonInsertion");
            siAxis = quadTendonInsertion.Position - patTendonOrigin.Position;

            var patLateralEdge = patient.Landmarks.FirstOrDefault(lm => lm.Type == "LateralEdge");
            var patMedialEdge = patient.Landmarks.FirstOrDefault(lm => lm.Type == "MedialEdge");

            // inital projection on axial plane
            var bonePatLateralEdgePos = Vector3.ProjectOnPlane(patLateralEdge.Position, siAxis);
            var bonePatMedialEdgePos = Vector3.ProjectOnPlane(patMedialEdge.Position, siAxis);

            mlAxis = bonePatLateralEdgePos - bonePatMedialEdgePos;
            // fix the direction of axis based on side
            apAxis = patient.Leftright.ToLower() == "left" ?
                Vector3.Cross(mlAxis, siAxis) : Vector3.Cross(siAxis, mlAxis);

            siAxis.Normalize();
            mlAxis.Normalize();
            apAxis.Normalize();
        }

        public static void GetTibiaAxes(Patient patient, out Vector3 siAxis, out Vector3 mlAxis, out Vector3 apAxis)
        {
            siAxis = Vector3.zero;
            mlAxis = Vector3.zero;
            apAxis = Vector3.zero;

            var tubercle = patient.Landmarks.FirstOrDefault(lm => lm.Type == "Tubercle");
            var pclInsertion = patient.Landmarks.FirstOrDefault(lm => lm.Type == "PCLInsertion");
            Vector3 tibCenter = (tubercle.Position + pclInsertion.Position) * 0.5f;

            var medMal = patient.Landmarks.FirstOrDefault(lm => lm.Type == "medialMalleolus");
            var latMal = patient.Landmarks.FirstOrDefault(lm => lm.Type == "lateralMalleolus");
            Vector3 ankleCenter = (medMal.Position + latMal.Position) * 0.5f;

            siAxis = tibCenter - ankleCenter;

            var tibTubercle = Vector3.ProjectOnPlane(tubercle.Position, siAxis);
            var tibPclInsertion = Vector3.ProjectOnPlane(pclInsertion.Position, siAxis);

            // Consider the reference AP axis for the ML calculation.
            apAxis = tibPclInsertion - tibTubercle;
            mlAxis = patient.Leftright.ToLower() == "left" ?
                Vector3.Cross(siAxis, apAxis) : Vector3.Cross(apAxis, siAxis);

            siAxis.Normalize();
            mlAxis.Normalize();
            apAxis.Normalize();
        }
    }
}
