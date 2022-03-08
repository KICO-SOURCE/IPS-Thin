using Assets.CaseFile.Components;
using System;
using System.Collections.Generic;

namespace Assets.CaseFile
{
    public class Project
    {
        #region Fields

        private static readonly Lazy<Project> _instance = new Lazy<Project>(() => new Project());

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Project Instance
        {
            get { return _instance.Value; }
        }

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

        private Project()
        {
            InitializeData();
        }

        #region Public Methods

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
