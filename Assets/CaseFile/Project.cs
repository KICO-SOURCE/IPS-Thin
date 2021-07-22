using System.Collections.Generic;

namespace Assets.CaseFile
{
    internal class Project
    {
        #region Fields

        public List<Dictionary<string, string>> PlanValues { get; set; }
        public List<Measurement> FunctionalValues { get; set; }

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
        }

        #endregion
    }
}
