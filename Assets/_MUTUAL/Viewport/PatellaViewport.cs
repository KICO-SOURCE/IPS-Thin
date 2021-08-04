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
            axialView = new _3DView() { Postion = new Vector2(0.4f, 0), Size = new Vector2(0.3f, 0.5f) };
            coronalView = new _3DView() { Postion = new Vector2(0.4f, 0.5f), Size = new Vector2(0.3f, 0.5f) };
            resectedAxialView = new _3DView() { Postion = new Vector2(0.7f, 0), Size = new Vector2(0.3f, 0.5f) };
            resectedCoronalView = new _3DView() { Postion = new Vector2(0.7f, 0.5f), Size = new Vector2(0.3f, 0.5f) };

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
            if (patient.Landmarks.Any(lm => lm.Type == "posteriorApex"))
            {
                Ips.Utils.MeasurementUtils.GetPatellaAxes(patient, out var siAxis, out var mlAxis, out var apAxis);
                var meshes = patient.MeshGeoms.Where(m => m.Key == "Patella").ToDictionary(x => x.Key, x => x.Value);

                var origin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "posteriorApex").Position;
                nativePatellaView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);
                coronalView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);
                resectedCoronalView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);

                origin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "quadTendonInsertion").Position;
                axialView.InitialiseView(meshes, ViewType.AxialView, origin, -siAxis, mlAxis, apAxis);
                resectedAxialView.InitialiseView(meshes, ViewType.AxialView, origin, -siAxis, mlAxis, apAxis);
            }
            base.CreateViews();
        }

        #endregion
    }
}