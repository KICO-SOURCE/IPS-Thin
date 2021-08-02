#region Usings

using Assets.CaseFile;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A dummy viewport class for testing.
    /// </summary>
    public class TestViewport : Viewport
    {
        #region Private Members

        private _3DView _3DView;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public TestViewport(Patient patient)
        {
            this.patient = patient;
            Views.Add(new TestView() { Postion = new Vector2(0, 0), Size = new Vector2(100,100) });

            _3DView = new _3DView() { Postion = new Vector2(0, 0), Size = new Vector2(100, 100) };
            Views.Add(_3DView);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public override void Activate()
        {
            if (patient.Landmarks.Any(lm => lm.Type == "femoralCenter"))
            {
                var origin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter").Position;
                Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis, out var mlAxis, out var apAxis);

                var meshes = patient.MeshGeoms.Where(m => m.Key == "DistalFemur").ToDictionary(x => x.Key, x => x.Value);
                _3DView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);
            }
            base.Activate();
        }

        #endregion
    }
}