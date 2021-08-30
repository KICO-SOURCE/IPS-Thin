#region Usings

using Assets.CaseFile;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A viewport class for visualize patella alignments.
    /// </summary>
    public class PatellaViewport : Viewport
    {
        #region Private Members

        private _3DView nativePatellaView;
        private _3DView coronalView;
        private _3DView resectedCoronalView;
        private _3DView axialView;
        private _3DView resectedAxialView;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public PatellaViewport(Patient patient)
        {
            this.patient = patient;
            nativePatellaView = new _3DView() { Postion = new Vector2(0, 0), Size = new Vector2(0.4f, 0.95f) };
            axialView = new _3DView() { Postion = new Vector2(0.4f, 0), Size = new Vector2(0.3f, 0.475f), RotationAngle = -90 };
            coronalView = new _3DView() { Postion = new Vector2(0.4f, 0.475f), Size = new Vector2(0.3f, 0.475f) };
            resectedAxialView = new _3DView() { Postion = new Vector2(0.7f, 0), Size = new Vector2(0.3f, 0.475f), RotationAngle = -90 };
            resectedCoronalView = new _3DView() { Postion = new Vector2(0.7f, 0.475f), Size = new Vector2(0.3f, 0.475f) };

            Views.Add(nativePatellaView);
            Views.Add(axialView);
            Views.Add(coronalView);
            Views.Add(resectedAxialView);
            Views.Add(resectedCoronalView);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public override void CreateViews()
        {
            var origin = patient.GetLandmarkPosition("posteriorApex");
            Ips.Utils.MeasurementUtils.GetPatellaAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);

            var meshes = patient.GetMeshes("Patella");
            InitialiseMeshes(origin, meshes);

            var mask = GetCullingMask("Patella");

            nativePatellaView.InitialiseView(mask, origin, apAxis);
            coronalView.InitialiseView(mask, origin, apAxis);
            resectedCoronalView.InitialiseView(mask, origin, apAxis);
            axialView.InitialiseView(mask, origin, -siAxis);
            resectedAxialView.InitialiseView(mask, origin, -siAxis);
            base.CreateViews();
        }

        #endregion
    }
}