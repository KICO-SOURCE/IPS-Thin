using Assets.CaseFile;
using Assets.Utils;
using System;
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

        /// <summary>
        /// Calculate Anatomic to Mechanical angle measure.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        internal static double MeasureAnatomicalToMechanicalAngle(Transform tr)
        {
            Landmark landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter");
            Vector3 femoralCenter = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "hipCenter");
            Vector3 hipCenter = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(s => s.Type == "midfemurCenter");
            Vector3 midfemurCenter = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(s => s.Type == "medialSulcus");
            Vector3 medialSulcus = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(s => s.Type == "lateralEpicondyle");
            Vector3 lateralEpicondyle = tr.TransformPoint(landmark.Position);
            lateralEpicondyle = tr.TransformPoint(lateralEpicondyle);

            Vector3 femurMA = femoralCenter - hipCenter;
            Vector3 plane = medialSulcus - lateralEpicondyle;
            Vector3 projectPlane = Vector3.Cross(plane, femurMA);

            femoralCenter = Vector3.ProjectOnPlane(femoralCenter, projectPlane);
            hipCenter = Vector3.ProjectOnPlane(hipCenter, projectPlane);
            midfemurCenter = Vector3.ProjectOnPlane(midfemurCenter, projectPlane);

            Vector3 Zv = femoralCenter - hipCenter;
            Vector3 Ztv = femoralCenter - midfemurCenter;

            double anglef = Vector3.Angle(Zv, Ztv);
            return anglef;
        }

        public static float AngleBetween(Vector3 axis1, Vector3 axis2, Vector3 refPlane)
        {
            var angle = Vector3.Angle(axis1, axis2);
            if (Math.Abs(angle) > 90)
            {
                angle = Math.Sign(angle) * (180 - Math.Abs(angle));
            }

            var cross = Vector3.Cross(axis1, axis2);
            if (Vector3.Dot(refPlane, cross) < 0)
            {
                angle = -angle;
            }

            return angle;
        }

        /// <summary>
        /// Calculate flexion/extension.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        internal static double MeasureFlexion(Transform tr)
        {
            Landmark landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "medialSulcus");
            var medialSulcus = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "lateralEpicondyle");
            Vector3 lateralEpicondyle = tr.TransformPoint(landmark.Position);
            lateralEpicondyle = tr.TransformPoint(lateralEpicondyle);

            var plane = Patient.Instance.Leftright.ToLower() == "left" ? lateralEpicondyle - medialSulcus : medialSulcus - lateralEpicondyle;

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter");
            Vector3 femoralCenter = landmark.Position;
            femoralCenter = tr.TransformPoint(femoralCenter);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "hipCenter");
            Vector3 hipCenter = landmark.Position;
            hipCenter = tr.TransformPoint(hipCenter);

            femoralCenter = Vector3.ProjectOnPlane(femoralCenter, plane);
            hipCenter = Vector3.ProjectOnPlane(hipCenter, plane);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "medialMalleolus");
            Vector3 medialMalleolus = landmark.Position;

            Landmark landmark2 = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "lateralMalleolus");
            Vector3 lateralMalleolus = landmark2.Position;

            Vector3 outpoint1 = MathUtil.GetMidPoint(medialMalleolus, lateralMalleolus);
            outpoint1 = tr.TransformPoint(outpoint1);
            outpoint1 = Vector3.ProjectOnPlane(outpoint1, plane);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "Tubercle");
            Vector3 tubercle = landmark.Position;

            landmark2 = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "PCLInsertion");
            Vector3 PCLInsertion = landmark2.Position;

            Vector3 outpoint2 = MathUtil.GetMidPoint(tubercle, PCLInsertion);
            outpoint2 = tr.TransformPoint(outpoint2);
            outpoint2 = Vector3.ProjectOnPlane(outpoint2, plane);

            Vector3 Zv = femoralCenter - hipCenter;
            Vector3 Ztv = outpoint1 - outpoint2;

            double anglef = AngleBetween(Ztv, Zv, plane);
            return anglef;
        }

        /// <summary>
        /// Calculate varus/valgus.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        internal static double MeasureVarusValgus(Transform tr)
        {
            Landmark landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter");
            Vector3 femoralCenter = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "hipCenter");
            Vector3 hipCenter = tr.TransformPoint(landmark.Position);


            landmark = Patient.Instance.Landmarks.FirstOrDefault(s => s.Type == "medialSulcus");
            Vector3 medialSulcus = tr.TransformPoint(landmark.Position);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(s => s.Type == "lateralEpicondyle");
            Vector3 lateralEpicondyle = tr.TransformPoint(landmark.Position);
            lateralEpicondyle = tr.TransformPoint(lateralEpicondyle); ;

            Vector3 femurMA = femoralCenter - hipCenter;
            Vector3 plane = medialSulcus - lateralEpicondyle;
            Vector3 projectPlane = Vector3.Cross(plane, femurMA);

            femoralCenter = Vector3.ProjectOnPlane(femoralCenter, projectPlane);
            hipCenter = Vector3.ProjectOnPlane(hipCenter, projectPlane);

            Vector3 Zv = femoralCenter - hipCenter;

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "medialMalleolus");
            Vector3 medialMalleolus = landmark.Position;

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "lateralMalleolus");
            Vector3 lateralMalleolus = landmark.Position;

            Vector3 outpoint1 = MathUtil.GetMidPoint(medialMalleolus, lateralMalleolus);
            outpoint1 = tr.TransformPoint(outpoint1);

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "Tubercle");
            Vector3 tubercle = landmark.Position;

            landmark = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "PCLInsertion");
            Vector3 PCLInsertion = landmark.Position;

            Vector3 outpoint2 = MathUtil.GetMidPoint(tubercle, PCLInsertion);
            outpoint2 = tr.TransformPoint(outpoint2);

            outpoint1 = Vector3.ProjectOnPlane(outpoint1, projectPlane);
            outpoint2 = Vector3.ProjectOnPlane(outpoint2, projectPlane);

            Vector3 Ztv = outpoint1 - outpoint2;

            double anglef = Vector3.Angle(Zv, Ztv);
            return anglef;
        }
    }
}
