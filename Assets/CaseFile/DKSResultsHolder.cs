using System.Collections.Generic;
using Assets.CaseFile.Enums;
using Assets.CaseFile.DataConnector;

namespace Assets.CaseFile
{
    public class DKSResultsHolder
    {
        public List<SensitivityResults> Results { get; set; }
        public DatabaseDKS RawResultsFromDB { get; set; }
        internal DKSViewType Type { get; set; }

        public List<bool> IsCongruent = new List<bool>();
        public List<int> YMax = new List<int>();
        public List<int> YMin = new List<int>();
        public List<int> XMax = new List<int>();
        public List<int> XMin = new List<int>();


        public DKSResultsHolder()
        {
            Results = new List<SensitivityResults>();
        }
    }
}