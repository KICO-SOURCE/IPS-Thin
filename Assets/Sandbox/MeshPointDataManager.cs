using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Sandbox
{
    /// <summary>
    /// Mesh vertices population manager
    /// </summary>
    public class MeshPointDataManager : MonoBehaviour
    {
        #region Private Fields
        int Index = 0;

        #endregion

        #region Private Members

        private static readonly Lazy<MeshPointDataManager> _instance = new Lazy<MeshPointDataManager>(() => new MeshPointDataManager());

        #endregion

        #region Public Fields

        public GameObject PointList;
        public GameObject PointListTemplate;
        public GameObject viewPrefab;
        public GameObject row;

        #endregion

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static MeshPointDataManager Instance
        {
            get { return _instance.Value; }
        }

        #region Constructor

        /// <summary>
        /// Creates new instance of MeshPointDataManager.
        /// </summary>
        private MeshPointDataManager()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sort mesh vertices by x
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetMeshVerticesSortByX(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.OrderBy(p => p.x).ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Sort mesh vertices by y
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetMeshVerticesSortByY(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.OrderBy(p => p.y).ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Sort mesh vertices by z
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetMeshVerticesSortByZ(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.OrderBy(p => p.z).ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// First 100 mesh vertices
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetFirst100MeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
                if (Index == 100) break;
            }
        }


        /// <summary>
        /// First N mesh vertices
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetFirstNMeshVerticesList(Mesh mesh, int verticesCount, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
                if (Index == verticesCount) break;
            }
        }


        /// <summary>
        /// Loaded bone vertices list
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetLoadedBoneMeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            int Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Optional bone vertices list
        /// </summary>
        /// <param name="mesh">optional bone mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetOptionalBoneMeshVerticesList(Mesh mesh, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                meshPoints.Add(Index, point);
                Index++;
            }
        }


        /// <summary>
        /// Filter the mesh vertices
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        private void GetFilteredMeshVerticesList(Mesh mesh, int startIndx, int endIndx, Dictionary<int, Vector3> meshPoints)
        {
            List<Vector3> points = new List<Vector3>();
            points = mesh.vertices.ToList();
            Index = 0;
            foreach (var point in points)
            {
                if (Index > startIndx && Index < endIndx)
                {
                    meshPoints.Add(Index, point);
                }
                Index++;
            }
        }

        #endregion


        /// <summary>
        /// Display the mesh vertices list in SAndBox UI
        /// </summary>
        /// <param name="mesh">Loaded mesh</param>
        /// <param name="meshPoints">sorted mesh vertices</param>
        public void ShowMeshVerticesList(GameObject uiParent, GameObject mesh, string operation, Mesh optionalBone)
        {
            var filter = mesh.GetComponent<MeshFilter>();
            Mesh bone = filter.mesh;

            Dictionary<int, Vector3> meshPoints = new Dictionary<int, Vector3>();

            if (operation == "Sort by X")
            {
                GetMeshVerticesSortByX(bone, meshPoints);
            }

            else if (operation == "Sort by Y")
            {
                GetMeshVerticesSortByY(bone, meshPoints);
            }

            else if (operation == "Sort by Z")
            {
                GetMeshVerticesSortByZ(bone, meshPoints);
            }

            else if (operation == "First 100 vertices")
            {
                GetFirst100MeshVerticesList(bone, meshPoints);
            }

            else if (operation == "First N Vertices List")
            {
                GetFirstNMeshVerticesList(bone, 10, meshPoints);
            }

            else if (operation == "Loaded bone vertices list")
            {
                GetLoadedBoneMeshVerticesList(bone, meshPoints);
            }

            else if (operation == "Optional bone vertices list")
            {
                if(optionalBone != null)
                GetOptionalBoneMeshVerticesList(optionalBone, meshPoints);
            }
            else if (operation == "Filtered vertices List")
            {
                GetFilteredMeshVerticesList(bone, 1, 10, meshPoints);
            }

            var prefab = Resources.Load<GameObject>("Prefabs/MeshVertexList");
            viewPrefab = UnityEngine.Object.Instantiate(prefab, uiParent.transform);

            var viewport = viewPrefab.transform.Find("Viewport").gameObject;
            var content = viewport.transform.Find("Content").gameObject;
            PointList = content.transform.Find("PointList").gameObject;
            GameObject point = UnityEngine.Object.Instantiate(PointList, content.transform);
            PointListTemplate = PointList.transform.Find("PointListTemplate").gameObject;

            for (int i = 0; i< meshPoints.Count; i++)
            {
                row = UnityEngine.Object.Instantiate(PointListTemplate, point.transform);
                row.transform.localPosition = new Vector3(0, i * -50, 0);

                var cell = row.transform.Find("NO");
                var text = cell.GetComponent<TextMeshProUGUI>();
                text.text = i.ToString();
                text.color = Color.black;
                text.fontSize = 15;

                cell = row.transform.Find("X");
                var text1 = cell.GetComponent<TextMeshProUGUI>();
                text1.text = meshPoints[i].x.ToString();
                text1.color = Color.black;
                text1.fontSize = 15;

                cell = row.transform.Find("Y");
                var text2 = cell.GetComponent<TextMeshProUGUI>();
                text2.text = meshPoints[i].y.ToString();
                text2.color = Color.black;
                text2.fontSize = 15;

                cell = row.transform.Find("Z");
                var text3 = cell.GetComponent<TextMeshProUGUI>();
                text3.text = meshPoints[i].z.ToString();
                text3.color = Color.black;
                text3.fontSize = 15;

                row.SetActive(true);

                if (i > 500) break;
            }

            Destroy(viewPrefab);
        }

    }
}
