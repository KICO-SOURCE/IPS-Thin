using Assets.CaseFile.Components;
using System.Collections.Generic;

namespace Assets.CaseFile
{
    internal class Project
    {
        #region Fields

        public List<Dictionary<string, string>> PlanValues { get; set; }
        public List<Measurement> FunctionalValues { get; set; }

        /// <summary>
        /// Holds the each component positional data against a plan (stored against a plan index).
        /// </summary>
        public Dictionary<int, Dictionary<string, PositionalData>> PlanComponentPosition { get; set; }

        /// <summary>
        /// Holds each component data against plan (stored against a plan index).
        /// </summary>
        public Dictionary<int, Dictionary<ComponentType, Implant>> PlanImplants { get; set; }

        #endregion

        #region Public Methods

        public Project()
        {
            InitializeData();
        }

        public void InitializeData()
        {
            PlanValues = new List<Dictionary<string, string>>();
            FunctionalValues = new List<Measurement>();
            PlanComponentPosition = new Dictionary<int, Dictionary<string, PositionalData>>();
            PlanImplants = new Dictionary<int, Dictionary<ComponentType, Implant>>();
        }

        #endregion
    }
}
