using System.Collections.Generic;

namespace Assets.Geometries
{
    public class GeometryManager
    {
        public List<Geometry> Geometries;

        public GeometryManager()
        {
            Geometries = new List<Geometry>();
            AddContents();
        }

        //Dummy data
        private void AddContents()
        {
            Geometries.Add(new Geometry() { Tag = "DistalFemur" });
            Geometries.Add(new Geometry() { Tag = "DistalTibia" });
            Geometries.Add(new Geometry() { Tag = "FemurComponent" });
            Geometries.Add(new Geometry() { Tag = "PatellaComponent" });
        }
    }
}
